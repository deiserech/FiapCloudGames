using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.DTOs;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using FiapCloudGames.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;

namespace FiapCloudGames.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Setup default JWT configuration
            SetupJwtConfiguration();
            
            _authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);
        }

        private void SetupJwtConfiguration()
        {
            var jwtSettingsSection = new Mock<IConfigurationSection>();
            jwtSettingsSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
            jwtSettingsSection.Setup(x => x["Audience"]).Returns("TestAudience");
            jwtSettingsSection.Setup(x => x["ExpiryInMinutes"]).Returns("60");

            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSettingsSection.Object);
            _mockConfiguration.Setup(x => x["JwtSettings:SecretKey"]).Returns("this-is-a-very-long-secret-key-that-is-at-least-32-characters-long");
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDependencies_ShouldCreateInstance()
        {
            // Arrange & Act
            var service = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion

        #region Login Tests

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnAuthResponse()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "validpassword"
            };

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };
            user.SetPassword("validpassword");

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(user);

            // Act
            var result = _authService.Login(loginDto);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(user.Email);
            result.Name.Should().Be(user.Name);
            result.UserId.Should().Be(user.Id);
            result.Token.Should().NotBeNullOrEmpty();

            // Verify JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.CanReadToken(result.Token).Should().BeTrue();

            _mockUserRepository.Verify(repo => repo.GetByEmail(loginDto.Email), Times.Once);
        }

        [Fact]
        public void Login_WithNonExistentEmail_ShouldReturnNull()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "password"
            };

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns((User?)null);

            // Act
            var result = _authService.Login(loginDto);

            // Assert
            result.Should().BeNull();
            _mockUserRepository.Verify(repo => repo.GetByEmail(loginDto.Email), Times.Once);
        }

        [Fact]
        public void Login_WithInvalidPassword_ShouldReturnNull()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };
            user.SetPassword("correctpassword");

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(user);

            // Act
            var result = _authService.Login(loginDto);

            // Assert
            result.Should().BeNull();
            _mockUserRepository.Verify(repo => repo.GetByEmail(loginDto.Email), Times.Once);
        }

        [Fact]
        public void Login_WithAdministratorUser_ShouldReturnValidAuthResponse()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@example.com",
                Password = "adminpassword"
            };

            var adminUser = new User
            {
                Id = 2,
                Name = "Admin User",
                Email = "admin@example.com",
                Role = UserRole.Admin
            };
            adminUser.SetPassword("adminpassword");

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(adminUser);

            // Act
            var result = _authService.Login(loginDto);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(adminUser.Email);
            result.Name.Should().Be(adminUser.Name);
            result.UserId.Should().Be(adminUser.Id);
            result.Token.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Login_WithEmptyOrWhitespacePassword_ShouldReturnNull(string password)
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = password
            };

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };
            user.SetPassword("validpassword");

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(user);

            // Act
            var result = _authService.Login(loginDto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Login_WithNullLoginDto_ShouldThrowException()
        {
            // Arrange
            LoginDto nullLoginDto = null!;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _authService.Login(nullLoginDto));
        }

        [Fact]
        public void Login_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email))
                              .Throws(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _authService.Login(loginDto));
            exception.Message.Should().Be("Database error");
        }

        #endregion

        #region Register Tests

        [Fact]
        public void Register_WithValidData_ShouldReturnAuthResponse()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "New User",
                Email = "newuser@example.com",
                Password = "newpassword123",
                Role = UserRole.User
            };

            _mockUserRepository.Setup(repo => repo.EmailExists(registerDto.Email)).Returns(false);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).Verifiable();

            // Act
            var result = _authService.Register(registerDto);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(registerDto.Email);
            result.Name.Should().Be(registerDto.Name);
            result.UserId.Should().Be(0); // Default value since user is new
            result.Token.Should().NotBeNullOrEmpty();

            // Verify JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.CanReadToken(result.Token).Should().BeTrue();

            _mockUserRepository.Verify(repo => repo.EmailExists(registerDto.Email), Times.Once);
            _mockUserRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void Register_WithExistingEmail_ShouldReturnNull()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "New User",
                Email = "existing@example.com",
                Password = "password123",
                Role = UserRole.User
            };

            _mockUserRepository.Setup(repo => repo.EmailExists(registerDto.Email)).Returns(true);

            // Act
            var result = _authService.Register(registerDto);

            // Assert
            result.Should().BeNull();
            _mockUserRepository.Verify(repo => repo.EmailExists(registerDto.Email), Times.Once);
            _mockUserRepository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void Register_WithAdministratorRole_ShouldCreateAdminUser()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Admin User",
                Email = "admin@example.com",
                Password = "adminpassword123",
                Role = UserRole.Admin
            };

            User capturedUser = null!;
            _mockUserRepository.Setup(repo => repo.EmailExists(registerDto.Email)).Returns(false);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                              .Callback<User>(user => capturedUser = user);

            // Act
            var result = _authService.Register(registerDto);

            // Assert
            result.Should().NotBeNull();
            capturedUser.Should().NotBeNull();
            capturedUser.Role.Should().Be(UserRole.Admin);
            capturedUser.Name.Should().Be(registerDto.Name);
            capturedUser.Email.Should().Be(registerDto.Email);
            capturedUser.PasswordHash.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Register_ShouldHashPassword()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "plaintextpassword",
                Role = UserRole.User
            };

            User capturedUser = null!;
            _mockUserRepository.Setup(repo => repo.EmailExists(registerDto.Email)).Returns(false);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                              .Callback<User>(user => capturedUser = user);

            // Act
            var result = _authService.Register(registerDto);

            // Assert
            result.Should().NotBeNull();
            capturedUser.Should().NotBeNull();
            capturedUser.PasswordHash.Should().NotBe(registerDto.Password);
            capturedUser.PasswordHash.Should().NotBeNullOrEmpty();
            capturedUser.VerifyPassword(registerDto.Password).Should().BeTrue();
        }

        [Fact]
        public void Register_WithSpecialCharactersInData_ShouldHandleCorrectly()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "José da Silva Ñoño",
                Email = "josé.ñoño@example.com",
                Password = "password123!@#",
                Role = UserRole.User
            };

            _mockUserRepository.Setup(repo => repo.EmailExists(registerDto.Email)).Returns(false);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).Verifiable();

            // Act
            var result = _authService.Register(registerDto);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(registerDto.Name);
            result.Email.Should().Be(registerDto.Email);
        }

        [Fact]
        public void Register_WithNullRegisterDto_ShouldThrowException()
        {
            // Arrange
            RegisterDto nullRegisterDto = null!;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _authService.Register(nullRegisterDto));
        }

        [Fact]
        public void Register_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "password123",
                Role = UserRole.User
            };

            _mockUserRepository.Setup(repo => repo.EmailExists(registerDto.Email))
                              .Throws(new InvalidOperationException("Database connection failed"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _authService.Register(registerDto));
            exception.Message.Should().Be("Database connection failed");
        }

        [Fact]
        public void Register_WhenAddThrowsException_ShouldPropagateException()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "password123",
                Role = UserRole.User
            };

            _mockUserRepository.Setup(repo => repo.EmailExists(registerDto.Email)).Returns(false);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                              .Throws(new InvalidOperationException("Database insert failed"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _authService.Register(registerDto));
            exception.Message.Should().Be("Database insert failed");
        }

        #endregion

        #region JWT Token Generation Tests

        [Fact]
        public void GenerateJwtToken_WithValidUser_ShouldCreateValidToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new User
            {
                Id = 123,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };
            user.SetPassword("password");

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(user);

            // Act
            var result = _authService.Login(loginDto);

            // Assert
            result.Should().NotBeNull();
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(result!.Token);

            token.Should().NotBeNull();
            token.Issuer.Should().Be("TestIssuer");
            token.Audiences.Should().Contain("TestAudience");
            
            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            userIdClaim.Should().NotBeNull();
            userIdClaim!.Value.Should().Be(user.Id.ToString());
            
            var emailClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            emailClaim.Should().NotBeNull();
            emailClaim!.Value.Should().Be(user.Email);
            
            var nameClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            nameClaim.Should().NotBeNull();
            nameClaim!.Value.Should().Be(user.Name);
        }

        [Fact]
        public void GenerateJwtToken_WithMissingSecretKey_ShouldThrowException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["JwtSettings:SecretKey"]).Returns((string?)null);
            
            var authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);
            
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };
            user.SetPassword("password");

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(user);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => authService.Login(loginDto));
            exception.Message.Should().Contain("JWT SecretKey is missing or too short");
        }

        [Fact]
        public void GenerateJwtToken_WithShortSecretKey_ShouldThrowException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["JwtSettings:SecretKey"]).Returns("short-key");
            
            var authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);
            
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };
            user.SetPassword("password");

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(user);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => authService.Login(loginDto));
            exception.Message.Should().Contain("JWT SecretKey is missing or too short");
        }

        [Fact]
        public void GenerateJwtToken_WithCustomExpiryTime_ShouldSetCorrectExpiration()
        {
            // Arrange
            var jwtSettingsSection = new Mock<IConfigurationSection>();
            jwtSettingsSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
            jwtSettingsSection.Setup(x => x["Audience"]).Returns("TestAudience");
            jwtSettingsSection.Setup(x => x["ExpiryInMinutes"]).Returns("120"); // 2 hours

            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSettingsSection.Object);
            _mockConfiguration.Setup(x => x["JwtSettings:SecretKey"]).Returns("this-is-a-very-long-secret-key-that-is-at-least-32-characters-long");

            var authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };
            user.SetPassword("password");

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(user);

            // Act
            var result = authService.Login(loginDto);

            // Assert
            result.Should().NotBeNull();
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(result!.Token);

            var expirationTime = token.ValidTo;
            var expectedExpiration = DateTime.UtcNow.AddMinutes(120);
            
            // Allow for a few seconds of tolerance due to execution time
            expirationTime.Should().BeCloseTo(expectedExpiration, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void GenerateJwtToken_WithMissingExpiryConfiguration_ShouldUseDefaultExpiry()
        {
            // Arrange
            var jwtSettingsSection = new Mock<IConfigurationSection>();
            jwtSettingsSection.Setup(x => x["Issuer"]).Returns("TestIssuer");
            jwtSettingsSection.Setup(x => x["Audience"]).Returns("TestAudience");
            jwtSettingsSection.Setup(x => x["ExpiryInMinutes"]).Returns((string?)null); // Missing expiry

            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSettingsSection.Object);
            _mockConfiguration.Setup(x => x["JwtSettings:SecretKey"]).Returns("this-is-a-very-long-secret-key-that-is-at-least-32-characters-long");

            var authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };
            user.SetPassword("password");

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(user);

            // Act
            var result = authService.Login(loginDto);

            // Assert
            result.Should().NotBeNull();
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(result!.Token);

            var expirationTime = token.ValidTo;
            var expectedExpiration = DateTime.UtcNow.AddMinutes(60); // Default 60 minutes
            
            expirationTime.Should().BeCloseTo(expectedExpiration, TimeSpan.FromSeconds(10));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Login_And_Register_ShouldProduceSimilarTokenStructure()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Integration User",
                Email = "integration@example.com",
                Password = "integrationpassword123",
                Role = UserRole.User
            };

            var loginDto = new LoginDto
            {
                Email = "integration@example.com",
                Password = "integrationpassword123"
            };

            _mockUserRepository.Setup(repo => repo.EmailExists(registerDto.Email)).Returns(false);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).Verifiable();

            User registeredUser = null!;
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                              .Callback<User>(user => registeredUser = user);

            // Act
            var registerResult = _authService.Register(registerDto);
            
            // Simulate that the user was saved and now exists for login
            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(registeredUser);
            var loginResult = _authService.Login(loginDto);

            // Assert
            registerResult.Should().NotBeNull();
            loginResult.Should().NotBeNull();

            var registerTokenHandler = new JwtSecurityTokenHandler();
            var loginTokenHandler = new JwtSecurityTokenHandler();
            
            var registerToken = registerTokenHandler.ReadJwtToken(registerResult!.Token);
            var loginToken = loginTokenHandler.ReadJwtToken(loginResult!.Token);

            // Both tokens should have the same structure
            registerToken.Issuer.Should().Be(loginToken.Issuer);
            registerToken.Audiences.Should().BeEquivalentTo(loginToken.Audiences);
            
            var registerClaims = registerToken.Claims.Select(c => c.Type).ToList();
            var loginClaims = loginToken.Claims.Select(c => c.Type).ToList();
            
            registerClaims.Should().BeEquivalentTo(loginClaims);
        }

        [Theory]
        [InlineData(UserRole.User)]
        [InlineData(UserRole.Admin)]
        public void AuthService_ShouldHandleAllUserRoles(UserRole role)
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = $"{role} User",
                Email = $"{role.ToString().ToLower()}@example.com",
                Password = "password123",
                Role = role
            };

            _mockUserRepository.Setup(repo => repo.EmailExists(registerDto.Email)).Returns(false);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).Verifiable();

            // Act
            var result = _authService.Register(registerDto);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(result.Token);
            token.Should().NotBeNull();
        }

        #endregion

        #region Edge Cases and Security Tests

        [Fact]
        public void Login_WithCaseDifferentEmail_ShouldDependOnRepository()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "TEST@EXAMPLE.COM",
                Password = "password"
            };

            // The service relies on repository behavior for case sensitivity
            _mockUserRepository.Setup(repo => repo.GetByEmail("TEST@EXAMPLE.COM")).Returns((User?)null);

            // Act
            var result = _authService.Login(loginDto);

            // Assert
            result.Should().BeNull();
            _mockUserRepository.Verify(repo => repo.GetByEmail("TEST@EXAMPLE.COM"), Times.Once);
        }

        [Fact]
        public void Register_WithLongPassword_ShouldHandleCorrectly()
        {
            // Arrange
            var longPassword = new string('a', 100);
            var registerDto = new RegisterDto
            {
                Name = "Long Password User",
                Email = "longpassword@example.com",
                Password = longPassword,
                Role = UserRole.User
            };

            _mockUserRepository.Setup(repo => repo.EmailExists(registerDto.Email)).Returns(false);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).Verifiable();

            // Act
            var result = _authService.Register(registerDto);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void AuthService_ShouldNotExposePasswordsInTokens()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "security@example.com",
                Password = "secretpassword123"
            };

            var user = new User
            {
                Id = 1,
                Name = "Security User",
                Email = "security@example.com",
                Role = UserRole.User
            };
            user.SetPassword("secretpassword123");

            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email)).Returns(user);

            // Act
            var result = _authService.Login(loginDto);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotContain("secretpassword123");
            result.Token.Should().NotContain(user.PasswordHash);
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(result.Token);
            
            var allClaimValues = token.Claims.Select(c => c.Value).ToList();
            allClaimValues.Should().NotContain("secretpassword123");
            allClaimValues.Should().NotContain(user.PasswordHash);
        }

        #endregion
    }
}
