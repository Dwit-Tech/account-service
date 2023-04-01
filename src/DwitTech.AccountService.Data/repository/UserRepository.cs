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
        public async Task CreateUser(User user)
        {
             await _accountDbContext.Users.AddAsync(user);
            _accountDbContext.Attach(user.Roles);
            await _accountDbContext.SaveChangesAsync();

            

        }

        public Task CreateUserLoginCredentials(UserLogin credentials)
        {
            
        }
    }
}
