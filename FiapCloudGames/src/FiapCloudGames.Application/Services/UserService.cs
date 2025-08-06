using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repositories;
using FiapCloudGames.Domain.Interfaces.Services;
using System;

namespace FiapCloudGames.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public User? GetByIdAsync(string id) => _repo.GetById(id);

        public void CreateUserAsync(User user) => _repo.Add(user);
    }
}