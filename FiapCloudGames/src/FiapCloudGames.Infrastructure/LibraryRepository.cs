using Microsoft.EntityFrameworkCore;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGames.Domain.Interfaces.Repositories;

namespace FiapCloudGames.Infrastructure
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly AppDbContext _context;

        public LibraryRepository(AppDbContext context)
        {
            _context = context;
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
                .ToListAsync();
        }

        public async Task<Library> CreateAsync(Library library)
        {
            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();

            // Retorna a entrada da biblioteca com as entidades relacionadas incluídas
            return await GetByIdAsync(library.Id) ?? library;
        }
        public async Task<bool> UserOwnsGameAsync(int userId, int gameId)
        {
            return await _context.Libraries
                .AnyAsync(l => l.UserId == userId && l.GameId == gameId);
        }

    }
}
