using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface IUserRepository
    {
        User? GetById(string id);
        User? GetByEmail(string email);
        void Add(User user);
        bool EmailExists(string email);
    }
}
