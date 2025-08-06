using Xunit;
using Moq;
using FluentAssertions;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using FiapCloudGames.Domain.Interfaces.Repositories;

namespace FiapCloudGames.Tests.Services
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _mockGameRepository;
        private readonly GameService _gameService;

        public GameServiceTests()
        {
            _mockGameRepository = new Mock<IGameRepository>();
            _gameService = new GameService(_mockGameRepository.Object);
        }

        [Fact]
        public void Cadastrar_WithValidGame_ShouldCallRepositoryAdd()
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

            // Act
            _gameService.CreateGameAsync(game);

            // Assert
            _mockGameRepository.Verify(repo => repo.Add(game), Times.Once);
        }

        [Fact]
        public void Cadastrar_WithNullGame_ShouldStillCallRepository()
        {
            // Arrange
            Game nullGame = null!;

            // Act & Assert
            // O service não faz validação de null, então deve passar para o repository
            _gameService.CreateGameAsync(nullGame);
            _mockGameRepository.Verify(repo => repo.Add(nullGame), Times.Once);
        }

        [Fact]
        public void ObterPorId_WithValidId_ShouldReturnGame()
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

            _mockGameRepository.Setup(repo => repo.GetById(gameId))
                              .Returns(expectedGame);

            // Act
            var result = _gameService.GetByAsync(gameId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedGame);
            result!.Id.Should().Be(gameId);
            result.Title.Should().Be("Test Game");
            _mockGameRepository.Verify(repo => repo.GetById(gameId), Times.Once);
        }

        [Fact]
        public void ObterPorId_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;
            _mockGameRepository.Setup(repo => repo.GetById(invalidId))
                              .Returns((Game?)null);

            // Act
            var result = _gameService.GetByAsync(invalidId);

            // Assert
            result.Should().BeNull();
            _mockGameRepository.Verify(repo => repo.GetById(invalidId), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ObterPorId_WithInvalidIds_ShouldCallRepository(int invalidId)
        {
            // Arrange
            _mockGameRepository.Setup(repo => repo.GetById(invalidId))
                              .Returns((Game?)null);

            // Act
            var result = _gameService.GetByAsync(invalidId);

            // Assert
            result.Should().BeNull();
            _mockGameRepository.Verify(repo => repo.GetById(invalidId), Times.Once);
        }

        [Fact]
        public void ListarTodos_WithGamesInRepository_ShouldReturnAllGames()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    Title = "Game 1",
                    Description = "First game",
                    Price = 29.99m,
                    ReleaseDate = DateTime.Now.AddDays(-30)
                },
                new Game
                {
                    Id = 2,
                    Title = "Game 2",
                    Description = "Second game",
                    Price = 39.99m,
                    ReleaseDate = DateTime.Now.AddDays(-15)
                },
                new Game
                {
                    Id = 3,
                    Title = "Game 3",
                    Description = "Third game",
                    Price = 49.99m,
                    ReleaseDate = DateTime.Now
                }
            };

            _mockGameRepository.Setup(repo => repo.GetAll())
                              .Returns(games);

            // Act
            var result = _gameService.GetallAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(games);
            _mockGameRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void ListarTodos_WithEmptyRepository_ShouldReturnEmptyCollection()
        {
            // Arrange
            var emptyGames = new List<Game>();
            _mockGameRepository.Setup(repo => repo.GetAll())
                              .Returns(emptyGames);

            // Act
            var result = _gameService.GetallAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockGameRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void ListarTodos_ShouldReturnSameCollectionFromRepository()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    Title = "Test Game",
                    Description = "Test Description",
                    Price = 19.99m,
                    ReleaseDate = DateTime.Now
                }
            };

            _mockGameRepository.Setup(repo => repo.GetAll())
                              .Returns(games);

            // Act
            var result = _gameService.GetallAsync();

            // Assert
            result.Should().BeSameAs(games);
            _mockGameRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void GameService_ShouldBeInstantiableWithValidRepository()
        {
            // Arrange
            var mockRepo = new Mock<IGameRepository>();

            // Act
            var service = new GameService(mockRepo.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void Cadastrar_MultipleCalls_ShouldCallRepositoryMultipleTimes()
        {
            // Arrange
            var game1 = new Game { Id = 1, Title = "Game 1", Price = 10.00m, ReleaseDate = DateTime.Now };
            var game2 = new Game { Id = 2, Title = "Game 2", Price = 20.00m, ReleaseDate = DateTime.Now };

            // Act
            _gameService.CreateGameAsync(game1);
            _gameService.CreateGameAsync(game2);

            // Assert
            _mockGameRepository.Verify(repo => repo.Add(It.IsAny<Game>()), Times.Exactly(2));
            _mockGameRepository.Verify(repo => repo.Add(game1), Times.Once);
            _mockGameRepository.Verify(repo => repo.Add(game2), Times.Once);
        }

        [Fact]
        public void ObterPorId_MultipleCalls_ShouldCallRepositoryForEachCall()
        {
            // Arrange
            var game1 = new Game { Id = 1, Title = "Game 1", Price = 10.00m, ReleaseDate = DateTime.Now };
            var game2 = new Game { Id = 2, Title = "Game 2", Price = 20.00m, ReleaseDate = DateTime.Now };

            _mockGameRepository.Setup(repo => repo.GetById(1)).Returns(game1);
            _mockGameRepository.Setup(repo => repo.GetById(2)).Returns(game2);

            // Act
            var result1 = _gameService.GetByAsync(1);
            var result2 = _gameService.GetByAsync(2);

            // Assert
            result1.Should().Be(game1);
            result2.Should().Be(game2);
            _mockGameRepository.Verify(repo => repo.GetById(1), Times.Once);
            _mockGameRepository.Verify(repo => repo.GetById(2), Times.Once);
        }

        [Fact]
        public void ListarTodos_MultipleCalls_ShouldCallRepositoryForEachCall()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game { Id = 1, Title = "Game 1", Price = 10.00m, ReleaseDate = DateTime.Now }
            };

            _mockGameRepository.Setup(repo => repo.GetAll()).Returns(games);

            // Act
            var result1 = _gameService.GetallAsync();
            var result2 = _gameService.GetallAsync();

            // Assert
            result1.Should().BeEquivalentTo(games);
            result2.Should().BeEquivalentTo(games);
            _mockGameRepository.Verify(repo => repo.GetAll(), Times.Exactly(2));
        }
    }
}
