using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Repositories;
using FiapCloudGames.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FiapCloudGames.Domain.Interfaces.Repositories;

namespace FiapCloudGames.Tests.Repositories
{
    public class GameRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IGameRepository _repository;

        public GameRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new GameRepository(_context);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenGameExists()
        {
            // Arrange
            var game = new Game
            {
                Title = "Test Game",
                Description = "A test game",
                Price = 59.99m,
                ReleaseDate = DateTime.Now
            };

            var createdGame = await _repository.CreateAsync(game);

            // Act
            var exists = await _repository.ExistsAsync(createdGame.Id);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenGameDoesNotExist()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var exists = await _repository.ExistsAsync(nonExistentId);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGame_WhenGameExists()
        {
            // Arrange
            var game = new Game
            {
                Title = "Another Test Game",
                Description = "Another test game",
                Price = 29.99m,
                ReleaseDate = DateTime.Now
            };

            var createdGame = await _repository.CreateAsync(game);

            // Act
            var retrievedGame = await _repository.GetByIdAsync(createdGame.Id);

            // Assert
            Assert.NotNull(retrievedGame);
            Assert.Equal(createdGame.Title, retrievedGame.Title);
            Assert.Equal(createdGame.Price, retrievedGame.Price);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenGameDoesNotExist()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var game = await _repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(game);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateGame_WithValidData()
        {
            // Arrange
            var game = new Game
            {
                Title = "New Game",
                Description = "A brand new game",
                Price = 49.99m,
                ReleaseDate = DateTime.Now.AddDays(30)
            };

            // Act
            var createdGame = await _repository.CreateAsync(game);

            // Assert
            Assert.NotNull(createdGame);
            Assert.True(createdGame.Id > 0);
            Assert.Equal(game.Title, createdGame.Title);
            
            // Verify it exists in database
            var exists = await _repository.ExistsAsync(createdGame.Id);
            Assert.True(exists);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateGame_WhenGameExists()
        {
            // Arrange
            var game = new Game
            {
                Title = "Original Title",
                Description = "Original description",
                Price = 39.99m,
                ReleaseDate = DateTime.Now
            };

            var createdGame = await _repository.CreateAsync(game);
            createdGame.Title = "Updated Title";
            createdGame.Price = 49.99m;

            // Act
            var updatedGame = await _repository.UpdateAsync(createdGame);

            // Assert
            Assert.NotNull(updatedGame);
            Assert.Equal("Updated Title", updatedGame.Title);
            Assert.Equal(49.99m, updatedGame.Price);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveGame_WhenGameExists()
        {
            // Arrange
            var game = new Game
            {
                Title = "Game to Delete",
                Description = "This game will be deleted",
                Price = 19.99m,
                ReleaseDate = DateTime.Now
            };

            var createdGame = await _repository.CreateAsync(game);
            var gameId = createdGame.Id;

            // Verify it exists first
            var existsBeforeDelete = await _repository.ExistsAsync(gameId);
            Assert.True(existsBeforeDelete);

            // Act
            await _repository.DeleteAsync(gameId);

            // Assert
            var existsAfterDelete = await _repository.ExistsAsync(gameId);
            Assert.False(existsAfterDelete);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllGames()
        {
            // Arrange
            var game1 = new Game { Title = "Game 1", Description = "First game", Price = 10.99m, ReleaseDate = DateTime.Now };
            var game2 = new Game { Title = "Game 2", Description = "Second game", Price = 20.99m, ReleaseDate = DateTime.Now };

            await _repository.CreateAsync(game1);
            await _repository.CreateAsync(game2);

            // Act
            var allGames = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(allGames);
            Assert.True(allGames.Count() >= 2);
            Assert.Contains(allGames, g => g.Title == "Game 1");
            Assert.Contains(allGames, g => g.Title == "Game 2");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
