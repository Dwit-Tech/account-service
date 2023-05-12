using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
        Task SaveUserValidationCode(ValidationCode validation);
        Task<ValidationCode> GetUserValidationCode(string activationCode, CodeType codeType);
        Task<User> GetUser(int id);
        Task UpdateUser(User user);
        Task<int> CreateUser(User user);
        Task CreateUserLogin(UserLogin credentials);
        Task UpdateUserLoginAsync(User user, string newPasswordHash);
        Task<User> GetUserByEmail(string userEmail);
        Task DeleteUserAsync(int id);
        Task<ValidationCode> FindUserValidationCode(int userId, CodeType codeType);
        Task UpdateValidationCode(ValidationCode validationCode);
    }

}
