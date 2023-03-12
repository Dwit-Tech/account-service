using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Repository
{
    public class AuthenticationRepository:IAuthenticationRepository
    {
        private readonly AccountDbContext _context;

        public AuthenticationRepository(AccountDbContext context)
        {
            _context = context;
        }


        public async Task<SessionToken?> FindSessionByUserIdAsync(int userId)
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
    }
}
