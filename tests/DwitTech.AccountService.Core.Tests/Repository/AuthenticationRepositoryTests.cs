using DwitTech.AccountService.Core.Utilities;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DwitTech.AccountService.Core.Tests.Repository
{
    public class AuthenticationRepositoryTests : IDisposable
    {
        private readonly AccountDbContext _accountDbContext;
        private Mock<IAuthenticationRepository> mockAuthRepo;
        private const string validRefreshToken = "HC2MBsmq7zRY3iWc3jvkjukYu/jdYab9c5+MENq2+RQ=";

        public AuthenticationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AccountDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            _accountDbContext = new AccountDbContext(options);
            _accountDbContext.Database.EnsureCreated();

            _accountDbContext.SaveChanges();

            mockAuthRepo = new Mock<IAuthenticationRepository>();
        }

        
        [Fact]
        public async Task UpdateSessiontokenAsync_UpdatesSessiondetailsToDb_WhenSessionIsValid()
        {
            //Arrange
            var sessionDetails = new SessionToken
            {
                UserId = 1,
                RefreshToken = StringUtil.HashString(validRefreshToken),
                ModifiedOnUtc = DateTime.UtcNow
            };

            var existingSession = new SessionToken
            {
                UserId = 1,
                RefreshToken = "old refresh token",
                ModifiedOnUtc = DateTime.UtcNow
            };

            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var accountDbContext = new AccountDbContext(options);

            await accountDbContext.SessionTokens.AddAsync(existingSession);
            await accountDbContext.SaveChangesAsync();

            var authRepo = new AuthenticationRepository(accountDbContext);

            //Act
            existingSession.RefreshToken = sessionDetails.RefreshToken;
            await authRepo.UpdateSessionTokenAsync(existingSession);

            //Assert
            var updatedSession = await accountDbContext.SessionTokens.FirstOrDefaultAsync(x => x.UserId == sessionDetails.UserId);

            Assert.NotNull(updatedSession);
            Assert.Equal(sessionDetails.RefreshToken, updatedSession.RefreshToken);
        }


        [Fact]
        public async Task FindSessionAsync_Returns_Session_When_SessionExists()
        {
            //Arrange
            var sessionDetails = new SessionToken
            {
                UserId = 1,
                RefreshToken = StringUtil.HashString(validRefreshToken),
                ModifiedOnUtc = DateTime.UtcNow
            };

            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var accountDbContext = new AccountDbContext(options);

            await accountDbContext.SessionTokens.AddAsync(sessionDetails);
            await accountDbContext.SaveChangesAsync();

            var authRepo = new AuthenticationRepository(accountDbContext);

            //Act
            var result = await authRepo.FindSessionByUserIdAsync(sessionDetails.UserId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(sessionDetails, result);
        }

        [Fact]
        public async Task FindSessionAsync_Returns_Null_When_SessionDoesNotExist()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var accountDbContext = new AccountDbContext(options);

            var authRepo = new AuthenticationRepository(accountDbContext);

            //Act
            var result = await authRepo.FindSessionByUserIdAsync(1);

            //Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task AddSessionAsync_AddsSessionToDb_WhenSessionIsValid()
        {
            //Arrange
            var sessionDetails = new SessionToken
            {
                UserId = 1,
                RefreshToken = StringUtil.HashString(validRefreshToken),
                ModifiedOnUtc = DateTime.UtcNow
            };

            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var accountDbContext = new AccountDbContext(options);

            var authRepo = new AuthenticationRepository(accountDbContext);

            //Act
            await authRepo.AddSessionAsync(sessionDetails);

            //Assert
            var addedSession = await accountDbContext.SessionTokens.FindAsync(sessionDetails.UserId);
            Assert.NotNull(addedSession);
            Assert.Equal(sessionDetails, addedSession);
        }        

        public void Dispose()
        {
            _accountDbContext.Database.EnsureDeleted();
            _accountDbContext.Dispose();
        }
    }
}
