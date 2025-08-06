using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Enums;
using FiapCloudGames.Infrastructure;
using FiapCloudGames.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FiapCloudGames.Domain.Interfaces.Repositories;

namespace FiapCloudGames.Tests.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                Role = UserRole.User
            };
            user.SetPassword("testpassword");

            var createdUser = await _repository.CreateAsync(user);

            // Act
            var exists = await _repository.ExistsAsync(createdUser.Id);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var exists = await _repository.ExistsAsync(nonExistentId);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Name = "Another Test User",
                Email = "another@example.com",
                Role = UserRole.User
            };
            user.SetPassword("anotherpassword");

            var createdUser = await _repository.CreateAsync(user);

            // Act
            var retrievedUser = await _repository.GetByIdAsync(createdUser.Id);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(createdUser.Name, retrievedUser.Name);
            Assert.Equal(createdUser.Email, retrievedUser.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var user = await _repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            // Arrange
            var user = new User
            {
                Name = "Email Test User",
                Email = "email@test.com",
                Role = UserRole.User
            };
            user.SetPassword("emailpassword");

            await _repository.CreateAsync(user);

            // Act
            var retrievedUser = await _repository.GetByEmailAsync("email@test.com");

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Name, retrievedUser.Name);
            Assert.Equal(user.Email, retrievedUser.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            // Arrange
            var nonExistentEmail = "nonexistent@example.com";

            // Act
            var user = await _repository.GetByEmailAsync(nonExistentEmail);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
        {
            // Arrange
            var user = new User
            {
                Name = "Email Exists Test",
                Email = "exists@test.com",
                Role = UserRole.User
            };
            user.SetPassword("existspassword");

            await _repository.CreateAsync(user);

            // Act
            var exists = await _repository.EmailExistsAsync("exists@test.com");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
        {
            // Arrange
            var nonExistentEmail = "doesnotexist@example.com";

            // Act
            var exists = await _repository.EmailExistsAsync(nonExistentEmail);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateUser_WithValidData()
        {
            // Arrange
            var user = new User
            {
                Name = "New User",
                Email = "new@user.com",
                Role = UserRole.Admin
            };
            user.SetPassword("newuserpassword");

            // Act
            var createdUser = await _repository.CreateAsync(user);

            // Assert
            Assert.NotNull(createdUser);
            Assert.True(createdUser.Id > 0);
            Assert.Equal(user.Name, createdUser.Name);
            Assert.Equal(user.Email, createdUser.Email);
            
            // Verify it exists in database
            var exists = await _repository.ExistsAsync(createdUser.Id);
            Assert.True(exists);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Name = "Original Name",
                Email = "original@email.com",
                Role = UserRole.User
            };
            user.SetPassword("originalpassword");

            var createdUser = await _repository.CreateAsync(user);
            createdUser.Name = "Updated Name";
            createdUser.Role = UserRole.Admin;

            // Act
            var updatedUser = await _repository.UpdateAsync(createdUser);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("Updated Name", updatedUser.Name);
            Assert.Equal(UserRole.Admin, updatedUser.Role);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Name = "User to Delete",
                Email = "delete@test.com",
                Role = UserRole.User
            };
            user.SetPassword("deletepassword");

            var createdUser = await _repository.CreateAsync(user);
            var userId = createdUser.Id;

            // Verify it exists first
            var existsBeforeDelete = await _repository.ExistsAsync(userId);
            Assert.True(existsBeforeDelete);

            // Act
            await _repository.DeleteAsync(userId);

            // Assert
            var existsAfterDelete = await _repository.ExistsAsync(userId);
            Assert.False(existsAfterDelete);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var user1 = new User { Name = "User 1", Email = "user1@test.com", Role = UserRole.User };
            var user2 = new User { Name = "User 2", Email = "user2@test.com", Role = UserRole.Admin };
            
            user1.SetPassword("password1");
            user2.SetPassword("password2");

            await _repository.CreateAsync(user1);
            await _repository.CreateAsync(user2);

            // Act
            var allUsers = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(allUsers);
            Assert.True(allUsers.Count() >= 2);
            Assert.Contains(allUsers, u => u.Email == "user1@test.com");
            Assert.Contains(allUsers, u => u.Email == "user2@test.com");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
