using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DwitTech.AccountService.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _context;

        public UserRepository(AccountDbContext context)
        {
            _context = context;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            var user = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            return user;
        }

        public async Task<bool> UpdateSessionTokenAsync(SessionToken sessionDetails)
        {
            // Update the properties of the existing entity with the new values

            var existingSession = await _context.SessionTokens.Where(x => x.UserId == sessionDetails.UserId).FirstOrDefaultAsync();

            if (existingSession == null)
            {
                throw new ArgumentException("Entity does not exist.");
            }

            existingSession.RefreshToken = sessionDetails.RefreshToken;

            return await SaveChangesAsync();
        }

        public async Task<SessionToken> FindSessionByUserIdAsync(int userId)
        {
            var session = await _context.SessionTokens.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            return session;
        }

        public async Task<bool> AddSessionAsync(SessionToken sessionDetails)
        {
            await _context.SessionTokens.AddAsync(sessionDetails);
            await _context.SaveChangesAsync();

            return await SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
