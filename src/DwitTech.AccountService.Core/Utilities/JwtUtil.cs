﻿using Microsoft.Extensions.Configuration;
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
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: null,
                claims,
                expires: DateTime.Now.AddMinutes(int.Parse(configuration["Jwt:JwtTokenExpiryTime"])),
                signingCredentials: credentials);

            // Serialize the token to a string
            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }
    }
}