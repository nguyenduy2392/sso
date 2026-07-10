using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sso.Core.Entities;

namespace Sso.Core.Helpers;

public class JwtHelper(IConfiguration config)
{
    public string GenerateAccessToken(User user, Tenant? tenant = null)
    {
        var secret = config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret is not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresSeconds = GetAccessTokenSeconds();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (tenant != null)
        {
            claims.Add(new Claim("tenantId", tenant.Id.ToString()));
            claims.Add(new Claim("tenantName", tenant.Name));
        }

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expiresSeconds),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public int AccessTokenExpiresIn => GetAccessTokenSeconds();

    private int GetAccessTokenSeconds()
    {
        if (int.TryParse(config["Jwt:AccessTokenSeconds"], out var s) && s > 0) return s;
        if (int.TryParse(config["Jwt:AccessTokenMinutes"], out var m) && m > 0) return m * 60;
        return 3600;
    }
}
