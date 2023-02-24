using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Repository
{


    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _accountDbContext;

        public UserRepository(AccountDbContext accountDbContext) 
        {
            _accountDbContext = accountDbContext;
        }
        public async Task<ValidationCode> SaveUserValidationCode(string userId, string validationCode)
        {

           var response = await
            _accountDbContext.ValidationCode.AddAsync(new ValidationCode { Code= validationCode, UserId= userId });
            return response.Entity;
        }
    }
}
