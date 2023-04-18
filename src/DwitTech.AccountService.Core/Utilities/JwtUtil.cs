using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DwitTech.AccountService.Core.Utilities
{
    public class JwtUtil
    {
        public static string GenerateJwtToken(List<Claim> claims, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT_KEY"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JWT_ISSUER"],
                audience: null,
                claims,
                expires: DateTime.Now.AddMinutes(int.Parse(configuration["JWT_TOKEN_EXPIRY_MINUTES"])),
                signingCredentials: credentials);

            // Serialize the token to a string
            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }

        public static string GenerateIdFromToken(string authHeader)
        {
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                try
                {
                    string bearerToken = authHeader.Substring("Bearer ".Length);
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(bearerToken.Trim());
                    var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "UserId");
                    var userId = userIdClaim.Value;
                    return userId = userIdClaim.Value;
                }
                catch (ArgumentNullException ex)
                {
                    throw new ArgumentNullException("Authorization Header is invalid",ex);
                }
            }
            else
            {
                throw new Exception("Authorization Header Not Supplied");
            }
        }
    }
}
