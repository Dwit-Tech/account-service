using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);        
        Task<bool> UpdateSessionTokenAsync(SessionToken sessionDetails);
        Task<SessionToken> GetSessionByUserIdAsync(int userId);
        Task<bool> AddSessionAsync(SessionToken sessionDetails);
        //Task<bool> SaveChangesAsync();
    }
}
