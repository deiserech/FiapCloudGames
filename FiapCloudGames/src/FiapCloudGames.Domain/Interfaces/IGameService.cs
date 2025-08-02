using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface IGameService
    {
        void Cadastrar(Game game);
        Game? ObterPorId(int id);
        IEnumerable<Game> ListarTodos();
    }
}
