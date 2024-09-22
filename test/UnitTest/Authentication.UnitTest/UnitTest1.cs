using Microsoft.AspNetCore.Identity;
using Moq;
using Authentication.API.Services;
using GitStartFramework.Shared.Exceptions;
using Authentication.API.Domain.Entities;
using Authentication.API.Repository;

namespace Authentication.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            _userService = new UserService(_mockPasswordHasher.Object, _mockUserRepository.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_UserAlreadyExists_ThrowsBadHttpRequestException()
        {
            var email = "test@example.com";
            _mockUserRepository.Setup(repo => repo.AnyAsync(u => u.Email == email)).ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _userService.RegisterUserAsync("username", email, "password"));

            Assert.Equal("Email is already registered.", exception.Message);
        }

        [Fact]
        public async Task RegisterUserAsync_Success_ReturnsUser()
        {
            var email = "test@example.com";
            var user = new User { Id = Guid.NewGuid(), Email = email };
            _mockUserRepository.Setup(repo => repo.AnyAsync(u => u.Email == email)).ReturnsAsync(false);
            _mockPasswordHasher.Setup(p => p.HashPassword(user, "password")).Returns("hashed_password");
            _mockUserRepository.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var response = await _userService.RegisterUserAsync("username", email, "password");

            // Assert
            Assert.NotNull(response.Data);
            Assert.Equal(email, response.Data.Email);
        }

        [Fact]
        public async Task LoginUserAsync_InvalidCredentials_ThrowsBadRequestException()
        {
            var email = "test@example.com";
            var password = "wrong_password";
            _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((User)null);

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _userService.LoginUserAsync(email, password));

            Assert.Equal("Invalid email or password.", exception.Message);
        }

        [Fact]
        public async Task LoginUserAsync_Success_ReturnsAuthToken()
        {
            var email = "test@example.com";
            var password = "password";
            var user = new User { Id = Guid.NewGuid(), Email = email, PasswordHash = "hashed_password" };
            _mockUserRepository.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);
            _mockPasswordHasher.Setup(p => p.VerifyHashedPassword(user, user.PasswordHash, password)).Returns(PasswordVerificationResult.Success);
            _mockTokenService.Setup(ts => ts.GenerateToken(user)).Returns("token");

            // Act
            var response = await _userService.LoginUserAsync(email, password);

            // Assert
            Assert.NotNull(response.Data);
            Assert.Equal("token", response.Data.AuthToken);
        }

        [Fact]
        public async Task UpdateUserAsync_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _userService.UpdateUserAsync(userId, "username", "test@example.com", "newpassword"));

            Assert.Equal("User not found.", exception.Message);
        }

        [Fact]
        public async Task UpdateUserAsync_EmailAlreadyExists_ThrowsBadHttpRequestException()
        {
            var userId = Guid.NewGuid();
            var existingEmail = "existing@example.com";
            var user = new User { Id = userId, Email = "old@example.com" };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.AnyAsync(u => u.Email == existingEmail)).ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _userService.UpdateUserAsync(userId, "username", existingEmail, null));

            Assert.Equal("Email is already registered by another user.", exception.Message);
        }

        [Fact]
        public async Task UpdateUserAsync_Success_ReturnsUpdatedUser()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "old@example.com", Username = "oldusername" };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockPasswordHasher.Setup(p => p.HashPassword(user, "newpassword")).Returns("new_hashed_password");

            // Act
            var response = await _userService.UpdateUserAsync(userId, "newusername", "new@example.com", "newpassword");

            // Assert
            Assert.NotNull(response.Data);
            Assert.Equal("new@example.com", response.Data.Email);
            Assert.Equal("newusername", response.Data.Username);
        }

        [Fact]
        public async Task DeleteUserAsync_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _userService.DeleteUserAsync(userId));

            Assert.Equal("User not found.", exception.Message);
        }

        [Fact]
        public async Task DeleteUserAsync_Success_ReturnsTrue()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.DeleteAsync(user)).Returns(Task.CompletedTask);

            // Act
            var response = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.True(response.Data);
        }
    }
}