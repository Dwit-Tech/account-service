using DwitTech.AccountService.Core.Interfaces;
using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;
using DwitTech.AccountService.Data.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DwitTech.AccountService.WebApi.Controllers
{
    [Route("api/dummy")]
    [ApiController]
    public class TestLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;
        private readonly AccountDbContext _context;

        public TestLoginController(IConfiguration configuration, IUserService userService, IAuthenticationService authService, AccountDbContext context)
        {
            _configuration = configuration;
            _userService = userService;
            _authService = authService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginModel userLogin)
        {
            var user = await Authenticate(userLogin);

            if (user != null)
            {
                var token = _authService.GenerateAccessToken(user);
                var refreshToken = _authService.GenerateRefreshToken();

                //var RefreshToken = refreshToken;
                //var RefreshTokenExpiryTime = DateTime.SpecifyKind(DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:RefreshTokenExpiryTime"])), DateTimeKind.Utc);

                var currentUserSession = await _userService.GetSessionByUserIdAsync(user.Id);

                if (currentUserSession != null)
                {
                    currentUserSession.RefreshToken = refreshToken;
                    currentUserSession.ModifiedOnUtc = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    await _userService.UpdateSessionTokenAsync(currentUserSession);
                }
                else
                {                 
                    var newUserSession = new SessionToken
                    {
                        Id = 1,
                        UserId = user.Id,
                        RefreshToken = refreshToken,
                        //RefreshTokenExpiryTime = RefreshTokenExpiryTime
                    };

                    await _userService.AddSessionAsync(newUserSession);
                }              

                return Ok(new TokenModel
                {
                    accessToken = token,
                    refreshToken = refreshToken
                });
            }
            return NotFound("User not found!");
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenModel tokenModel)
        {
            var accessToken = tokenModel.accessToken;
            var tokenHandler = new JwtSecurityTokenHandler();
            var userTokenObject = tokenHandler.ReadJwtToken(accessToken);
            var principal = _authService.GetPrincipalFromExpiredToken(accessToken);
            var user = await _userService.GetUserByEmailAsync(principal.FindFirstValue(ClaimTypes.Email));

            var session = await _userService.GetSessionByUserIdAsync(user.Id);

            if (session.ModifiedOnUtc.Value.AddHours(int.Parse(_configuration["Jwt:JwtTokenExpiryTime"])) < DateTime.SpecifyKind(userTokenObject.ValidTo, DateTimeKind.Utc))
            {
                var result = await _authService.GenerateAccessTokenfromRefreshToken(tokenModel);
                return Ok(result);
            }
            return BadRequest("Token is not expired");
        }

        [AllowAnonymous]
        [HttpGet("validate")]
        public IActionResult Validate([FromBody] TokenModel tokenModel)
        {
            var response = _authService.ValidateAccessToken(tokenModel.accessToken);
            return Ok(response);
        }


        //Test Auth Method
        private async Task<User> Authenticate(UserLoginModel userLogin)
        {
            var currentUser = await _context.Users.Where(o => o.Email.ToLower() == userLogin.Email.ToLower() && o.Password == userLogin.Password).FirstOrDefaultAsync();

            if (currentUser != null)
            {
                return currentUser;
            }

            return null;
        }
    }
}
