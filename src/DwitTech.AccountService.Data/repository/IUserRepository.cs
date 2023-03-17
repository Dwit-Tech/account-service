using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Repository
{
    public interface IUserRepository
    {
        Task<ValidationCode> GetUserActivationDetail(string activationCode);
        Task<bool> GetUserStatus(int id);
        Task<bool> ValidateUserActivationCodeExpiry(string activationCode);
        Task UpdateUserStatus(ValidationCode validationDetails);
    }
    
}
