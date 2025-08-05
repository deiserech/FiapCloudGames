using Microsoft.EntityFrameworkCore;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using FiapCloudGames.Infrastructure.Data;

namespace FiapCloudGames.Infrastructure
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly AppDbContext _context;

        public LibraryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Library>> GetAllAsync()
        {
            return await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Game)
                .ToListAsync();
        }

        public async Task<Library?> GetByIdAsync(int id)
        {
            return await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Game)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Library>> GetByUserIdAsync(int userId)
        {
            return await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Game)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.PurchaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Library>> GetByGameIdAsync(int gameId)
        {
            return await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Game)
                .Where(l => l.GameId == gameId)
                .OrderByDescending(l => l.PurchaseDate)
                .ToListAsync();
        }

        public async Task<Library?> GetByUserAndGameAsync(int userId, int gameId)
        {
            return await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Game)
                .FirstOrDefaultAsync(l => l.UserId == userId && l.GameId == gameId);
        }

        public async Task<IEnumerable<Library>> GetRecentPurchasesAsync(int userId, int days = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            return await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Game)
                .Where(l => l.UserId == userId && l.PurchaseDate >= cutoffDate)
                .OrderByDescending(l => l.PurchaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Library>> GetGiftedGamesAsync(int userId)
        {
            return await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Game)
                .Where(l => l.UserId == userId && l.IsGift)
                .OrderByDescending(l => l.PurchaseDate)
                .ToListAsync();
        }

        public async Task<Library> CreateAsync(Library library)
        {
            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();
            
            // Retorna a entrada da biblioteca com as entidades relacionadas incluídas
            return await GetByIdAsync(library.Id) ?? library;
        }

        public async Task<Library> UpdateAsync(Library library)
        {
            _context.Entry(library).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            // Retorna a entrada atualizada com as entidades relacionadas incluídas
            return await GetByIdAsync(library.Id) ?? library;
        }

        public async Task DeleteAsync(int id)
        {
            var library = await _context.Libraries.FindAsync(id);
            if (library != null)
            {
                _context.Libraries.Remove(library);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UserOwnsGameAsync(int userId, int gameId)
        {
            return await _context.Libraries
                .AnyAsync(l => l.UserId == userId && l.GameId == gameId);
        }

        public async Task<int> GetTotalGamesCountByUserAsync(int userId)
        {
            return await _context.Libraries
                .CountAsync(l => l.UserId == userId);
        }

        public async Task<decimal> GetTotalSpentByUserAsync(int userId)
        {
            return await _context.Libraries
                .Where(l => l.UserId == userId)
                .SumAsync(l => l.PurchasePrice);
        }

        public async Task<IEnumerable<Library>> GetPurchasesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Libraries
                .Include(l => l.User)
                .Include(l => l.Game)
                .Where(l => l.UserId == userId && l.PurchaseDate >= startDate && l.PurchaseDate <= endDate)
                .OrderByDescending(l => l.PurchaseDate)
                .ToListAsync();
        }
    }
}
