using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace FiapCloudGames.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        #region ObterPorId Tests

        [Fact]
        public async Task TaskObterPorId_WithExistingUser_ShouldReturnUser()
        {
            // Arrange
            var userId = 123;
            var expectedUser = new User
            {
                Id = 123,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User,
                PasswordHash = "hashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedUser);
            result!.Id.Should().Be(expectedUser.Id);
            result.Name.Should().Be(expectedUser.Name);
            result.Email.Should().Be(expectedUser.Email);
            result.Role.Should().Be(expectedUser.Role);
            result.PasswordHash.Should().Be(expectedUser.PasswordHash);

            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task TaskObterPorId_WithNonExistingUser_ShouldReturnNull()
        {
            // Arrange
            var userId = 999;
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.Should().BeNull();
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        }


        [Fact]
        public async Task TaskObterPorId_WithAdministratorUser_ShouldReturnCorrectRole()
        {
            // Arrange
            var userId = 123;
            var adminUser = new User
            {
                Id = 456,
                Name = "Admin User",
                Email = "admin@example.com",
                Role = UserRole.Admin,
                PasswordHash = "adminhashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(adminUser);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result!.Role.Should().Be(UserRole.Admin);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        }

        #endregion

        #region Criar Tests

        [Fact]
        public async Task TaskCriar_WithValidUser_ShouldCallRepositoryAdd()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "New User",
                Email = "newuser@example.com",
                Role = UserRole.User,
                PasswordHash = "hashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.CreateAsync(user)).Verifiable();

            // Act
            await _userService.CreateUserAsync(user);

            // Assert
            _mockUserRepository.Verify(repo => repo.CreateAsync(user), Times.Once);
        }

        [Fact]
        public async Task TaskCriar_WithNullUser_ShouldCallRepositoryWithNull()
        {
            // Arrange
            User nullUser = null!;

            // Act
            await _userService.CreateUserAsync(nullUser);

            // Assert
            _mockUserRepository.Verify(repo => repo.CreateAsync(nullUser), Times.Once);
        }

        [Fact]
        public async Task TaskCriar_WithAdministratorUser_ShouldCallRepositoryAdd()
        {
            // Arrange
            var adminUser = new User
            {
                Id = 2,
                Name = "Admin User",
                Email = "admin@example.com",
                Role = UserRole.Admin,
                PasswordHash = "adminhashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.CreateAsync(adminUser)).Verifiable();

            // Act
            await _userService.CreateUserAsync(adminUser);

            // Assert
            _mockUserRepository.Verify(repo => repo.CreateAsync(adminUser), Times.Once);
        }

        #endregion

    }
}
