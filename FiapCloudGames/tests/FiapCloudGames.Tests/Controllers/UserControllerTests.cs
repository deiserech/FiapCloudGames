using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FiapCloudGames.Api.Controllers;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.Domain.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FiapCloudGames.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _userController = new UserController(_mockUserService.Object);
        }

        #region GetUser Tests

        [Fact]
        public void GetUser_WithExistingUser_ShouldReturnOkWithUserData()
        {
            // Arrange
            var userId = "123";
            var expectedUser = new User
            {
                Id = 123,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };

            _mockUserService.Setup(s => s.GetByIdAsync(userId)).Returns(expectedUser);

            // Act
            var result = _userController.GetUser(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            
            var returnedData = okResult!.Value;
            returnedData.Should().NotBeNull();
            
            // Verificar se o objeto anônimo contém as propriedades esperadas
            var userProperties = returnedData!.GetType().GetProperties();
            userProperties.Should().HaveCount(4);
            
            var idProperty = returnedData.GetType().GetProperty("Id");
            idProperty!.GetValue(returnedData).Should().Be(expectedUser.Id);
            
            var nameProperty = returnedData.GetType().GetProperty("Name");
            nameProperty!.GetValue(returnedData).Should().Be(expectedUser.Name);
            
            var emailProperty = returnedData.GetType().GetProperty("Email");
            emailProperty!.GetValue(returnedData).Should().Be(expectedUser.Email);
            
            var roleProperty = returnedData.GetType().GetProperty("Role");
            roleProperty!.GetValue(returnedData).Should().Be(expectedUser.Role.ToString());

            _mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public void GetUser_WithNonExistingUser_ShouldReturnNotFound()
        {
            // Arrange
            var userId = "999";
            _mockUserService.Setup(s => s.GetByIdAsync(userId)).Returns((User?)null);

            // Act
            var result = _userController.GetUser(userId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("abc")]
        public void GetUser_WithVariousIds_ShouldCallService(string userId)
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByIdAsync(userId)).Returns((User?)null);

            // Act
            var result = _userController.GetUser(userId);

            // Assert
            _mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public void GetUser_WithAdministratorUser_ShouldReturnCorrectRole()
        {
            // Arrange
            var userId = "456";
            var adminUser = new User
            {
                Id = 456,
                Name = "Admin User",
                Email = "admin@example.com",
                Role = UserRole.Admin
            };

            _mockUserService.Setup(s => s.GetByIdAsync(userId)).Returns(adminUser);

            // Act
            var result = _userController.GetUser(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedData = okResult!.Value;
            
            var roleProperty = returnedData!.GetType().GetProperty("Role");
            roleProperty!.GetValue(returnedData).Should().Be("Administrador");
        }

        [Fact]
        public void GetUser_WhenServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var userId = "123";
            _mockUserService.Setup(s => s.GetByIdAsync(userId))
                           .Throws(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _userController.GetUser(userId));
            exception.Message.Should().Be("Database error");
        }

        #endregion

        #region GetProfile Tests

        [Fact]
        public void GetProfile_WithAuthenticatedUser_ShouldReturnOkWithUserProfile()
        {
            // Arrange
            var userId = "123";
            var expectedUser = new User
            {
                Id = 123,
                Name = "Authenticated User",
                Email = "auth@example.com",
                Role = UserRole.User
            };

            // Setup claims para simular usuário autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            _mockUserService.Setup(s => s.GetByIdAsync(userId)).Returns(expectedUser);

            // Act
            var result = _userController.GetProfile();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            
            var returnedData = okResult!.Value;
            returnedData.Should().NotBeNull();
            
            var idProperty = returnedData!.GetType().GetProperty("Id");
            idProperty!.GetValue(returnedData).Should().Be(expectedUser.Id);
            
            var nameProperty = returnedData.GetType().GetProperty("Name");
            nameProperty!.GetValue(returnedData).Should().Be(expectedUser.Name);
            
            var emailProperty = returnedData.GetType().GetProperty("Email");
            emailProperty!.GetValue(returnedData).Should().Be(expectedUser.Email);
            
            var roleProperty = returnedData.GetType().GetProperty("Role");
            roleProperty!.GetValue(returnedData).Should().Be(expectedUser.Role.ToString());

            _mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public void GetProfile_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Arrange
            var principal = new ClaimsPrincipal(); // Sem claims

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            // Act
            var result = _userController.GetProfile();

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
            _mockUserService.Verify(s => s.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void GetProfile_WithAuthenticatedUserButUserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var userId = "999";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            _mockUserService.Setup(s => s.GetByIdAsync(userId)).Returns((User?)null);

            // Act
            var result = _userController.GetProfile();

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public void GetProfile_WithEmptyUserIdClaim_ShouldReturnUnauthorized()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "") // Claim vazio
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            // Act
            var result = _userController.GetProfile();

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
            _mockUserService.Verify(s => s.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void GetProfile_WhenServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var userId = "123";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            _mockUserService.Setup(s => s.GetByIdAsync(userId))
                           .Throws(new InvalidOperationException("Database connection failed"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _userController.GetProfile());
            exception.Message.Should().Be("Database connection failed");
        }

        #endregion

        #region CriarUser Tests

        [Fact]
        public void CriarUser_WithValidUser_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "New User",
                Email = "newuser@example.com",
                Role = UserRole.User
            };

            _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).Verifiable();

            // Act
            var result = _userController.CriarUser(user);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.ActionName.Should().Be(nameof(_userController.GetUser));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(user.Id);
            
            var returnedData = createdResult.Value;
            returnedData.Should().NotBeNull();
            
            var idProperty = returnedData!.GetType().GetProperty("Id");
            idProperty!.GetValue(returnedData).Should().Be(user.Id);
            
            var nameProperty = returnedData.GetType().GetProperty("Name");
            nameProperty!.GetValue(returnedData).Should().Be(user.Name);
            
            var emailProperty = returnedData.GetType().GetProperty("Email");
            emailProperty!.GetValue(returnedData).Should().Be(user.Email);
            
            var roleProperty = returnedData.GetType().GetProperty("Role");
            roleProperty!.GetValue(returnedData).Should().Be(user.Role.ToString());

            _mockUserService.Verify(s => s.CreateUserAsync(user), Times.Once);
        }

        [Fact]
        public void CriarUser_WithAdministratorRole_ShouldCreateSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Id = 2,
                Name = "Admin User",
                Email = "admin@example.com",
                Role = UserRole.Admin
            };

            _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).Verifiable();

            // Act
            var result = _userController.CriarUser(user);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            var returnedData = createdResult!.Value;
            
            var roleProperty = returnedData!.GetType().GetProperty("Role");
            roleProperty!.GetValue(returnedData).Should().Be("Administrador");

            _mockUserService.Verify(s => s.CreateUserAsync(user), Times.Once);
        }

        [Fact]
        public void CriarUser_WithNullUser_ShouldReturnBadRequest()
        {
            // Arrange
            User nullUser = null!;

            // Act
            var result = _userController.CriarUser(nullUser);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().Be("User data is required");
            _mockUserService.Verify(s => s.CreateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void CriarUser_WhenServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };

            _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>()))
                           .Throws(new InvalidOperationException("Email already exists"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _userController.CriarUser(user));
            exception.Message.Should().Be("Email already exists");
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidUserService_ShouldCreateInstance()
        {
            // Arrange & Act
            var controller = new UserController(_mockUserService.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullUserService_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new UserController(null!));
            exception.ParamName.Should().Be("service");
        }

        #endregion

        #region Controller Attributes and Configuration Tests

        [Fact]
        public void Controller_ShouldHaveCorrectRouteAndAttributes()
        {
            // Arrange & Act
            var controllerType = typeof(UserController);

            // Assert
            var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);
            apiControllerAttribute.Should().HaveCount(1);

            var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;
            routeAttribute.Should().NotBeNull();
            routeAttribute!.Template.Should().Be("api/[controller]");

            var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false);
            authorizeAttribute.Should().HaveCount(1);
        }

        [Fact]
        public void GetUser_ShouldHaveCorrectAttributes()
        {
            // Arrange
            var method = typeof(UserController).GetMethod(nameof(UserController.GetUser));

            // Assert
            method.Should().NotBeNull();
            
            var httpGetAttribute = method!.GetCustomAttributes(typeof(HttpGetAttribute), false).FirstOrDefault() as HttpGetAttribute;
            httpGetAttribute.Should().NotBeNull();
            httpGetAttribute!.Template.Should().Be("{id}");
        }

        [Fact]
        public void GetProfile_ShouldHaveCorrectAttributes()
        {
            // Arrange
            var method = typeof(UserController).GetMethod(nameof(UserController.GetProfile));

            // Assert
            method.Should().NotBeNull();
            
            var httpGetAttribute = method!.GetCustomAttributes(typeof(HttpGetAttribute), false).FirstOrDefault() as HttpGetAttribute;
            httpGetAttribute.Should().NotBeNull();
            httpGetAttribute!.Template.Should().Be("profile");
        }

        [Fact]
        public void CriarUser_ShouldHaveCorrectAttributes()
        {
            // Arrange
            var method = typeof(UserController).GetMethod(nameof(UserController.CriarUser));

            // Assert
            method.Should().NotBeNull();
            
            var httpPostAttribute = method!.GetCustomAttributes(typeof(HttpPostAttribute), false);
            httpPostAttribute.Should().HaveCount(1);

            var allowAnonymousAttribute = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
            allowAnonymousAttribute.Should().HaveCount(1);
        }

        #endregion

        #region Edge Cases and Complex Scenarios

        [Theory]
        [InlineData("user@domain.com")]
        [InlineData("very.long.email.address@example.domain.com")]
        [InlineData("user+tag@example.com")]
        public void GetUser_WithDifferentUserIdFormats_ShouldHandleCorrectly(string userId)
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByIdAsync(userId)).Returns((User?)null);

            // Act
            var result = _userController.GetUser(userId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public void GetProfile_WithMultipleClaims_ShouldUseCorrectNameIdentifierClaim()
        {
            // Arrange
            var userId = "123";
            var expectedUser = new User
            {
                Id = 123,
                Name = "Multi Claim User",
                Email = "multi@example.com",
                Role = UserRole.User
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "SomeOtherName"),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, "claim@example.com"),
                new Claim(ClaimTypes.Role, "SomeRole")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            _mockUserService.Setup(s => s.GetByIdAsync(userId)).Returns(expectedUser);

            // Act
            var result = _userController.GetProfile();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public void CriarUser_WithUserWithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var user = new User
            {
                Id = 999,
                Name = "José da Silva Ñoño",
                Email = "josé.ñoño@example.com",
                Role = UserRole.User
            };

            _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).Verifiable();

            // Act
            var result = _userController.CriarUser(user);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            var returnedData = createdResult!.Value;
            
            var nameProperty = returnedData!.GetType().GetProperty("Name");
            nameProperty!.GetValue(returnedData).Should().Be(user.Name);
            
            var emailProperty = returnedData.GetType().GetProperty("Email");
            emailProperty!.GetValue(returnedData).Should().Be(user.Email);

            _mockUserService.Verify(s => s.CreateUserAsync(user), Times.Once);
        }

        [Fact]
        public void GetUser_ResponseShouldNotContainPasswordHash()
        {
            // Arrange
            var userId = "123";
            var user = new User
            {
                Id = 123,
                Name = "Secure User",
                Email = "secure@example.com",
                Role = UserRole.User,
                PasswordHash = "super-secret-hash"
            };

            _mockUserService.Setup(s => s.GetByIdAsync(userId)).Returns(user);

            // Act
            var result = _userController.GetUser(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedData = okResult!.Value;
            
            // Verificar que PasswordHash não está na resposta
            var properties = returnedData!.GetType().GetProperties();
            properties.Should().NotContain(p => p.Name == "PasswordHash");
            properties.Should().HaveCount(4); // Id, Name, Email, Role
        }

        [Fact]
        public void GetProfile_ResponseShouldNotContainPasswordHash()
        {
            // Arrange
            var userId = "123";
            var user = new User
            {
                Id = 123,
                Name = "Secure Profile User",
                Email = "profile@example.com",
                Role = UserRole.Admin,
                PasswordHash = "another-secret-hash"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            _mockUserService.Setup(s => s.GetByIdAsync(userId)).Returns(user);

            // Act
            var result = _userController.GetProfile();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedData = okResult!.Value;
            
            // Verificar que PasswordHash não está na resposta
            var properties = returnedData!.GetType().GetProperties();
            properties.Should().NotContain(p => p.Name == "PasswordHash");
            properties.Should().HaveCount(4); // Id, Name, Email, Role
        }

        #endregion
    }
}
