using Microsoft.EntityFrameworkCore;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using FiapCloudGames.Domain.Interfaces.Repositories;

namespace FiapCloudGames.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _context;
        private static readonly List<Game> _games = new();
        private static int _nextId = 1;

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
            _context.Entry(game).State = EntityState.Modified;
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
