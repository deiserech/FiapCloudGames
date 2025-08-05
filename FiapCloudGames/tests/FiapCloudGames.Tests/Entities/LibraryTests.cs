using FiapCloudGames.Domain.Entities;
using Xunit;

namespace FiapCloudGames.Tests.Entities
{
    public class LibraryTests
    {
        [Fact]
        public void IsRecentPurchase_ShouldReturnTrue_WhenPurchaseIsWithinDays()
        {
            // Arrange
            var library = new Library
            {
                Id = 1,
                UserId = 1,
                GameId = 1,
                PurchaseDate = DateTime.Now.AddDays(-15),
                PurchasePrice = 50.00m,
                IsGift = false
            };

            // Act
            var result = library.IsRecentPurchase(30);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsRecentPurchase_ShouldReturnFalse_WhenPurchaseIsOlderThanDays()
        {
            // Arrange
            var library = new Library
            {
                Id = 1,
                UserId = 1,
                GameId = 1,
                PurchaseDate = DateTime.Now.AddDays(-45),
                PurchasePrice = 50.00m,
                IsGift = false
            };

            // Act
            var result = library.IsRecentPurchase(30);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WasPurchasedOnSale_ShouldReturnTrue_WhenPurchasePriceIsLowerThanOriginal()
        {
            // Arrange
            var originalPrice = 100.00m;
            var library = new Library
            {
                Id = 1,
                UserId = 1,
                GameId = 1,
                PurchaseDate = DateTime.Now,
                PurchasePrice = 80.00m,
                IsGift = false
            };

            // Act
            var result = library.WasPurchasedOnSale(originalPrice);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WasPurchasedOnSale_ShouldReturnFalse_WhenPurchasePriceEqualsOriginal()
        {
            // Arrange
            var originalPrice = 100.00m;
            var library = new Library
            {
                Id = 1,
                UserId = 1,
                GameId = 1,
                PurchaseDate = DateTime.Now,
                PurchasePrice = 100.00m,
                IsGift = false
            };

            // Act
            var result = library.WasPurchasedOnSale(originalPrice);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetSavingsAmount_ShouldReturnCorrectAmount_WhenPurchasedOnSale()
        {
            // Arrange
            var originalPrice = 100.00m;
            var library = new Library
            {
                Id = 1,
                UserId = 1,
                GameId = 1,
                PurchaseDate = DateTime.Now,
                PurchasePrice = 75.00m,
                IsGift = false
            };

            // Act
            var savings = library.GetSavingsAmount(originalPrice);

            // Assert
            Assert.Equal(25.00m, savings);
        }

        [Fact]
        public void GetSavingsAmount_ShouldReturnZero_WhenNotPurchasedOnSale()
        {
            // Arrange
            var originalPrice = 100.00m;
            var library = new Library
            {
                Id = 1,
                UserId = 1,
                GameId = 1,
                PurchaseDate = DateTime.Now,
                PurchasePrice = 100.00m,
                IsGift = false
            };

            // Act
            var savings = library.GetSavingsAmount(originalPrice);

            // Assert
            Assert.Equal(0m, savings);
        }

        [Fact]
        public void GetSavingsPercentage_ShouldReturnCorrectPercentage_WhenPurchasedOnSale()
        {
            // Arrange
            var originalPrice = 100.00m;
            var library = new Library
            {
                Id = 1,
                UserId = 1,
                GameId = 1,
                PurchaseDate = DateTime.Now,
                PurchasePrice = 80.00m,
                IsGift = false
            };

            // Act
            var savingsPercentage = library.GetSavingsPercentage(originalPrice);

            // Assert
            Assert.Equal(20m, savingsPercentage);
        }

        [Fact]
        public void GetSavingsPercentage_ShouldReturnZero_WhenOriginalPriceIsZero()
        {
            // Arrange
            var originalPrice = 0m;
            var library = new Library
            {
                Id = 1,
                UserId = 1,
                GameId = 1,
                PurchaseDate = DateTime.Now,
                PurchasePrice = 50.00m,
                IsGift = false
            };

            // Act
            var savingsPercentage = library.GetSavingsPercentage(originalPrice);

            // Assert
            Assert.Equal(0m, savingsPercentage);
        }

        [Fact]
        public void GetSavingsPercentage_ShouldReturnZero_WhenNotPurchasedOnSale()
        {
            // Arrange
            var originalPrice = 100.00m;
            var library = new Library
            {
                Id = 1,
                UserId = 1,
                GameId = 1,
                PurchaseDate = DateTime.Now,
                PurchasePrice = 110.00m, // Mais caro que o original
                IsGift = false
            };

            // Act
            var savingsPercentage = library.GetSavingsPercentage(originalPrice);

            // Assert
            Assert.Equal(0m, savingsPercentage);
        }
    }
}
