using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.AccountService.Data.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AccountDbContext _accountDbContext;
        public RoleRepository(AccountDbContext accountDbContext)
        {

            _accountDbContext = accountDbContext;   

        }
        public async Task<IEnumerable<Role>> GetRoles()
        {
            return _accountDbContext.Roles.ToList();
        }
    }
}
