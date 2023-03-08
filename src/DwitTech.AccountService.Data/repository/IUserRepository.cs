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
        Task<ValidationCode> GetActivationDetail(string activationCode);
        Task<bool> GetUserStatus(int id);
        Task<bool> ValidateActivationCodeExpiry(string activationCode);
        Task UpdateUserStatus(ValidationCode validationDetails);
       
        public class ValidationCode
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Code { get; set; }
            public int Channel { get; set; }

            public int CodeType { get; set; }
            public DateTime CreatedTime { get; private set; } = DateTime.UtcNow;
        }
        
    }
    
}
