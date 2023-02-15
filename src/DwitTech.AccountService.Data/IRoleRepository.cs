using System;

namespace.DwitTech.AccountService.Core.Interfaces
    public interface IRoleRepository 
{
    Task<Role> GetByIdAsync(int id);
    Task<List<Role>> GetAllAsync();
    Task AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(Role role);
}

