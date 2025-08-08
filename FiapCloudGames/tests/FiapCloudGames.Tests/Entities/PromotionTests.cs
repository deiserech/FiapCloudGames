using FiapCloudGames.Domain.Entities;
using Xunit;

namespace FiapCloudGames.Tests.Entities
{
    public class PromotionTests
    {
        [Fact]
        public void IsValidPromotion_ShouldReturnTrue_WhenPromotionIsActiveAndWithinDateRange()
        {
            // Arrange
            var promotion = new Promotion
            {
                Id = 1,
                Title = "Promoção de Verão",
                Description = "Desconto especial para jogos de ação",
                DiscountPercentage = 20,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(7),
                IsActive = true,
                GameId = 1
            };

            // Act
            var result = promotion.IsValidPromotion();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidPromotion_ShouldReturnFalse_WhenPromotionIsInactive()
        {
            // Arrange
            var promotion = new Promotion
            {
                Id = 1,
                Title = "Promoção Inativa",
                Description = "Promoção desativada",
                DiscountPercentage = 20,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(7),
                IsActive = false,
                GameId = 1
            };

            // Act
            var result = promotion.IsValidPromotion();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidPromotion_ShouldReturnFalse_WhenPromotionHasExpired()
        {
            // Arrange
            var promotion = new Promotion
            {
                Id = 1,
                Title = "Promoção Expirada",
                Description = "Promoção que já passou",
                DiscountPercentage = 20,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(-1),
                IsActive = true,
                GameId = 1
            };

            // Act
            var result = promotion.IsValidPromotion();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CalculateDiscountedPrice_ShouldApplyPercentageDiscount_WhenPromotionIsValid()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Title = "Test Game",
                Price = 100m
            };

           var promotion = new Promotion
            {
                Id = 1,
                Title = "Desconto de 20%",
                Description = "Desconto percentual",
                DiscountPercentage = 20,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(7),
                IsActive = true,
                GameId = 1,
                Game = game
            };

            // Act
            var discountedPrice = promotion.CalculateDiscountedPrice();

            // Assert
            Assert.Equal(80m, discountedPrice);
        }

        [Fact]
        public void CalculateDiscountedPrice_ShouldApplyFixedDiscount_WhenDiscountAmountIsSpecified()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Title = "Test Game",
                Price = 100m
            };

            var promotion = new Promotion
            {
                Id = 1,
                Title = "Desconto de R$ 15",
                Description = "Desconto fixo",
                DiscountPercentage = 20, 
                DiscountAmount = 15m,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(7),
                IsActive = true,
                GameId = 1,
                Game = game
            };

            // Act
            var discountedPrice = promotion?.CalculateDiscountedPrice() ?? 0;

            // Assert
            Assert.Equal(85m, discountedPrice);
        }

        [Fact]
        public void CalculateDiscountedPrice_ShouldReturnOriginalPrice_WhenPromotionIsInvalid()
        {
            // Arrange
            var originalPrice = 100m;
            var game = new Game
            {
                Id = 1,
                Title = "Test Game",
                Price = originalPrice
            };

            var promotion = new Promotion
            {
                Id = 1,
                Title = "Promoção Inválida",
                Description = "Promoção expirada",
                DiscountPercentage = 20,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(-1),
                IsActive = true,
                GameId = 1,
                Game = game
            };

            // Act
            var discountedPrice = promotion?.CalculateDiscountedPrice() ?? 0;

            // Assert
            Assert.Equal(originalPrice, discountedPrice);
        }

        [Fact]
        public void CalculateDiscountedPrice_ShouldNotGoBelowZero_WhenDiscountIsLargerThanPrice()
        {
            // Arrange
            var game = new Game
            {
                Id = 1,
                Title = "Test Game",
                Price = 10m // Preço menor que o desconto
            };

            var promotion = new Promotion
            {
                Id = 1,
                Title = "Desconto Maior que Preço",
                Description = "Desconto que excede o preço",
                DiscountAmount = 15m,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(7),
                IsActive = true,
                GameId = 1,
                Game = game
            };

            // Act
            var discountedPrice = promotion?.CalculateDiscountedPrice() ?? 0;

            // Assert
            Assert.Equal(0m, discountedPrice);
        }
    }
}
