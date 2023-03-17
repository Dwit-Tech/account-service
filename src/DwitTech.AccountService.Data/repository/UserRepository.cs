using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
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

        public async Task<ValidationCode> GetUserActivationDetail(string activationCode)
        {
            return await _dbContext.ValidationCode.Where(x => x.Code == activationCode).FirstOrDefaultAsync();
           
        }

        public async Task<bool> GetUserStatus(int id)
        {
            var status = await _dbContext.Users.Where(x => x.Id == id).Select(x => x.Status).FirstOrDefaultAsync();
            if (status == UserStatus.Inactive)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ValidateUserActivationCodeExpiry(string activationCode)
        {
            var validationCode = await GetUserActivationDetail(activationCode);

            DateTime expiredTime = validationCode.CreatedOnUtc.AddMinutes(10);

            if (DateTime.UtcNow > expiredTime)
            {
                return true;
            }
            return false;
        }


        public async Task UpdateUserStatus(ValidationCode validationDetails)
        {

            var userStatus = await GetUserStatus(validationDetails.UserId);

            if (userStatus)
            {
                var user = await _dbContext.Users.Where(x => x.Id == validationDetails.UserId).FirstOrDefaultAsync();
                user.Status = UserStatus.Active;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();

            }
        }
    }
}
