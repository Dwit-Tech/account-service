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
        public async Task<ValidationCode> SaveUserValidationCode(ValidationCode validationCode)
        {
            var response = await _accountDbContext.ValidationCode.AddAsync(validationCode);
            await _accountDbContext.SaveChangesAsync();
            return response.Entity;
        }
    }
}