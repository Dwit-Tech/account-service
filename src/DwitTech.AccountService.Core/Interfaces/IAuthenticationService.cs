using DwitTech.AccountService.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Core.Interfaces
{
    public interface IAuthenticationService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
