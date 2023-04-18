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

        public async Task<ValidationCode> GetUserValidationCode(string activationCode, CodeType codeType)
        {
            var result = await _dbContext.ValidationCodes.Where(x => x.Code == activationCode).FirstOrDefaultAsync();
            return result;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            return user;
        }

        public async Task UpdateUser(User user)
        {
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveUserValidationCode(ValidationCode validationCode)
        {
            await _dbContext.ValidationCodes.AddAsync(validationCode);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<int> CreateUser(User user)
        {
            await _dbContext.Users.AddAsync(user);
            _dbContext.Attach(user.Role);
            await _dbContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task CreateUserLogin(UserLogin credentials)
        {
            _dbContext.UserLogins.Add(credentials);
            await _dbContext.SaveChangesAsync();
        }
    }
}