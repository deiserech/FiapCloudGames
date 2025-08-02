using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Api.Controllers;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FiapCloudGames.Tests.Controllers
{
    public class GameControllerUnitTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly GameController _gameController;

        public GameControllerUnitTests()
        {
            _mockGameService = new Mock<IGameService>();
            _gameController = new GameController(_mockGameService.Object);
        }

        #region Cadastrar Tests

        [Fact]
        public void Cadastrar_WithValidGame_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Title = "Test Game",
                Description = "A test game description",
                Price = 59.99m,
                ReleaseDate = DateTime.Now
            };

            _mockGameService.Setup(s => s.Cadastrar(It.IsAny<Game>())).Verifiable();

            // Act
            var result = _gameController.Cadastrar(game);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.ActionName.Should().Be(nameof(_gameController.ObterPorId));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(game.Id);
            createdResult.Value.Should().Be(game);
            _mockGameService.Verify(s => s.Cadastrar(game), Times.Once);
        }

        [Fact]
        public void Cadastrar_WithInvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var game = new Game();
            _gameController.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = _gameController.Cadastrar(game);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().BeOfType<SerializableError>();
            _mockGameService.Verify(s => s.Cadastrar(It.IsAny<Game>()), Times.Never);
        }

        [Fact]
        public void Cadastrar_WithNullGame_ShouldReturnBadRequest()
        {
            // Arrange
            Game nullGame = null!;

            // Act
            var result = _gameController.Cadastrar(nullGame);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.Value.Should().Be("Game data is required");
            _mockGameService.Verify(s => s.Cadastrar(It.IsAny<Game>()), Times.Never);
        }

        [Fact]
        public void Cadastrar_WhenServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Title = "Test Game",
                Description = "A test game description",
                Price = 59.99m,
                ReleaseDate = DateTime.Now
            };

            _mockGameService.Setup(s => s.Cadastrar(It.IsAny<Game>()))
                           .Throws(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _gameController.Cadastrar(game));
            exception.Message.Should().Be("Database error");
        }

        #endregion

        #region ObterPorId Tests

        [Fact]
        public void ObterPorId_WithExistingGame_ShouldReturnOkWithGame()
        {
            // Arrange
            var gameId = 1;
            var expectedGame = new Game
            {
                Id = gameId,
                Title = "Test Game",
                Description = "A test game description",
                Price = 59.99m,
                ReleaseDate = DateTime.Now
            };

            _mockGameService.Setup(s => s.ObterPorId(gameId)).Returns(expectedGame);

            // Act
            var result = _gameController.ObterPorId(gameId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedGame);
            _mockGameService.Verify(s => s.ObterPorId(gameId), Times.Once);
        }

        [Fact]
        public void ObterPorId_WithNonExistingGame_ShouldReturnNotFound()
        {
            // Arrange
            var gameId = 999;
            _mockGameService.Setup(s => s.ObterPorId(gameId)).Returns((Game?)null);

            // Act
            var result = _gameController.ObterPorId(gameId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockGameService.Verify(s => s.ObterPorId(gameId), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ObterPorId_WithInvalidId_ShouldCallServiceAndReturnNotFound(int invalidId)
        {
            // Arrange
            _mockGameService.Setup(s => s.ObterPorId(invalidId)).Returns((Game?)null);

            // Act
            var result = _gameController.ObterPorId(invalidId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockGameService.Verify(s => s.ObterPorId(invalidId), Times.Once);
        }

        [Fact]
        public void ObterPorId_WhenServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var gameId = 1;
            _mockGameService.Setup(s => s.ObterPorId(gameId))
                           .Throws(new InvalidOperationException("Database connection failed"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _gameController.ObterPorId(gameId));
            exception.Message.Should().Be("Database connection failed");
        }

        #endregion

        #region ListarTodos Tests

        [Fact]
        public void ListarTodos_WithGamesInDatabase_ShouldReturnOkWithGamesList()
        {
            // Arrange
            var expectedGames = new List<Game>
            {
                new Game { Id = 1, Title = "Game 1", Description = "Description 1", Price = 29.99m, ReleaseDate = DateTime.Now },
                new Game { Id = 2, Title = "Game 2", Description = "Description 2", Price = 39.99m, ReleaseDate = DateTime.Now },
                new Game { Id = 3, Title = "Game 3", Description = "Description 3", Price = 49.99m, ReleaseDate = DateTime.Now }
            };

            _mockGameService.Setup(s => s.ListarTodos()).Returns(expectedGames);

            // Act
            var result = _gameController.ListarTodos();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedGames);
            _mockGameService.Verify(s => s.ListarTodos(), Times.Once);
        }

        [Fact]
        public void ListarTodos_WithEmptyDatabase_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            var emptyGamesList = new List<Game>();
            _mockGameService.Setup(s => s.ListarTodos()).Returns(emptyGamesList);

            // Act
            var result = _gameController.ListarTodos();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(emptyGamesList);
            _mockGameService.Verify(s => s.ListarTodos(), Times.Once);
        }

        [Fact]
        public void ListarTodos_WithSingleGame_ShouldReturnOkWithSingleGameList()
        {
            // Arrange
            var singleGame = new List<Game>
            {
                new Game { Id = 1, Title = "Only Game", Description = "The only game", Price = 99.99m, ReleaseDate = DateTime.Now }
            };

            _mockGameService.Setup(s => s.ListarTodos()).Returns(singleGame);

            // Act
            var result = _gameController.ListarTodos();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(singleGame);
            var returnedGames = okResult.Value as IEnumerable<Game>;
            returnedGames.Should().HaveCount(1);
            _mockGameService.Verify(s => s.ListarTodos(), Times.Once);
        }

        [Fact]
        public void ListarTodos_WhenServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            _mockGameService.Setup(s => s.ListarTodos())
                           .Throws(new InvalidOperationException("Database unavailable"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _gameController.ListarTodos());
            exception.Message.Should().Be("Database unavailable");
        }

        [Fact]
        public void ListarTodos_WhenServiceReturnsNull_ShouldReturnOkWithNull()
        {
            // Arrange
            _mockGameService.Setup(s => s.ListarTodos()).Returns((IEnumerable<Game>?)null!);

            // Act
            var result = _gameController.ListarTodos();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeNull();
            _mockGameService.Verify(s => s.ListarTodos(), Times.Once);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidGameService_ShouldCreateInstance()
        {
            // Arrange & Act
            var controller = new GameController(_mockGameService.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullGameService_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new GameController(null!));
            exception.ParamName.Should().Be("service");
        }

        #endregion

        #region Integration-like Tests (Testing controller behavior more holistically)

        [Fact]
        public void Controller_ShouldHaveCorrectRouteAndAttributes()
        {
            // Arrange & Act
            var controllerType = typeof(GameController);

            // Assert
            var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);
            apiControllerAttribute.Should().HaveCount(1);

            var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;
            routeAttribute.Should().NotBeNull();
            routeAttribute!.Template.Should().Be("api/[controller]");
        }

        [Fact]
        public void Cadastrar_ShouldHaveCorrectAttributes()
        {
            // Arrange
            var method = typeof(GameController).GetMethod(nameof(GameController.Cadastrar));

            // Assert
            method.Should().NotBeNull();
            
            var httpPostAttribute = method!.GetCustomAttributes(typeof(HttpPostAttribute), false);
            httpPostAttribute.Should().HaveCount(1);

            var authorizeAttribute = method.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false).FirstOrDefault()
                as Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
            authorizeAttribute.Should().NotBeNull();
            authorizeAttribute!.Roles.Should().Be("Administrador");
        }

        [Fact]
        public void ObterPorId_ShouldHaveCorrectAttributes()
        {
            // Arrange
            var method = typeof(GameController).GetMethod(nameof(GameController.ObterPorId));

            // Assert
            method.Should().NotBeNull();
            
            var httpGetAttribute = method!.GetCustomAttributes(typeof(HttpGetAttribute), false).FirstOrDefault() as HttpGetAttribute;
            httpGetAttribute.Should().NotBeNull();
            httpGetAttribute!.Template.Should().Be("{id}");

            var allowAnonymousAttribute = method.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute), false);
            allowAnonymousAttribute.Should().HaveCount(1);
        }

        [Fact]
        public void ListarTodos_ShouldHaveCorrectAttributes()
        {
            // Arrange
            var method = typeof(GameController).GetMethod(nameof(GameController.ListarTodos));

            // Assert
            method.Should().NotBeNull();
            
            var httpGetAttribute = method!.GetCustomAttributes(typeof(HttpGetAttribute), false);
            httpGetAttribute.Should().HaveCount(1);

            var allowAnonymousAttribute = method.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute), false);
            allowAnonymousAttribute.Should().HaveCount(1);
        }

        #endregion

        #region Edge Cases and Error Scenarios

        [Fact]
        public void Cadastrar_WithComplexValidationErrors_ShouldReturnBadRequestWithAllErrors()
        {
            // Arrange
            var game = new Game();
            _gameController.ModelState.AddModelError("Title", "Title is required");
            _gameController.ModelState.AddModelError("Price", "Price must be greater than 0");
            _gameController.ModelState.AddModelError("ReleaseDate", "Release date is required");

            // Act
            var result = _gameController.Cadastrar(game);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            var errors = badRequestResult!.Value as SerializableError;
            errors.Should().NotBeNull();
            errors.Should().HaveCount(3);
            errors.Should().ContainKeys("Title", "Price", "ReleaseDate");
        }

        [Fact]
        public void Cadastrar_WithLargeGameObject_ShouldHandleCorrectly()
        {
            // Arrange
            var game = new Game
            {
                Id = int.MaxValue,
                Title = new string('A', 100), // Maximum length
                Description = new string('B', 1000), // Maximum length
                Price = 999.99m, // Near maximum
                ReleaseDate = DateTime.MaxValue
            };

            _mockGameService.Setup(s => s.Cadastrar(It.IsAny<Game>())).Verifiable();

            // Act
            var result = _gameController.Cadastrar(game);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            _mockGameService.Verify(s => s.Cadastrar(game), Times.Once);
        }

        [Theory]
        [InlineData(int.MaxValue)]
        [InlineData(1)]
        [InlineData(42)]
        public void ObterPorId_WithVariousValidIds_ShouldCallService(int gameId)
        {
            // Arrange
            _mockGameService.Setup(s => s.ObterPorId(gameId)).Returns((Game?)null);

            // Act
            var result = _gameController.ObterPorId(gameId);

            // Assert
            _mockGameService.Verify(s => s.ObterPorId(gameId), Times.Once);
        }

        #endregion
    }
}
