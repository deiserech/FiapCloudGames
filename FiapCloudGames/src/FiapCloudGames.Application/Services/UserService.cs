   using FiapCloudGames.Domain.Entities;
   using FiapCloudGames.Domain.Interfaces;
   
   namespace FiapCloudGames.Application.Services
   {
       public class UserService
       {
           private readonly IUserRepository _repo;
   
           public UserService(IUserRepository repo)
           {
               _repo = repo;
           }
   
           public User ObterPorId(string id) => _repo.GetById(id);
   
           public void Criar(User user) => _repo.Add(user);
       }
   }