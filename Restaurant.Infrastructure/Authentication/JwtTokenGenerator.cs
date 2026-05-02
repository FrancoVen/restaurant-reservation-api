using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.Common.Settings;
using Restaurant.Application.Interfaces.Authentication;
using Restaurant.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Restaurant.Infrastructure.Authentication
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly IDateTimerProvider _dateTimerProvider;

        public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings, IDateTimerProvider dateTimerProvider)
        {
            this._jwtSettings = jwtSettings;
            this._dateTimerProvider = dateTimerProvider;
        }

        public (string, DateTime) GenerateToken(User user)
        {
            var keybytes = Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret);

            var securityKey = new SymmetricSecurityKey(keybytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var expires = _dateTimerProvider.UtcNow.AddMinutes(_jwtSettings.Value.ExpiryMinutes);

            var token = new JwtSecurityToken
            (
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);

        }
    }
}
