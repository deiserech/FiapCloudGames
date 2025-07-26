using System.Collections.Generic;
using System.Linq;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;

namespace FiapCloudGames.Infrastructure.Data
{
    public class ClienteRepository : IClienteRepository
    {
        private static readonly List<Cliente> _clientes = new();

        public Cliente GetById(int id)
        {
            return _clientes.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Cliente cliente)
        {
            cliente.Id = _clientes.Count + 1;
            _clientes.Add(cliente);
        }
    }
}