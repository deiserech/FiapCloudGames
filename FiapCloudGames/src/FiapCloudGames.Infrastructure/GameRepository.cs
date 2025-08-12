using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repositories;
using FiapCloudGames.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _context;

        public GameRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games
                .Include(g => g.Promotions)
                .Include(g => g.LibraryEntries)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Games
                .Include(g => g.Promotions)
                .Include(g => g.LibraryEntries)
                .ToListAsync();
        }

        public async Task<Game> CreateAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(game.Id) ?? game;
        }

        public async Task<Game> UpdateAsync(Game game)
        {
            await _context.SaveChangesAsync();

            return await GetByIdAsync(game.Id) ?? game;
        }

        public async Task DeleteAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Games.AnyAsync(g => g.Id == id);
        }
    }
}
