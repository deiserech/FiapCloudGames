using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Domain.Entities
{
    public class Library
    {
        public int Id { get; set; }

        // Foreign Key para User
        public int UserId { get; set; }

        // Foreign Key para Game
        public int GameId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PurchaseDate { get; set; }

        [Range(0.01, 1000.00)]
        public decimal PurchasePrice { get; set; }

        public bool IsGift { get; set; } = false;

        [StringLength(500)]
        public string? GiftMessage { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public Game Game { get; set; } = null!;

        public bool IsRecentPurchase(int days = 30)
        {
            return PurchaseDate.AddDays(days) >= DateTime.Now;
        }

        public bool WasPurchasedOnSale(decimal originalPrice)
        {
            return PurchasePrice < originalPrice;
        }

        public decimal GetSavingsAmount(decimal originalPrice)
        {
            if (PurchasePrice >= originalPrice)
                return 0;
            
            return originalPrice - PurchasePrice;
        }

        public decimal GetSavingsPercentage(decimal originalPrice)
        {
            if (originalPrice <= 0 || PurchasePrice >= originalPrice)
                return 0;

            return ((originalPrice - PurchasePrice) / originalPrice) * 100;
        }
    }
}
