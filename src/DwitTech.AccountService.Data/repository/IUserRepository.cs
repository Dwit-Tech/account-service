using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Data.repository
{
    public interface IUserRepository
    {
        Task<User> FindByEmailAsync(string email);
    }
}
