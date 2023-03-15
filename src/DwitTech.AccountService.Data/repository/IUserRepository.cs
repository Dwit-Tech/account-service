namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
        Task<ValidationCode> SaveUserValidationCode(string userId, string validationCode);
    }
}
