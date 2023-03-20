using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _accountDbContext;
        public UserRepository(AccountDbContext accountDbContext)
        {
                _accountDbContext = accountDbContext;
        }
        public async Task<User> CreateUser(User user)
        {
             _accountDbContext.Add(user);
           await _accountDbContext.SaveChangesAsync();
            return user;
        }

        
    }
}
