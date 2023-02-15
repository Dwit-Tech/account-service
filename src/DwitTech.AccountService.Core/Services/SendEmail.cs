using DwitTech.AccountService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Services
{
    public class SendEmail : IEmailService
    {
        public SendEmail() { }

        bool IEmailService.SendEmail(string From, string To, string Subject, string Body, string CC, string BCC)
        {
            throw new NotImplementedException();
        }
    }

}
