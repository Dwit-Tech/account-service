using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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

        public async Task<ValidationCode> FindUserValidationCode(int userId, CodeType codeType)
        {
            var result = await _dbContext.ValidationCodes.Where(x => x.UserId == userId && x.CodeType == codeType).FirstOrDefaultAsync();
            return result;
        }

        public async Task UpdateValidationCode(ValidationCode validationCode)
        {
            _dbContext.Update(validationCode);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetUser(int id)
        {
            var user = await GetActiveUsers().FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }

        public async Task<User> GetUserByEmail(string userEmail)
        {
            var user = await GetActiveUsers().Where(x => x.Email == userEmail).FirstOrDefaultAsync();
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

        public async Task UpdateUserLoginAsync(User user, string newPasswordHash)
        {
            var login = await _dbContext.UserLogins
            .Where(l => l.Username == user.Email && l.UserId == user.Id)
            .Select(l => new UserLogin
            {
                Id = l.Id,
                UserId = l.UserId,
                Username = l.Username,
                ModifiedOnUtc = DateTime.UtcNow,
                Password = newPasswordHash
            })
            .FirstOrDefaultAsync();

            if (login != null)
            {
                _dbContext.UserLogins.Attach(login);
                _dbContext.Entry(login).Property(x => x.Password).IsModified = true;
                _dbContext.Entry(login).Property(x => x.ModifiedOnUtc).IsModified = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUser(id);
            if (user != null)
            {
                user.Status = UserStatus.Deleted;
                await _dbContext.SaveChangesAsync();
            }
        }

        private IQueryable<User> GetActiveUsers()
        {
            return _dbContext.Users.Where(x => x.Status != UserStatus.Deleted);
        }

        public bool FindPasswordResetToken(string token)
        {
            var tokenExists = _dbContext.ValidationCodes.Any(x => x.Code == token);
            return tokenExists;
        }

        public async Task<int> GetUserIdByPasswordResetToken(string token)
        {
            var result = await _dbContext.ValidationCodes.Where(x => x.Code == token).FirstOrDefaultAsync();
            return result.UserId;
        }

        public async Task UpdateUserLoginsPassword(UserLogin userLogin)
        {
            _dbContext.UserLogins.Update(userLogin);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserLogin> GetUserLoginsByUserId(int userId)
        {
            var result = await _dbContext.UserLogins.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            return result;
        }
    }
}

