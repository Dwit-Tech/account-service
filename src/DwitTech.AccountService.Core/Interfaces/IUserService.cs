using DwitTech.AccountService.Core.Dtos;
using DwitTech.AccountService.Core.Models;
using DwitTech.AccountService.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IUserService
    {
        Task<TokenModel> AuthenticateUserLogin(string email, string hashedPassword);
    }
}

