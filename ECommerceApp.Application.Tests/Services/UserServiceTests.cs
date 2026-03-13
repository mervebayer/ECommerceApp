using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Application.Services;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Exceptions;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly Mock<IValidator<UserQueryParams>> _queryParamsValidatorMock;

        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _queryParamsValidatorMock = new Mock<IValidator<UserQueryParams>>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _loggerMock.Object, 
                _queryParamsValidatorMock.Object);
        }

        [Fact]
        public async Task AssignRoleAsync_RoleNameIsEmpty_ShouldThrowBadRequestException()
        {
            //Arrange
            var userId = "1";
            var roleName = "";

            
            Func<Task> act = async () =>
                await _userService.AssignRoleAsync(userId, roleName, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<BadRequestException>().WithMessage("RoleName is required."); ;
        }

        [Fact]
        public async Task AssignRoleAsync_UserIsNull_ShouldThrowNotFoundException()
        {
            var userId = "1";
            var roleName = "Admin";

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AppUser?)null);


            Func<Task> act = async () =>
                await _userService.AssignRoleAsync(userId, roleName, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found.");
        }

        [Fact]
        public async Task AssignRoleAsync_AlreadyInRole_ShouldThrowBadRequestException()
        {
            var userId = "1";
            var roleName = "Admin";

            var user = new AppUser
            {
                Id = userId
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x =>x.IsInRoleAsync(user, roleName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            Func<Task> act = async () =>
               await _userService.AssignRoleAsync(userId, roleName, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>().WithMessage("User already has this role.");

        }

        [Fact]
        public async Task AssignRoleAsync_UserNotInRole_ShouldAddRoleSuccessfully()
        {
            var userId = "1";
            var roleName = "Admin";

            var user = new AppUser
            {
                Id = userId
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x => x.IsInRoleAsync(user, roleName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);


            await _userService.AssignRoleAsync(userId, roleName, CancellationToken.None);

            _userRepositoryMock.Verify
                (x=>x.AddToRoleAsync(user, roleName, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RemoveRoleAsync_RoleNameIsEmpty_ShouldThrowBadRequestException()
        {
            var userId = "1";
            var roleName = "";

            Func<Task> act = async () =>
                await _userService.RemoveRoleAsync(userId, roleName, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>().WithMessage("RoleName is required.");
        }

        [Fact]
        public async Task RemoveRoleAsync_UserNotFound_ShouldThrowNotFoundException()
        {
            var userId = "1";
            var roleName = "Admin";

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AppUser?)null);

            Func<Task> act = async () =>
                await _userService.RemoveRoleAsync(userId, roleName, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found.");
        }

        [Fact]
        public async Task RemoveRoleAsync_UserNotInRole_ShouldThrowBadRequestException()
        {
            var userId = "1";
            var roleName = "Admin";

            var user = new AppUser
            {
                Id = userId
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x => x.IsInRoleAsync(user, roleName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            Func<Task> act = async () =>
                await _userService.RemoveRoleAsync(userId, roleName, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>().WithMessage("User does not have this role.");
        }

        [Fact]
        public async Task RemoveRoleAsyncc_UserInRole_ShouldRemoveRoleSuccessfully()
        {
            var userId = "1";
            var roleName = "Admin";

            var user = new AppUser
            {
                Id = userId
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x => x.IsInRoleAsync(user, roleName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await _userService.RemoveRoleAsync(userId, roleName, CancellationToken.None);

            _userRepositoryMock.Verify
                (x => x.RemoveFromRoleAsync(user, roleName, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task GetUserRolesAsync_UserNotFound_ShouldThrowNotFoundException()
        {
            var userId = "1";

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AppUser?)null);

            Func<Task> act = async () =>
                await _userService.GetUserRolesAsync(userId, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found.");
        }

        [Fact]
        public async Task GetUserRolesAsync_UserFound_ShouldReturnRolesSuccessfully()
        {
            var userId = "1";

            AppUser user = new AppUser
            {
                Id = userId,
            };

            var roles = new List<string> { "Admin", "Moderator" };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x => x.GetRolesAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);


            var result = await _userService.GetUserRolesAsync(userId, CancellationToken.None);

            result.Should().BeEquivalentTo(roles);

            _userRepositoryMock.Verify
                (x => x.GetRolesAsync(user, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async Task GetUserByIdAsync_UserNotFound_ShouldThrowNotFoundException()
        {
            var userId = "1";

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AppUser?)null);

            Func<Task> act = async () =>
                await _userService.GetUserByIdAsync(userId, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found.");

            _userRepositoryMock.Verify(
                 x => x.GetRolesAsync(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserFound_ShouldReturnUserDetailDtoSuccessfully()
        {
            // Arrange
            var userId = "1";

            var user = new AppUser
            {
                Id = userId,
                UserName = "merve",
                Email = "merve@test.com",
                FirstName = "Merve",
                LastName = "Bay"
            };

            var roles = new List<string> { "Admin", "Moderator" };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x => x.GetRolesAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            // Act
            var result = await _userService.GetUserByIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.UserName.Should().Be(user.UserName);
            result.Email.Should().Be(user.Email);
            result.FirstName.Should().Be(user.FirstName);
            result.LastName.Should().Be(user.LastName);
            result.Roles.Should().BeEquivalentTo(roles);

            _userRepositoryMock.Verify(
                x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.GetRolesAsync(user, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
