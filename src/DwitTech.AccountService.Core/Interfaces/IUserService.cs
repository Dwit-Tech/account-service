using DwitTech.AccountService.Core.Dtos;
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
        Task<Task<Models.TokenModel>> UserLogin(string email, string hashedPassword);
    }
}
