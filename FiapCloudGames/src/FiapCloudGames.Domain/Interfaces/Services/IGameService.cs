using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces.Services
{
    public interface IGameService
    {
        IEnumerable<Game> GetallAsync();
        Task<Game?> GetByIdAsync(int id);
        Task<IEnumerable<Game>> GetAllAsync();
        Task<Game> CreateAsync(Game game);
    }
}
