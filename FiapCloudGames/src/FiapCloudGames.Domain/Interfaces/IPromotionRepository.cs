using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<Promotion?> GetByIdAsync(int id);
        Task<IEnumerable<Promotion>> GetByGameIdAsync(int gameId);
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
        Task<IEnumerable<Promotion>> GetActivePromotionsByGameIdAsync(int gameId);
        Task<Promotion> CreateAsync(Promotion promotion);
        Task<Promotion> UpdateAsync(Promotion promotion);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
