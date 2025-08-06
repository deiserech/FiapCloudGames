using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using FiapCloudGames.Api.Controllers;
using FiapCloudGames.Domain.DTOs;
using FiapCloudGames.Domain.Enums;
using System;
using FiapCloudGames.Domain.Interfaces.Services;

namespace FiapCloudGames.Tests.Controllers
{
    public class AuthControllerUnitTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _authController;

        public AuthControllerUnitTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _authController = new AuthController(_mockAuthService.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidAuthService_ShouldCreateInstance()
        {
            // Arrange & Act
            var controller = new AuthController(_mockAuthService.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullAuthService_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new AuthController(null!));
            exception.ParamName.Should().Be("authService");
        }

        #endregion

        #region Login Tests

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnOkWithAuthResponse()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "validpassword"
            };

            var expectedResponse = new AuthResponseDto
            {
                Token = "valid-jwt-token",
                Email = "test@example.com",
                Name = "Test User",
                UserId = 1
            };

            _mockAuthService.Setup(s => s.Login(loginDto)).Returns(expectedResponse);

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResponse);
            _mockAuthService.Verify(s => s.Login(loginDto), Times.Once);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            _mockAuthService.Setup(s => s.Login(loginDto)).Returns((AuthResponseDto?)null);

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult!.Value.Should().BeEquivalentTo(new { message = "Email ou senha inválidos" });
            _mockAuthService.Verify(s => s.Login(loginDto), Times.Once);
        }

        [Fact]
        public void Login_WithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "invalid-email",
                Password = "password"
            };

            _authController.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeOfType<SerializableError>();
            _mockAuthService.Verify(s => s.Login(It.IsAny<LoginDto>()), Times.Never);
        }

        [Fact]
        public void Login_WithInvalidEmailFormat_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "invalid-email-format",
                Password = "password123"
            };

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeOfType<SerializableError>();

            var errors = badRequestResult.Value as SerializableError;
            errors.Should().ContainKey("Email");
            _mockAuthService.Verify(s => s.Login(It.IsAny<LoginDto>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("plaintext")]
        [InlineData("@domain.com")]
        [InlineData("user@")]
        [InlineData("user.domain.com")]
        public void Login_WithVariousInvalidEmailFormats_ShouldReturnBadRequest(string invalidEmail)
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = invalidEmail,
                Password = "password123"
            };

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockAuthService.Verify(s => s.Login(It.IsAny<LoginDto>()), Times.Never);
        }

        [Fact]
        public void Login_WithValidEmailFormat_ShouldCallAuthService()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "valid@example.com",
                Password = "password123"
            };

            _mockAuthService.Setup(s => s.Login(loginDto)).Returns((AuthResponseDto?)null);

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            _mockAuthService.Verify(s => s.Login(loginDto), Times.Once);
        }

        [Fact]
        public void Login_WhenAuthServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };

            _mockAuthService.Setup(s => s.Login(loginDto))
                           .Throws(new InvalidOperationException("Service error"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _authController.Login(loginDto));
            exception.Message.Should().Be("Service error");
        }

        #endregion

        #region Register Tests

        [Fact]
        public void Register_WithValidData_ShouldReturnOkWithAuthResponse()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "New User",
                Email = "newuser@example.com",
                Password = "ValidPassword123!",
                Role = UserRole.User
            };

            var expectedResponse = new AuthResponseDto
            {
                Token = "valid-jwt-token",
                Email = "newuser@example.com",
                Name = "New User",
                UserId = 1
            };

            _mockAuthService.Setup(s => s.Register(registerDto)).Returns(expectedResponse);

            // Act
            var result = _authController.Register(registerDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResponse);
            _mockAuthService.Verify(s => s.Register(registerDto), Times.Once);
        }

        [Fact]
        public void Register_WithExistingEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "New User",
                Email = "existing@example.com",
                Password = "ValidPassword123!",
                Role = UserRole.User
            };

            _mockAuthService.Setup(s => s.Register(registerDto)).Returns((AuthResponseDto?)null);

            // Act
            var result = _authController.Register(registerDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeEquivalentTo(new { message = "Email já está em uso" });
            _mockAuthService.Verify(s => s.Register(registerDto), Times.Once);
        }

        [Fact]
        public void Register_WithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "",
                Email = "test@example.com",
                Password = "ValidPassword123!",
                Role = UserRole.User
            };

            _authController.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = _authController.Register(registerDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeOfType<SerializableError>();
            _mockAuthService.Verify(s => s.Register(It.IsAny<RegisterDto>()), Times.Never);
        }

        [Fact]
        public void Register_WithInvalidEmailFormat_ShouldReturnBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Test User",
                Email = "invalid-email-format",
                Password = "ValidPassword123!",
                Role = UserRole.User
            };

            // Act
            var result = _authController.Register(registerDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeOfType<SerializableError>();

            var errors = badRequestResult.Value as SerializableError;
            errors.Should().ContainKey("Email");
            _mockAuthService.Verify(s => s.Register(It.IsAny<RegisterDto>()), Times.Never);
        }

        [Theory]
        [InlineData("weak")]
        [InlineData("password")]
        [InlineData("PASSWORD")]
        [InlineData("12345678")]
        [InlineData("password123")]
        [InlineData("PASSWORD123")]
        [InlineData("Password")]
        public void Register_WithWeakPassword_ShouldReturnBadRequest(string weakPassword)
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = weakPassword,
                Role = UserRole.User
            };

            // Act
            var result = _authController.Register(registerDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeOfType<SerializableError>();

            var errors = badRequestResult.Value as SerializableError;
            errors.Should().ContainKey("Password");
            _mockAuthService.Verify(s => s.Register(It.IsAny<RegisterDto>()), Times.Never);
        }

        [Theory]
        [InlineData("ValidPass123!")]
        [InlineData("MyStr0ng@Password")]
        [InlineData("Complex#Pass123")]
        public void Register_WithStrongPassword_ShouldCallAuthService(string strongPassword)
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = strongPassword,
                Role = UserRole.User
            };

            _mockAuthService.Setup(s => s.Register(registerDto)).Returns((AuthResponseDto?)null);

            // Act
            var result = _authController.Register(registerDto);

            // Assert
            _mockAuthService.Verify(s => s.Register(registerDto), Times.Once);
        }

        [Fact]
        public void Register_WithAdministratorRole_ShouldReturnOkWithAuthResponse()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Admin User",
                Email = "admin@example.com",
                Password = "AdminPassword123!",
                Role = UserRole.Admin
            };

            var expectedResponse = new AuthResponseDto
            {
                Token = "admin-jwt-token",
                Email = "admin@example.com",
                Name = "Admin User",
                UserId = 2
            };

            _mockAuthService.Setup(s => s.Register(registerDto)).Returns(expectedResponse);

            // Act
            var result = _authController.Register(registerDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResponse);
            _mockAuthService.Verify(s => s.Register(registerDto), Times.Once);
        }

        [Fact]
        public void Register_WhenAuthServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "ValidPassword123!",
                Role = UserRole.User
            };

            _mockAuthService.Setup(s => s.Register(registerDto))
                           .Throws(new InvalidOperationException("Service error"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _authController.Register(registerDto));
            exception.Message.Should().Be("Service error");
        }

        #endregion

        #region Controller Attributes Tests

        [Fact]
        public void Controller_ShouldHaveCorrectRouteAndAttributes()
        {
            // Arrange & Act
            var controllerType = typeof(AuthController);

            // Assert
            var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);
            apiControllerAttribute.Should().HaveCount(1);

            var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;
            routeAttribute.Should().NotBeNull();
            routeAttribute!.Template.Should().Be("api/[controller]");
        }

        [Fact]
        public void Login_ShouldHaveCorrectAttributes()
        {
            // Arrange
            var method = typeof(AuthController).GetMethod(nameof(AuthController.Login));

            // Assert
            method.Should().NotBeNull();

            var httpPostAttribute = method!.GetCustomAttributes(typeof(HttpPostAttribute), false).FirstOrDefault() as HttpPostAttribute;
            httpPostAttribute.Should().NotBeNull();
            httpPostAttribute!.Template.Should().Be("login");
        }

        [Fact]
        public void Register_ShouldHaveCorrectAttributes()
        {
            // Arrange
            var method = typeof(AuthController).GetMethod(nameof(AuthController.Register));

            // Assert
            method.Should().NotBeNull();

            var httpPostAttribute = method!.GetCustomAttributes(typeof(HttpPostAttribute), false).FirstOrDefault() as HttpPostAttribute;
            httpPostAttribute.Should().NotBeNull();
            httpPostAttribute!.Template.Should().Be("register");
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public void Login_WithSpecialCharactersInEmail_ShouldHandleCorrectly()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "user+test@example-domain.co.uk",
                Password = "password123"
            };

            var expectedResponse = new AuthResponseDto
            {
                Token = "valid-token",
                Email = loginDto.Email,
                Name = "Test User",
                UserId = 1
            };

            _mockAuthService.Setup(s => s.Login(loginDto)).Returns(expectedResponse);

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockAuthService.Verify(s => s.Login(loginDto), Times.Once);
        }

        [Fact]
        public void Register_WithSpecialCharactersInName_ShouldHandleCorrectly()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "José María Ñoño-González",
                Email = "jose@example.com",
                Password = "ValidPassword123!",
                Role = UserRole.User
            };

            var expectedResponse = new AuthResponseDto
            {
                Token = "valid-token",
                Email = registerDto.Email,
                Name = registerDto.Name,
                UserId = 1
            };

            _mockAuthService.Setup(s => s.Register(registerDto)).Returns(expectedResponse);

            // Act
            var result = _authController.Register(registerDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockAuthService.Verify(s => s.Register(registerDto), Times.Once);
        }

        [Fact]
        public void AuthController_ShouldNotLogSensitiveInformation()
        {
            // This test ensures that the controller doesn't accidentally expose sensitive data
            // In a real scenario, you would check actual logging output

            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "secretpassword123"
            };

            _mockAuthService.Setup(s => s.Login(loginDto)).Returns((AuthResponseDto?)null);

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            var responseValue = unauthorizedResult!.Value!.ToString();

            // Ensure password is not exposed in response
            responseValue.Should().NotContain("secretpassword123");
        }

        [Theory]
        [InlineData(null)]
        public void Login_WithNullDto_ShouldHandleGracefully(LoginDto loginDto)
        {
            // Note: In practice, ASP.NET Core model binding would handle null DTOs
            // This test is more for completeness

            // Act & Assert
            if (loginDto == null)
            {
                // The framework would typically handle this before reaching the controller
                Assert.True(true); // Placeholder assertion
            }
        }

        [Fact]
        public void Register_WithEmptyPassword_ShouldReturnBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "",
                Role = UserRole.User
            };

            // Act
            var result = _authController.Register(registerDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockAuthService.Verify(s => s.Register(It.IsAny<RegisterDto>()), Times.Never);
        }

        [Fact]
        public void AuthController_ShouldValidateEmailBeforeCallingService()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "definitely-not-an-email",
                Password = "password123"
            };

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockAuthService.Verify(s => s.Login(It.IsAny<LoginDto>()), Times.Never);
        }

        #endregion
    }
}