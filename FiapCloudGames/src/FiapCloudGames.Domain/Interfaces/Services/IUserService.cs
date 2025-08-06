using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int id);
        Task CreateUserAsync(User user);
    }
}
