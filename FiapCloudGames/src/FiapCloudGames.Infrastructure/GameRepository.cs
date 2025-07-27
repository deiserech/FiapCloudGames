using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FiapCloudGames.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private static readonly List<Game> _games = new();
        private static int _nextId = 1;

        public void Add(Game game)
        {
            game.Id = _nextId++;
            _games.Add(game);
        }

        public Game? GetById(int id)
        {
            return _games.FirstOrDefault(g => g.Id == id);
        }

        public IEnumerable<Game> GetAll()
        {
            return _games;
        }
    }
}
