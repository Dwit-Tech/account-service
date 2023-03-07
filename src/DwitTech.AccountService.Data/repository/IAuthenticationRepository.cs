using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IAuthenticationRepository
    {
        Task<SessionToken?> FindSessionByUserIdAsync(int userId);
        Task UpdateSessionTokenAsync(SessionToken sessiondetails);
        Task AddSessionAsync(SessionToken sessionDetails);
    }
}
