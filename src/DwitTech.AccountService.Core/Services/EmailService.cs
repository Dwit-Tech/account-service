﻿using DwitTech.AccountService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEmail(string userId, string FromEmail, string ToEmail, string Body, string Subject, string CC, string BCC)
        {
            throw new NotImplementedException();
        }
    }
}
