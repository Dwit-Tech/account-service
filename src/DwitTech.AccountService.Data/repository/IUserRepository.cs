using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
        Task<ValidationCode> SaveUserValidationCode(ValidationCode validation);
        Task<ValidationCode> GetUserValidationCode(string activationCode, CodeType codeType);
        Task<User> GetUser(int id);
        Task UpdateUser(User user);
    }
}