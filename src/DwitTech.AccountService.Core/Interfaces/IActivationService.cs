using DwitTech.AccountService.Core.Models;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IActivationService
    {
       Task <bool> SendActivationEmail(int userId, string recipientName, Email email, string templateName);
       Task<bool> ActivateUser(string activationCode);
    }
}