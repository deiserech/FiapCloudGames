using FiapCloudGames.Domain.Entities;
using System.Collections.Generic;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface IGameRepository
    {
        void Add(Game game);
        Game? GetById(int id);
        IEnumerable<Game> GetAll();
    }
}
