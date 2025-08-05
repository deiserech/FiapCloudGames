using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Domain.DTOs
{
    public class LibraryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int GameId { get; set; }
        public string? GameTitle { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool IsGift { get; set; }
        public string? GiftMessage { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? SavingsAmount { get; set; }
        public decimal? SavingsPercentage { get; set; }
        public bool IsRecentPurchase { get; set; }
    }
}
