using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Enum;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IActivationService
    {
       Task <bool> SendActivationEmail(int userId, string RecipientName, Email email, string templateName);
       Task<bool> ActivateUser(string activationCode);
    }
}