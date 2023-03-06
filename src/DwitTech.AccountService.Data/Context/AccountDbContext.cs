using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Data.Context
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.AddEntityConfigurations(assembly);
            base.OnModelCreating(builder);

            #region OwnedTypeSeed
            builder.Entity<Role>().OwnsOne(r => r.Roles).HasData(
                new { RoleId = 1, Name = "", Description = "" },
                new { RoleId = 2, Name = "", Description = "" });
            #endregion
        }
    }
}
