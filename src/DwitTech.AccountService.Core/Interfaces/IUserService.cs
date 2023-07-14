using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Data.Entities;

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
        Task<bool> UpdatePassword(int userId, PasswordResetModel passwordResetModel);
        Task<bool> HandlePasswordReset(string token, PasswordResetModel passwordResetModel);
    }
}

