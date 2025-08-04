using Xunit;
using Moq;
using FluentAssertions;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using FiapCloudGames.Domain.Enums;

namespace FiapCloudGames.Tests.Services
{
    public class UserServiceests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidRepository_ShouldCreateInstance()
        {
            // Arrange & Act
            var service = new UserService(_mockUserRepository.Object);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion

        #region ObterPorId Tests

        [Fact]
        public void ObterPorId_WithExistingUser_ShouldReturnUser()
        {
            // Arrange
            var userId = "123";
            var expectedUser = new User
            {
                Id = 123,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.Usuario,
                PasswordHash = "hashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.GetById(userId)).Returns(expectedUser);

            // Act
            var result = _userService.ObterPorId(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedUser);
            result!.Id.Should().Be(expectedUser.Id);
            result.Name.Should().Be(expectedUser.Name);
            result.Email.Should().Be(expectedUser.Email);
            result.Role.Should().Be(expectedUser.Role);
            result.PasswordHash.Should().Be(expectedUser.PasswordHash);

            _mockUserRepository.Verify(repo => repo.GetById(userId), Times.Once);
        }

        [Fact]
        public void ObterPorId_WithNonExistingUser_ShouldReturnNull()
        {
            // Arrange
            var userId = "999";
            _mockUserRepository.Setup(repo => repo.GetById(userId)).Returns((User?)null);

            // Act
            var result = _userService.ObterPorId(userId);

            // Assert
            result.Should().BeNull();
            _mockUserRepository.Verify(repo => repo.GetById(userId), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("abc")]
        [InlineData("user@email.com")]
        [InlineData("very-long-user-id-123456789")]
        public void ObterPorId_WithVariousIdFormats_ShouldCallRepository(string userId)
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetById(userId)).Returns((User?)null);

            // Act
            var result = _userService.ObterPorId(userId);

            // Assert
            _mockUserRepository.Verify(repo => repo.GetById(userId), Times.Once);
        }

        [Fact]
        public void ObterPorId_WithNullId_ShouldCallRepositoryWithNull()
        {
            // Arrange
            string nullId = null!;
            _mockUserRepository.Setup(repo => repo.GetById(nullId)).Returns((User?)null);

            // Act
            var result = _userService.ObterPorId(nullId);

            // Assert
            result.Should().BeNull();
            _mockUserRepository.Verify(repo => repo.GetById(nullId), Times.Once);
        }

        [Fact]
        public void ObterPorId_WithAdministratorUser_ShouldReturnCorrectRole()
        {
            // Arrange
            var userId = "admin123";
            var adminUser = new User
            {
                Id = 456,
                Name = "Admin User",
                Email = "admin@example.com",
                Role = UserRole.Administrador,
                PasswordHash = "adminhashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.GetById(userId)).Returns(adminUser);

            // Act
            var result = _userService.ObterPorId(userId);

            // Assert
            result.Should().NotBeNull();
            result!.Role.Should().Be(UserRole.Administrador);
            _mockUserRepository.Verify(repo => repo.GetById(userId), Times.Once);
        }

        [Fact]
        public void ObterPorId_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var userId = "123";
            _mockUserRepository.Setup(repo => repo.GetById(userId))
                              .Throws(new InvalidOperationException("Database connection failed"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _userService.ObterPorId(userId));
            exception.Message.Should().Be("Database connection failed");
            _mockUserRepository.Verify(repo => repo.GetById(userId), Times.Once);
        }

        [Fact]
        public void ObterPorId_WithUserWithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var userId = "user-with-special-chars-123";
            var userWithSpecialChars = new User
            {
                Id = 789,
                Name = "José da Silva Ñoño",
                Email = "josé.ñoño@example.com",
                Role = UserRole.Usuario,
                PasswordHash = "specialhashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.GetById(userId)).Returns(userWithSpecialChars);

            // Act
            var result = _userService.ObterPorId(userId);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("José da Silva Ñoño");
            result.Email.Should().Be("josé.ñoño@example.com");
            _mockUserRepository.Verify(repo => repo.GetById(userId), Times.Once);
        }

        [Fact]
        public void ObterPorId_CalledMultipleTimes_ShouldCallRepositoryEachTime()
        {
            // Arrange
            var userId = "123";
            var user = new User
            {
                Id = 123,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.Usuario
            };

            _mockUserRepository.Setup(repo => repo.GetById(userId)).Returns(user);

            // Act
            var result1 = _userService.ObterPorId(userId);
            var result2 = _userService.ObterPorId(userId);
            var result3 = _userService.ObterPorId(userId);

            // Assert
            result1.Should().Be(user);
            result2.Should().Be(user);
            result3.Should().Be(user);
            _mockUserRepository.Verify(repo => repo.GetById(userId), Times.Exactly(3));
        }

        #endregion

        #region Criar Tests

        [Fact]
        public void Criar_WithValidUser_ShouldCallRepositoryAdd()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "New User",
                Email = "newuser@example.com",
                Role = UserRole.Usuario,
                PasswordHash = "hashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.Add(user)).Verifiable();

            // Act
            _userService.Criar(user);

            // Assert
            _mockUserRepository.Verify(repo => repo.Add(user), Times.Once);
        }

        [Fact]
        public void Criar_WithNullUser_ShouldCallRepositoryWithNull()
        {
            // Arrange
            User nullUser = null!;

            // Act
            _userService.Criar(nullUser);

            // Assert
            _mockUserRepository.Verify(repo => repo.Add(nullUser), Times.Once);
        }

        [Fact]
        public void Criar_WithAdministratorUser_ShouldCallRepositoryAdd()
        {
            // Arrange
            var adminUser = new User
            {
                Id = 2,
                Name = "Admin User",
                Email = "admin@example.com",
                Role = UserRole.Administrador,
                PasswordHash = "adminhashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.Add(adminUser)).Verifiable();

            // Act
            _userService.Criar(adminUser);

            // Assert
            _mockUserRepository.Verify(repo => repo.Add(adminUser), Times.Once);
        }

        [Fact]
        public void Criar_WithUserWithSpecialCharacters_ShouldCallRepositoryAdd()
        {
            // Arrange
            var userWithSpecialChars = new User
            {
                Id = 3,
                Name = "François Müller",
                Email = "françois.müller@example.com",
                Role = UserRole.Usuario,
                PasswordHash = "specialhashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.Add(userWithSpecialChars)).Verifiable();

            // Act
            _userService.Criar(userWithSpecialChars);

            // Assert
            _mockUserRepository.Verify(repo => repo.Add(userWithSpecialChars), Times.Once);
        }

        [Fact]
        public void Criar_WithUserWithLongFields_ShouldCallRepositoryAdd()
        {
            // Arrange
            var userWithLongFields = new User
            {
                Id = 4,
                Name = new string('A', 100),
                Email = $"{new string('b', 50)}@{new string('c', 50)}.com",
                Role = UserRole.Usuario,
                PasswordHash = new string('x', 500)
            };

            _mockUserRepository.Setup(repo => repo.Add(userWithLongFields)).Verifiable();

            // Act
            _userService.Criar(userWithLongFields);

            // Assert
            _mockUserRepository.Verify(repo => repo.Add(userWithLongFields), Times.Once);
        }

        [Fact]
        public void Criar_WithUserWithEmptyFields_ShouldCallRepositoryAdd()
        {
            // Arrange
            var userWithEmptyFields = new User
            {
                Id = 5,
                Name = "",
                Email = "",
                Role = UserRole.Usuario,
                PasswordHash = ""
            };

            _mockUserRepository.Setup(repo => repo.Add(userWithEmptyFields)).Verifiable();

            // Act
            _userService.Criar(userWithEmptyFields);

            // Assert
            _mockUserRepository.Verify(repo => repo.Add(userWithEmptyFields), Times.Once);
        }

        [Fact]
        public void Criar_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var user = new User
            {
                Id = 6,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.Usuario,
                PasswordHash = "hashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.Add(user))
                              .Throws(new InvalidOperationException("Email already exists"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _userService.Criar(user));
            exception.Message.Should().Be("Email already exists");
            _mockUserRepository.Verify(repo => repo.Add(user), Times.Once);
        }

        [Fact]
        public void Criar_WhenRepositoryThrowsArgumentException_ShouldPropagateException()
        {
            // Arrange
            var user = new User
            {
                Id = 7,
                Name = "Test User",
                Email = "invalid-email",
                Role = UserRole.Usuario,
                PasswordHash = "hashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.Add(user))
                              .Throws(new ArgumentException("Invalid email format"));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _userService.Criar(user));
            exception.Message.Should().Be("Invalid email format");
            _mockUserRepository.Verify(repo => repo.Add(user), Times.Once);
        }

        [Fact]
        public void Criar_CalledMultipleTimes_ShouldCallRepositoryEachTime()
        {
            // Arrange
            var user1 = new User { Id = 1, Name = "User 1", Email = "user1@example.com", Role = UserRole.Usuario };
            var user2 = new User { Id = 2, Name = "User 2", Email = "user2@example.com", Role = UserRole.Usuario };
            var user3 = new User { Id = 3, Name = "User 3", Email = "user3@example.com", Role = UserRole.Administrador };

            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).Verifiable();

            // Act
            _userService.Criar(user1);
            _userService.Criar(user2);
            _userService.Criar(user3);

            // Assert
            _mockUserRepository.Verify(repo => repo.Add(user1), Times.Once);
            _mockUserRepository.Verify(repo => repo.Add(user2), Times.Once);
            _mockUserRepository.Verify(repo => repo.Add(user3), Times.Once);
            _mockUserRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Exactly(3));
        }

        [Fact]
        public void Criar_WithSameUserMultipleTimes_ShouldCallRepositoryEachTime()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Repeated User",
                Email = "repeated@example.com",
                Role = UserRole.Usuario,
                PasswordHash = "hashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.Add(user)).Verifiable();

            // Act
            _userService.Criar(user);
            _userService.Criar(user);

            // Assert
            _mockUserRepository.Verify(repo => repo.Add(user), Times.Exactly(2));
        }

        #endregion

        #region Integration-like Tests (Testing service behavior more holistically)

        [Fact]
        public void UserService_ShouldNotPerformAnyValidation_DelegatesAllToRepository()
        {
            // Arrange
            var invalidUser = new User
            {
                Id = -1,
                Name = null!,
                Email = null!,
                Role = (UserRole)999, // Invalid enum value
                PasswordHash = null!
            };

            _mockUserRepository.Setup(repo => repo.Add(invalidUser)).Verifiable();

            // Act
            _userService.Criar(invalidUser);

            // Assert
            // O serviço não faz validação, apenas delega para o repository
            _mockUserRepository.Verify(repo => repo.Add(invalidUser), Times.Once);
        }

        [Fact]
        public void UserService_ShouldPassThroughAllUserProperties()
        {
            // Arrange
            var complexUser = new User
            {
                Id = 12345,
                Name = "Complex User With Many Properties",
                Email = "complex.user+tag@subdomain.example.com",
                Role = UserRole.Administrador,
                PasswordHash = "very-long-complex-hashed-password-with-special-characters-!@#$%^&*()"
            };

            User capturedUser = null!;
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                              .Callback<User>(user => capturedUser = user);

            // Act
            _userService.Criar(complexUser);

            // Assert
            capturedUser.Should().NotBeNull();
            capturedUser.Should().BeSameAs(complexUser); // Mesma referência
            capturedUser.Id.Should().Be(complexUser.Id);
            capturedUser.Name.Should().Be(complexUser.Name);
            capturedUser.Email.Should().Be(complexUser.Email);
            capturedUser.Role.Should().Be(complexUser.Role);
            capturedUser.PasswordHash.Should().Be(complexUser.PasswordHash);
        }

        [Theory]
        [InlineData(UserRole.Usuario)]
        [InlineData(UserRole.Administrador)]
        public void UserService_ShouldHandleAllUserRoles(UserRole role)
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = $"User with {role} role",
                Email = $"user.{role}@example.com",
                Role = role,
                PasswordHash = "hashedpassword"
            };

            _mockUserRepository.Setup(repo => repo.Add(user)).Verifiable();

            // Act
            _userService.Criar(user);

            // Assert
            _mockUserRepository.Verify(repo => repo.Add(user), Times.Once);
        }

        [Fact]
        public void UserService_WithDifferentUserIds_ShouldRetrieveCorrectUsers()
        {
            // Arrange
            var user1 = new User { Id = 1, Name = "User 1", Email = "user1@example.com", Role = UserRole.Usuario };
            var user2 = new User { Id = 2, Name = "User 2", Email = "user2@example.com", Role = UserRole.Administrador };

            _mockUserRepository.Setup(repo => repo.GetById("1")).Returns(user1);
            _mockUserRepository.Setup(repo => repo.GetById("2")).Returns(user2);
            _mockUserRepository.Setup(repo => repo.GetById("3")).Returns((User?)null);

            // Act
            var result1 = _userService.ObterPorId("1");
            var result2 = _userService.ObterPorId("2");
            var result3 = _userService.ObterPorId("3");

            // Assert
            result1.Should().Be(user1);
            result2.Should().Be(user2);
            result3.Should().BeNull();

            _mockUserRepository.Verify(repo => repo.GetById("1"), Times.Once);
            _mockUserRepository.Verify(repo => repo.GetById("2"), Times.Once);
            _mockUserRepository.Verify(repo => repo.GetById("3"), Times.Once);
        }

        #endregion

        #region Performance and Edge Cases

        [Fact]
        public void UserService_WithVeryLargeUserId_ShouldHandleCorrectly()
        {
            // Arrange
            var largeUserId = new string('9', 1000); // Very large user ID
            _mockUserRepository.Setup(repo => repo.GetById(largeUserId)).Returns((User?)null);

            // Act
            var result = _userService.ObterPorId(largeUserId);

            // Assert
            result.Should().BeNull();
            _mockUserRepository.Verify(repo => repo.GetById(largeUserId), Times.Once);
        }

        [Fact]
        public void UserService_WithUserIdContainingSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var specialUserId = "user-id_with.special+chars@domain.com";
            var user = new User
            {
                Id = 999,
                Name = "Special ID User",
                Email = "special@example.com",
                Role = UserRole.Usuario
            };

            _mockUserRepository.Setup(repo => repo.GetById(specialUserId)).Returns(user);

            // Act
            var result = _userService.ObterPorId(specialUserId);

            // Assert
            result.Should().Be(user);
            _mockUserRepository.Verify(repo => repo.GetById(specialUserId), Times.Once);
        }

        #endregion
    }
}
