   using FiapCloudGames.Domain.Entities;
   
   namespace FiapCloudGames.Domain.Interfaces
   {
       public interface IUserRepository
       {
           User GetById(string id);
           void Add(User user);
       }
   }   