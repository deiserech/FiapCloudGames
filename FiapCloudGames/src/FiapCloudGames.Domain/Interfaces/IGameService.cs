using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface IGameService
    {
        void Cadastrar(Game game);
        Game? ObterPorId(int id);
        IEnumerable<Game> ListarTodos();
        Task<Game?> GetByIdAsync(int id);
        Task<IEnumerable<Game>> GetAllAsync();
        Task<Game> CreateAsync(Game game);
        Task<Game> UpdateAsync(Game game);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
