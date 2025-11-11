using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RegistroDeTickets.Service
{
    public  class TokenService
    {
        private readonly string _jwtkey;
        private readonly IDataProtector _protector;
        public TokenService(IConfiguration config,IDataProtectionProvider provider)
        {
            _jwtkey = config["Jwt:Key"];
            _protector = provider.CreateProtector("JwtTokenProtector");
        }

        public string GenerateToken(string username, IList<string> roles, int Id, IList<Claim> additionalClaims)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,username)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            claims.Add(new Claim("Id", Id.ToString()));

            if (additionalClaims != null && additionalClaims.Any())
            {
                claims.AddRange(additionalClaims);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtkey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "RegistroDeTickets.Web",
                audience: "RegistroDeTickets.Web",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return _protector.Protect(tokenString);
        }

        public string DecryptToken(string protectedToken)
        {
            try
            {
                return _protector.Unprotect(protectedToken);
            }
            catch
            {
                return null;
            }
        }



    }
}
