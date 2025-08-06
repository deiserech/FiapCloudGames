using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetAllPromotionsAsync();
        Task<Promotion?> GetPromotionByIdAsync(int id);
        Task<IEnumerable<Promotion>> GetPromotionsByGameIdAsync(int gameId);
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
        Task<IEnumerable<Promotion>> GetActivePromotionsByGameIdAsync(int gameId);
        Task<Promotion> CreatePromotionAsync(Promotion promotion);
        Task<Promotion> UpdatePromotionAsync(Promotion promotion);
        Task DeletePromotionAsync(int id);
        Task<decimal> GetDiscountedPriceAsync(int gameId, decimal originalPrice);
        Task<Promotion?> GetBestPromotionForGameAsync(int gameId);
    }
}
