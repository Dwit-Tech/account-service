using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
        Task CreateUser(User user);

        Task CreateUserLoginCredentials(UserLogin credentials);
    }
}
