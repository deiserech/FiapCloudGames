using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext _context;

        public UserRepository(DbContext context)
        {
            _context = context;
        }

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
    }
}
