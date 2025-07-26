   using FiapCloudGames.Domain.Entities;
   using FiapCloudGames.Domain.Interfaces;
   
   namespace FiapCloudGames.Application.Services
   {
       public class ClienteService
       {
           private readonly IClienteRepository _repo;
   
           public ClienteService(IClienteRepository repo)
           {
               _repo = repo;
           }
   
           public Cliente ObterPorId(int id) => _repo.GetById(id);
   
           public void Criar(Cliente cliente) => _repo.Add(cliente);
       }
   }