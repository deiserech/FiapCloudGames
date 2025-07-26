using System.Collections.Generic;
using System.Linq;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;

namespace FiapCloudGames.Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        private static readonly List<User> _users = new();

        public User GetById(string id)
        {
            return _users.FirstOrDefault(c => c.Id == id);
        }

        public void Add(User user)
        {
            _users.Add(user);
        }
    }
}