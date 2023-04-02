using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;
using Microsoft.EntityFrameworkCore;

namespace DwitTech.AccountService.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _dbContext;

        public UserRepository(AccountDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ValidationCode> GetUserValidationCode(string activationCode, int codeType)
        {
            var result = await _dbContext.ValidationCode.Where(x => x.Code == activationCode).FirstOrDefaultAsync();
            return result;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            return user;   
        }

        public async Task UpdateUser(User user)
        {
            _dbContext.Update(user);
             await _dbContext.SaveChangesAsync();
        }
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

        public async Task CreateUserLoginCredentials(UserLogin credentials)
        {
            _accountDbContext.UsersLogin.Add(credentials);
            await _accountDbContext.SaveChangesAsync();
        }
    }
}
