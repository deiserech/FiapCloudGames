using FiapCloudGames.Domain.Entities;
using System.Collections.Generic;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface IGameRepository
    {
        void Add(Game game);
        Game? GetById(int id);
        IEnumerable<Game> GetAll();
        Task<Game?> GetByIdAsync(int id);
        Task<IEnumerable<Game>> GetAllAsync();
        Task<Game> CreateAsync(Game game);
        Task<Game> UpdateAsync(Game game);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
