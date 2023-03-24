using DwitTech.AccountService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IActivationService
    {
        Task <bool> SendActivationEmail(int userId, string templateName, string RecipientName, Email email);
    }
}
