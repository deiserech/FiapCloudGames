using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface IUserService
    {
        User? ObterPorId(string id);
        void Criar(User user);
    }
}
