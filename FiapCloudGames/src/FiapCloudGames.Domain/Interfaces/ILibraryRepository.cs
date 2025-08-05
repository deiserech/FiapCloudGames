using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface ILibraryRepository
    {
        Task<IEnumerable<Library>> GetAllAsync();
        Task<Library?> GetByIdAsync(int id);
        Task<IEnumerable<Library>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Library>> GetByGameIdAsync(int gameId);
        Task<Library?> GetByUserAndGameAsync(int userId, int gameId);
        Task<IEnumerable<Library>> GetRecentPurchasesAsync(int userId, int days = 30);
        Task<IEnumerable<Library>> GetGiftedGamesAsync(int userId);
        Task<Library> CreateAsync(Library library);
        Task<Library> UpdateAsync(Library library);
        Task DeleteAsync(int id);
        Task<bool> UserOwnsGameAsync(int userId, int gameId);
        Task<int> GetTotalGamesCountByUserAsync(int userId);
        Task<decimal> GetTotalSpentByUserAsync(int userId);
        Task<IEnumerable<Library>> GetPurchasesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    }
}
