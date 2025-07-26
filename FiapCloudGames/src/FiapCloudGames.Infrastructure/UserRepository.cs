using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FiapCloudGames.Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public User GetById(string id)
        {
            if (int.TryParse(id, out int userId))
                return _context.Users.Find(userId);
            return null;
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}