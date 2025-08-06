using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface IUserService
    {
        User? GetByIdAsync(string id);
        void CreateUserAsync(User user);
    }
}
