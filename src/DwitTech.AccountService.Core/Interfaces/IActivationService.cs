using DwitTech.AccountService.Core.Models;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IActivationService
    {
       Task <bool> SendActivationEmail(int userId, string RecipientName, Email email, string templateName);
       Task<bool> ActivateUser(string activationCode);
    }
}