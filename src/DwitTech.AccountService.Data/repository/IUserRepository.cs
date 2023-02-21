using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
        Task<bool> SaveChangesAsync();
        Task<User> FindByEmailAsync(string email);
        Task<SessionToken> FindSessionByUserIdAsync(int userId);
        Task<bool> UpdateSessionTokenAsync(SessionToken sessionDetails);
        Task<bool> AddSessionAsync(SessionToken sessionDetails);

    }
}
