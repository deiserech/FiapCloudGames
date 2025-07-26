   using FiapCloudGames.Domain.Entities;
   
   namespace FiapCloudGames.Domain.Interfaces
   {
       public interface IClienteRepository
       {
           Cliente GetById(int id);
           void Add(Cliente cliente);
       }
   }   