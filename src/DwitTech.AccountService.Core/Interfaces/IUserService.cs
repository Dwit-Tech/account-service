namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IUserService
    {
      
        Task CreateUser(UserDto user);
    }
}
