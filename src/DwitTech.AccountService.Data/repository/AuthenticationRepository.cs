using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DwitTech.AccountService.Data.Repository
{
    public class AuthenticationRepository:IAuthenticationRepository
    {
        private readonly AccountDbContext _context;

        public AuthenticationRepository(AccountDbContext context)
        {
            _context = context;
        }


        public async Task<SessionToken> FindSessionByUserIdAsync(int userId)
        {
            return await _context.SessionTokens.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task UpdateSessionTokenAsync(SessionToken sessionDetails)
        {
            _context.Update(sessionDetails);
            await _context.SaveChangesAsync();
        }

        public async Task AddSessionAsync(SessionToken sessionDetails)
        {
            await _context.SessionTokens.AddAsync(sessionDetails);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            return user;
        }

        public async Task<bool> ValidateLogin(string email, string hashedPassword)
        {
            return await _context.UserLogins.AnyAsync(u => u.Username == email && u.Password == hashedPassword);
        }

        public async Task<int> DeleteSessionToken(int userId)
        {
            var user =await _context.SessionTokens.Where(x => x.UserId == userId).FirstAsync();
            _context.SessionTokens.Remove(user);
            return await _context.SaveChangesAsync();

        }
    }
}
