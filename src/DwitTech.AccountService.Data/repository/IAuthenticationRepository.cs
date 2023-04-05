using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IAuthenticationRepository
    {
        Task<SessionToken> FindSessionByUserIdAsync(int userId);
        Task UpdateSessionTokenAsync(SessionToken sessiondetails);
        Task AddSessionAsync(SessionToken sessionDetails);
    }
}
