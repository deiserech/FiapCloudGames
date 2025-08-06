using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repositories;
using FiapCloudGames.Domain.Interfaces.Services;
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

         public IEnumerable<Game> GetallAsync() => _repo.GetAll();

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Game> CreateAsync(Game game)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(game.Title))
            {
                throw new ArgumentException("O título do jogo é obrigatório.");
            }

            if (game.Price <= 0)
            {
                throw new ArgumentException("O preço do jogo deve ser maior que zero.");
            }

            return await _repo.CreateAsync(game);
        }
    }
}
