using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
        Task<ValidationCode> SaveUserValidationCode(ValidationCode validation);
        Task<ValidationCode> GetUserValidationCode(string activationCode, int codeType);
        Task<User> GetUser(int id);
        Task UpdateUser(User user);
    }
}