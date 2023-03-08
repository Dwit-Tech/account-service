using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IActivationService
    {
        
        Task<bool> ActivateUser(string activationCode, string fromEmail, string toEmail, string templateName, string subject = "Account Details", string cc = "", string bcc = "");
    }
}
