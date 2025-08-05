using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface IUserRepository
    {
        User? GetById(string id);
        User? GetByEmail(string email);
        void Add(User user);
        bool EmailExists(string email);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}
