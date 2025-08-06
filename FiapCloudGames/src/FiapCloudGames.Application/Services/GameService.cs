using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repositories;
using FiapCloudGames.Domain.Interfaces.Services;

namespace FiapCloudGames.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repo;

        public GameService(IGameRepository repo)
        {
            _repo = repo;
        }

         public async Task<IEnumerable<Game>> GetallAsync() => await _repo.GetAllAsync();

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Game> CreateAsync(Game game)
        {
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
