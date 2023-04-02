using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;

using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
        Task<ValidationCode> GetUserValidationCode(string activationCode, int codeType);
        Task<User> GetUser(int id);
        Task UpdateUser(User user);
        Task CreateUser(User user);

        Task CreateUserLoginCredentials(UserLogin credentials);
    }

}
