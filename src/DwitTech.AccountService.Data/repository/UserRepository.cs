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
    }
}
