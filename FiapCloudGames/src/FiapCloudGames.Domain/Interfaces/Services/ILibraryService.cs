using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces.Services
{
    public interface ILibraryService
    {
        Task<IEnumerable<Library>> GetUserLibraryAsync(int userId);
        Task<Library?> GetLibraryEntryAsync(int id);
        Task<Library?> GetUserGameAsync(int userId, int gameId);
        Task<IEnumerable<Library>> GetRecentPurchasesAsync(int userId, int days = 30);
        Task<IEnumerable<Library>> GetGiftedGamesAsync(int userId);
        Task<Library> PurchaseGameAsync(int userId, int gameId, decimal purchasePrice, bool isGift = false, string? giftMessage = null);
        Task<Library> GiftGameAsync(int fromUserId, int toUserId, int gameId, string? giftMessage = null);
        Task<bool> UserOwnsGameAsync(int userId, int gameId);
        Task<int> GetTotalGamesCountAsync(int userId);
        Task<decimal> GetTotalSpentAsync(int userId);
        Task<decimal> GetTotalSavingsAsync(int userId);
        Task<IEnumerable<Library>> GetPurchaseHistoryAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task RemoveFromLibraryAsync(int userId, int gameId);
        Task<IEnumerable<Game>> GetRecommendedGamesAsync(int userId, int count = 10);
    }
}
