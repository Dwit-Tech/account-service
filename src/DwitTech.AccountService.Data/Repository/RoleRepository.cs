using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DwitTech.AccountService.Data.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AccountDbContext _accountDbContext;
        public RoleRepository(AccountDbContext accountDbContext)
        {

            _accountDbContext = accountDbContext;   

        }
        public async Task<IEnumerable<Role>>GetRoles()
        {
            return await _accountDbContext.Roles.ToListAsync();
        }
    }
}
