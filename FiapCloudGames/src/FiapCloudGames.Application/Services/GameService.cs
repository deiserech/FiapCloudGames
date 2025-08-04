using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using System.Collections.Generic;

namespace FiapCloudGames.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repo;

        public GameService(IGameRepository repo)
        {
            _repo = repo;
        }

        public void Cadastrar(Game game) => _repo.Add(game);

        public Game? ObterPorId(int id) => _repo.GetById(id);

        public IEnumerable<Game> ListarTodos() => _repo.GetAll();
    }
}
