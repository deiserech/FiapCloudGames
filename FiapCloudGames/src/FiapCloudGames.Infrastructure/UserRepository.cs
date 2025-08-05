using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using FiapCloudGames.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Métodos síncronos (mantidos para compatibilidade)
        public User? GetById(string id)
        {
            if (int.TryParse(id, out int userId))
                return _context.Set<User>().Find(userId);
            return null;
        }

        public User? GetByEmail(string email)
        {
            return _context.Set<User>().FirstOrDefault(u => u.Email == email);
        }

        public void Add(User user)
        {
            _context.Set<User>().Add(user);
            _context.SaveChanges();
        }

        public bool EmailExists(string email)
        {
            return _context.Set<User>().Any(u => u.Email == email);
        }

        // Métodos assíncronos
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.LibraryGames)
                    .ThenInclude(l => l.Game)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.LibraryGames)
                    .ThenInclude(l => l.Game)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.LibraryGames)
                    .ThenInclude(l => l.Game)
                .ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // Retorna o usuário com as entidades relacionadas incluídas
            return await GetByIdAsync(user.Id) ?? user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            // Retorna o usuário atualizado com as entidades relacionadas incluídas
            return await GetByIdAsync(user.Id) ?? user;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
