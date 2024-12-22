using MindvizServer.Application.Interfaces;
using MindvizServer.Core.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MindvizServer.Application.Services
{
    public class JwtAuthService:IAuth
    {
        public string GenerateToken(User user)
        {
            Claim[] claims = new Claim[]
       {
            new Claim("_id", user.Id),
            new Claim(ClaimTypes.Role, user.Role.ToString()), // Use Role claim
       };

            var secretKey = Environment.GetEnvironmentVariable("JwtSecretKey"); 

            
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT secret key not found in environment variables.");
            }

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); SigningCredentials credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: "MindvizServer",
              //audience: "MindvizWebApp",
               expires: DateTime.Now.AddDays(2),
                claims: claims,
                signingCredentials:credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
