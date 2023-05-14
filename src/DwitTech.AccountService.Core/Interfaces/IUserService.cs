using DwitTech.AccountService.Core.Dtos;


namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IUserService
    {
        Task<bool>CreateUser(UserDto user);
        Task<bool> LogoutUser(string authHeader);
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
        Task DeleteUserAsync(int id);
        Task<bool> EditAccount(EditRequestDto editDto);
        Task<bool> ResetPassword(string userEmail);
    }
}

