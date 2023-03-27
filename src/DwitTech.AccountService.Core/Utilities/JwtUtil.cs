using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
    }
}
