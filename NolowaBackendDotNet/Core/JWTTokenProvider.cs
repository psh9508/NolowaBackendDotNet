using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.Configuration;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core
{
    public interface IJWTTokenProvider
    {
        public string GenerateJWTToken(AccountDTO account);
    }

    public class JWTTokenProvider : IJWTTokenProvider
    {
        private readonly JWT _jwtOption;

        public JWTTokenProvider(IOptions<JWT> jwtOption)
        {
            _jwtOption = jwtOption.Value;
        }

        public string GenerateJWTToken(AccountDTO account)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Email),
                //new Claim(ClaimTypes.Role, account.)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: "tempIssuer",
                audience: "tempAudience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
