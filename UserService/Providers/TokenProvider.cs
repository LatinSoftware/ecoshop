using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using UserService.Entities;


namespace UserService.Providers
{
    public sealed class TokenProvider(IConfiguration configuration)
    {
        public string Create(User user)
        {
            var secretKey = configuration["Jwt:Secret"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity
                (
                    [
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.Email.Value),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
                        new Claim(JwtRegisteredClaimNames.Name, user.Name),
                        new Claim(ClaimTypes.Role, user.Role.ToString().ToLower())
                    ]
                ),
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = "https://localhost:0",
            };

           

            var handler = new JsonWebTokenHandler();
            
            string token = handler.CreateToken(tokenDescriptor);
            
            return token;
        }
    }
}
