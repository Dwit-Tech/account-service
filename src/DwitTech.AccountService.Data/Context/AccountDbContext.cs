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
using static DwitTech.AccountService.Data.Repository.IUserRepository;

namespace DwitTech.AccountService.Data.Context
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ValidationCode> ValidationCode { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SessionToken> SessionTokens { get; set; }

        public DbSet<UserLogin> UserLogin { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.AddEntityConfigurations(assembly);
            base.OnModelCreating(builder);

            #region OwnedTypeSeed
            builder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Administrator Role" },
                new Role { Id = 2, Name = "User", Description = "User Role" });
            #endregion
        }
    }
}
