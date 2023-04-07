using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace DwitTech.AccountService.Data.Tests.Repository
{
    public class AuthenticationRepositoryTests : IDisposable
    {
        private const string validHashedRefreshToken = "f9f4ac2743d99de85f02a1af00d9dca4";

        [Fact]
        public async Task UpdateSessiontokenAsync_UpdatesSessiondetailsToDb_WhenSessionIsValid()
        {
            //Arrange
            var sessionDetails = new SessionToken
            {
                UserId = 1,
                RefreshToken = validHashedRefreshToken,
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

            using (var accountDbContext = new AccountDbContext(options))
            {
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
        }


        [Fact]
        public async Task FindSessionAsync_Returns_Session_When_SessionExists()
        {
            //Arrange
            var sessionDetails = new SessionToken
            {
                UserId = 1,
                RefreshToken = validHashedRefreshToken,
                ModifiedOnUtc = DateTime.UtcNow
            };

            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var accountDbContext = new AccountDbContext(options))
            {
                await accountDbContext.SessionTokens.AddAsync(sessionDetails);
                await accountDbContext.SaveChangesAsync();

                var authRepo = new AuthenticationRepository(accountDbContext);

                //Act
                var result = await authRepo.FindSessionByUserIdAsync(sessionDetails.UserId);

                //Assert
                Assert.NotNull(result);
                Assert.Equal(sessionDetails, result);
            }
        }


        [Fact]
        public async Task FindSessionAsync_Returns_Null_When_SessionDoesNotExist()
        {
            //Arrange
            var nonExistingId = 1;

            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var accountDbContext = new AccountDbContext(options))
            {
                var authRepo = new AuthenticationRepository(accountDbContext);

                //Act
                var result = await authRepo.FindSessionByUserIdAsync(nonExistingId);

                //Assert
                Assert.Null(result);
            }            
        }


        [Fact]
        public async Task AddSessionAsync_AddsSessionToDb_WhenSessionIsValid()
        {
            //Arrange
            var sessionDetails = new SessionToken
            {
                UserId = 1,
                RefreshToken = validHashedRefreshToken,
                ModifiedOnUtc = DateTime.UtcNow
            };

            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var accountDbContext = new AccountDbContext(options))
            {
                var authRepo = new AuthenticationRepository(accountDbContext);

                //Act
                await authRepo.AddSessionAsync(sessionDetails);

                //Assert
                var addedSession = await accountDbContext.SessionTokens.FindAsync(sessionDetails.UserId);
                Assert.NotNull(addedSession);
                Assert.Equal(sessionDetails, addedSession);
            }
        }

        [Fact]
        public async Task ValidateLogin_Returns_BooleanResult()
        {
            //Arrange
            UserLogin mockLogin = new()
            {
                Username = "hello@support.com",
                Password = "shwy736"
            };

            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var accountDbContext = new AccountDbContext(options);

            await accountDbContext.UserLogins.AddAsync(mockLogin);
            await accountDbContext.SaveChangesAsync();

            var authRepo = new AuthenticationRepository(accountDbContext);

            //Act
            var result = await authRepo.ValidateLogin(mockLogin.Username, mockLogin.Password);

            //Assert
            Assert.IsType<bool>(result);

        }

        public void Dispose()
        {
        }
    }
}
