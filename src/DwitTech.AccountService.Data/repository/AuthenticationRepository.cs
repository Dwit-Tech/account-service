using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.repository
{
    public class AuthenticationRepository:IAuthenticationRepository
    {
        private readonly AccountDbContext _context;

        public AuthenticationRepository(AccountDbContext context)
        {
            _context = context;
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
