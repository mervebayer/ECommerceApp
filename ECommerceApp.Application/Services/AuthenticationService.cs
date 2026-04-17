using AutoMapper;
using ECommerceApp.Application.Common.Helpers;
using ECommerceApp.Application.DTOs;
using ECommerceApp.Application.DTOs.Auth;
using ECommerceApp.Application.DTOs.Users;
using ECommerceApp.Application.Extensions;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ECommerceApp.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IValidator<UserRegisterDto> _registerValidator;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<RefreshTokenRequestDto> _refreshTokenValidator;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IEmailService _emailService;
    private readonly IAppUrlProvider _appUrlProvider;


    public AuthenticationService(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper, IValidator<UserRegisterDto> registerValidator, IUserRepository userRepository, ILogger<AuthenticationService> logger, IValidator<LoginDto> loginValidator, IValidator<RefreshTokenRequestDto> refreshTokenValidator, IEmailService emailService, IAppUrlProvider appUrlProvider)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _registerValidator = registerValidator;
        _userRepository = userRepository;
        _logger = logger;
        _loginValidator = loginValidator;
        _refreshTokenValidator = refreshTokenValidator;
        _emailService = emailService;
        _appUrlProvider = appUrlProvider;
    }

    public async Task<TokenDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validationResult = await _loginValidator.ValidateAsync(loginDto, cancellationToken);
        validationResult.ThrowIfInvalid();

        var user =
            await _userManager.FindByEmailAsync(loginDto.UsernameOrEmail)
            ?? await _userManager.FindByNameAsync(loginDto.UsernameOrEmail);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            _logger.LogWarning("Login failed. Invalid credentials. UsernameOrEmail={UsernameOrEmail}", loginDto.UsernameOrEmail);
            throw new UnauthorizedException("Invalid username/email or password.");
        }

        if (!user.EmailConfirmed)
            throw new UnauthorizedException("Email adresinizi dogrulamadan giris yapamazsiniz.");


        var roles = await _userManager.GetRolesAsync(user);

        var tokenDto = await _tokenService.CreateTokenAsync(user, roles);

        user.RefreshToken = RefreshTokenHasher.Hash(tokenDto.RefreshToken);
        user.RefreshTokenExpiration = tokenDto.RefreshTokenExpiration;

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
        {
            var errors = string.Join(", ", update.Errors.Select(e => e.Description));
            _logger.LogError("Failed to persist refresh token on login. UserId={UserId}, Errors={Errors}", user.Id, errors);
            throw new BadRequestException("Login succeeded but token persistence failed.");
        }

        return tokenDto;
    }

    public async Task<TokenDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validationResult = await _refreshTokenValidator.ValidateAsync(request, cancellationToken);
        validationResult.ThrowIfInvalid();

        var hashedRefreshToken = RefreshTokenHasher.Hash(request.RefreshToken);
        var user = await _userRepository.GetByRefreshTokenAsync(hashedRefreshToken, cancellationToken);

        if (user is null || user.RefreshTokenExpiration < DateTime.UtcNow)
        {
            _logger.LogWarning("Invalid or expired refresh token attempt.");
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var tokenDto = await _tokenService.CreateTokenAsync(user, roles);

        user.RefreshToken = RefreshTokenHasher.Hash(tokenDto.RefreshToken);
        user.RefreshTokenExpiration = tokenDto.RefreshTokenExpiration;

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
        {
            var errors = string.Join(", ", update.Errors.Select(e => e.Description));
            _logger.LogError("Failed to rotate refresh token. UserId={UserId}, Errors={Errors}", user.Id, errors);
            throw new BadRequestException("Token rotation failed.");
        }

        return tokenDto;
    }

    public async Task<UserDto> RegisterAsync(UserRegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _registerValidator.ValidateAsync(registerDto, cancellationToken);
        validationResult.ThrowIfInvalid();

        // Uniqueness checks
        var userExists = await _userManager.FindByNameAsync(registerDto.UserName);
        if (userExists != null)
            throw new BadRequestException("UserName already taken.");

        if (!string.IsNullOrWhiteSpace(registerDto.Email))
        {
            var emailExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (emailExists != null)
                throw new BadRequestException("Email already taken.");
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = _mapper.Map<AppUser>(registerDto);

            var createResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Registration failed during user creation. UserName={UserName}, Errors={Errors}",
                    registerDto.UserName, errors);

                throw new BadRequestException($"Registration failed: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Registration failed during role assignment. UserId={UserId}, Errors={Errors}",
                    user.Id, errors);

                // Compensating action
                await _userManager.DeleteAsync(user);

                throw new BadRequestException($"Authorization error: {errors}");
            }

            _logger.LogInformation("User registered successfully. UserId={UserId}", user.Id);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationLink = _appUrlProvider.GetEmailConfirmationUrl(user.Id, encodedToken);


            await TrySendEmailAsync(
                user.Email,
                "E-posta adresinizi dogrulayin",
                            $"""
                <h2>Merhaba {user.FirstName},</h2>
                <p>Kaydinizi tamamlamak icin asagidaki baglantiya tiklayin:</p>
                <p><a href="{confirmationLink}">E-postami dogrula</a></p>
                """);


            return _mapper.Map<UserDto>(user);
        }
        catch (OperationCanceledException) { throw; }
        catch (BadRequestException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration. UserName={UserName}", registerDto.UserName);
            throw new BadRequestException("A technical error occurred during the registration process.");
        }
    }

    public async Task LogoutAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validationResult = await _refreshTokenValidator.ValidateAsync(request, cancellationToken);
        validationResult.ThrowIfInvalid();

        var hashedRefreshToken = RefreshTokenHasher.Hash(request.RefreshToken);
        var user = await _userRepository.GetByRefreshTokenAsync(hashedRefreshToken, cancellationToken);

        if (user is null)
            return;

        user.RefreshToken = null;
        user.RefreshTokenExpiration = null;

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
        {
            var errors = string.Join(", ", update.Errors.Select(e => e.Description));
            throw new BadRequestException($"Logout failed: {errors}");
        }
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return false;

        try
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            return result.Succeeded;
        }
        catch (FormatException ex)
        {
            _logger.LogWarning(ex, "Invalid email confirmation token format. UserId={UserId}", userId);
            return false;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid email confirmation token. UserId={UserId}", userId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Email confirmation failed unexpectedly. UserId={UserId}", userId);
            return false;
        }
    }




    private async Task TrySendEmailAsync(string? toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            return;

        try
        {
            await _emailService.SendAsync(toEmail, subject, htmlBody);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Registration email could not be sent. To={ToEmail}", toEmail);
        }
    }


}