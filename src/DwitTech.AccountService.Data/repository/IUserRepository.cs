using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
        Task<User> FindByEmailAsync(string email);
    }
}
