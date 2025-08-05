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

        // Métodos síncronos (mantidos para compatibilidade)
        public void Cadastrar(Game game) => _repo.Add(game);

        public Game? ObterPorId(int id) => _repo.GetById(id);

        public IEnumerable<Game> ListarTodos() => _repo.GetAll();

        // Métodos assíncronos
        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
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

        public async Task<Game> UpdateAsync(Game game)
        {
            var existingGame = await _repo.GetByIdAsync(game.Id);
            if (existingGame == null)
            {
                throw new ArgumentException("Jogo não encontrado.");
            }

            // Validações básicas
            if (string.IsNullOrWhiteSpace(game.Title))
            {
                throw new ArgumentException("O título do jogo é obrigatório.");
            }

            if (game.Price <= 0)
            {
                throw new ArgumentException("O preço do jogo deve ser maior que zero.");
            }

            return await _repo.UpdateAsync(game);
        }

        public async Task DeleteAsync(int id)
        {
            var game = await _repo.GetByIdAsync(id);
            if (game == null)
            {
                throw new ArgumentException("Jogo não encontrado.");
            }

            await _repo.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repo.ExistsAsync(id);
        }
    }
}
