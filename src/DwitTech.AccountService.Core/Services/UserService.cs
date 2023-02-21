using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;

namespace DwitTech.AccountService.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }


        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _repository.FindByEmailAsync(email);
        }

        public async Task<bool> UpdateSessionTokenAsync(SessionToken sessionDetails)
        {
            return await _repository.UpdateSessionTokenAsync(sessionDetails);
        }

        public Task<SessionToken> GetSessionByUserIdAsync(int userId)
        {
            return _repository.FindSessionByUserIdAsync(userId);
        }

        public Task<bool> AddSessionAsync(SessionToken sessionDetails)
        {
            return _repository.AddSessionAsync(sessionDetails);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _repository.SaveChangesAsync();
        }
    }
}
