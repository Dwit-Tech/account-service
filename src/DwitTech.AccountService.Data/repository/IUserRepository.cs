using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
        Task<ValidationCode> GetUserActivationDetail(string activationCode);
        Task<bool> GetUserStatus(int id);
        Task<bool> ValidateUserActivationCodeExpiry(string activationCode);
        Task UpdateUserStatus(ValidationCode validationDetails);
    }

}
