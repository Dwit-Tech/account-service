using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Models;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IActivationService
    {
        bool SendActivationEmail(string fromEmail, string toEmail, string templateName, string RecipientName, string subject, string cc, string bcc);
        Task<bool> ActivateUser(string activationCode);
        Task<bool> SendActivationEmail(int userId, string templateName, string RecipientName, Email email);  
    }
}
