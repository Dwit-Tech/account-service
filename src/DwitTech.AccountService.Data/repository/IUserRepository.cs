namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
<<<<<<< HEAD
        Task<ValidationCode> GetUserActivationDetail(string activationCode);
        Task<bool> GetUserStatus(int id);
        Task<bool> ValidateUserActivationCodeExpiry(string activationCode);
        Task UpdateUserStatus(ValidationCode validationDetails);
=======
>>>>>>> 3f3714c76094d5c905c8d46d52bbc2c705b884e1
    }
    
}
