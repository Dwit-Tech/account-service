using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static DwitTech.AccountService.Data.Repository.IUserRepository;

namespace DwitTech.AccountService.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _dbContext;

        public UserRepository(AccountDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ValidationCode> GetActivationDetail(string activationCode)
        {
            var validationCode = await _dbContext.ValidationCodes.Where(x => x.Code == activationCode).FirstOrDefaultAsync();
            return validationCode;
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

        public async Task<bool> ValidateActivationCodeExpiry(string activationCode)
        {
            var validationCode = await GetActivationDetail(activationCode);

            DateTime ExpiredTime = validationCode.CreatedTime.AddMinutes(10);           

            if (DateTime.UtcNow > ExpiredTime)
            {
                return false;
            }
            return true;
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
