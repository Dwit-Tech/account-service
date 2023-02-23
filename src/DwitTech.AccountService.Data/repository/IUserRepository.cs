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
        Task<User> GetActivationDetail(string activationCode);
        Task<User> GetUser(int Id);
        Task<ActivationDetail> ActivationCodeExpiry(ActivationDetail codeDetail);
        Task<User> UpdateUserStatus(int Id);
        
        
    }
}
