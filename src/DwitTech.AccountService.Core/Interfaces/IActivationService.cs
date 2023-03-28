using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IActivationService
    {
       Task <bool> SendActivationEmail(int userId, string templateName, string RecipientName, Email email);
        Task<bool> ActivateUser(string activationCode);
    }
}