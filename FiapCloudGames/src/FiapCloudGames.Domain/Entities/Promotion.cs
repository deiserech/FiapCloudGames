using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Domain.Entities
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 100.00)]
        public decimal DiscountPercentage { get; set; }

        [Range(0.01, 1000.00)]
        public decimal? DiscountAmount { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int GameId { get; set; }

        public Game Game { get; set; } = null!;

        public bool IsValidPromotion()
        {
            var now = DateTime.Now;
            return IsActive && now >= StartDate && now <= EndDate;
        }

        public decimal CalculateDiscountedPrice(decimal originalPrice)
        {
            if (!IsValidPromotion())
                return originalPrice;

            if (DiscountAmount.HasValue)
            {
                return Math.Max(0, originalPrice - DiscountAmount.Value);
            }

            var discountValue = originalPrice * (DiscountPercentage / 100);
            return Math.Max(0, originalPrice - discountValue);
        }
    }
}
