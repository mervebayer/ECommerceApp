using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.DTOs.Users;
using ECommerceApp.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authenticationService.LoginAsync(loginDto);
            return Ok(result); 
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var result = await _authenticationService.CreateTokenByRefreshTokenAsync(refreshToken);
            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto registerDto)
        {
            var result = await _authenticationService.RegisterAsync(registerDto);
            return Ok(result);
        }
    }
}
