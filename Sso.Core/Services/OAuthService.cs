using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Sso.Core.Data;
using Sso.Core.DTOs;
using Sso.Core.Entities;
using Sso.Core.Helpers;
using Sso.Core.Interfaces;

namespace Sso.Core.Services;

public class OAuthService(SsoDbContext db, JwtHelper jwt) : IOAuthService
{
    public async Task<string> AuthorizeAsync(string clientId, string redirectUri, string state, Guid ssoUserId)
    {
        var client = await db.OAuthClients.FirstOrDefaultAsync(c => c.ClientId == clientId)
            ?? throw new UnauthorizedAccessException("client_id không hợp lệ.");

        var allowed = JsonSerializer.Deserialize<List<string>>(client.AllowedRedirectUris) ?? [];
        if (!allowed.Contains(redirectUri))
            throw new UnauthorizedAccessException("redirect_uri không được phép.");

        var code = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-").Replace("/", "_").Replace("=", "");

        db.AuthorizationCodes.Add(new AuthorizationCode
        {
            Code = code,
            SsoUserId = ssoUserId,
            ClientId = clientId,
            RedirectUri = redirectUri,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
        });
        await db.SaveChangesAsync();

        var separator = redirectUri.Contains('?') ? '&' : '?';
        return $"{redirectUri}{separator}code={Uri.EscapeDataString(code)}&state={Uri.EscapeDataString(state)}";
    }

    public async Task<TokenResponse> ExchangeCodeAsync(TokenRequest request)
    {
        var client = await db.OAuthClients.FirstOrDefaultAsync(c => c.ClientId == request.ClientId)
            ?? throw new UnauthorizedAccessException("client_id không hợp lệ.");

        if (!BCrypt.Net.BCrypt.Verify(request.ClientSecret, client.ClientSecretHash))
            throw new UnauthorizedAccessException("client_secret không đúng.");

        var authCode = await db.AuthorizationCodes.FirstOrDefaultAsync(c =>
            c.Code == request.Code && c.ClientId == request.ClientId && !c.IsUsed)
            ?? throw new UnauthorizedAccessException("Authorization code không hợp lệ hoặc đã dùng.");

        if (authCode.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Authorization code đã hết hạn.");

        if (authCode.RedirectUri != request.RedirectUri)
            throw new UnauthorizedAccessException("redirect_uri không khớp.");

        authCode.IsUsed = true;
        await db.SaveChangesAsync();

        var user = await db.Users.FindAsync(authCode.SsoUserId)
            ?? throw new UnauthorizedAccessException("User không tồn tại.");

        var userTenant = await db.UserTenants
            .Include(ut => ut.Tenant)
            .FirstOrDefaultAsync(ut => ut.UserId == user.Id);

        var tenant = userTenant?.Tenant;
        var accessToken = jwt.GenerateAccessToken(user, tenant);

        return new TokenResponse
        {
            AccessToken = accessToken,
            ExpiresIn = jwt.AccessTokenExpiresIn,
            SsoUserId = user.Id,
            TenantName = tenant?.Name ?? string.Empty,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Avatar = user.Avatar,
            Description = user.Description,
        };
    }
}
