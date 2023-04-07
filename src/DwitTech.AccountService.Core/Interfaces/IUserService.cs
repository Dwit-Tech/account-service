using DwitTech.AccountService.Core.Dtos;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IUserService
    {
      
        Task<bool>CreateUser(UserDto user);
    }
}
