using Microsoft.EntityFrameworkCore;
using Sso.Core.Data;
using Sso.Core.DTOs;
using Sso.Core.Entities;
using Sso.Core.Enums;
using Sso.Core.Helpers;
using Sso.Core.Interfaces;

namespace Sso.Core.Services;

public class AuthService(SsoDbContext db, JwtHelper jwt) : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Name == request.TenantName && t.Status == Status.Active)
            ?? throw new UnauthorizedAccessException("Tenant không tồn tại hoặc đã bị khoá.");

        var userTenant = await db.UserTenants
            .Include(ut => ut.User)
            .FirstOrDefaultAsync(ut =>
                ut.TenantId == tenant.Id &&
                ut.Status == Status.Active &&
                ut.User.UserName == request.UserName &&
                ut.User.Status == Status.Active)
            ?? throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, userTenant.User.PasswordHash))
            throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng.");

        return await IssueTokensAsync(userTenant.User, tenant);
    }

    public async Task<LoginResponse> RefreshAsync(RefreshRequest request)
    {
        var stored = await db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken && !r.IsRevoked)
            ?? throw new UnauthorizedAccessException("Refresh token không hợp lệ hoặc đã hết hạn.");

        if (stored.ExpiresAt < DateTime.UtcNow)
        {
            stored.IsRevoked = true;
            await db.SaveChangesAsync();
            throw new UnauthorizedAccessException("Refresh token đã hết hạn.");
        }

        stored.IsRevoked = true;

        // Lấy lại tenant từ claim trong token cũ (không lưu tenantId trong RefreshToken)
        // Tạm thời giữ nguyên tenant từ lần login gần nhất qua bảng UserTenants
        var userTenant = await db.UserTenants
            .Include(ut => ut.Tenant)
            .Where(ut => ut.UserId == stored.UserId && ut.Status == Status.Active)
            .OrderByDescending(ut => ut.CreatedAt)
            .FirstOrDefaultAsync();

        await db.SaveChangesAsync();

        return await IssueTokensAsync(stored.User, userTenant?.Tenant);
    }

    public async Task LogoutAsync(Guid userId, string refreshToken)
    {
        var token = await db.RefreshTokens
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Token == refreshToken);

        if (token != null)
        {
            token.IsRevoked = true;
            await db.SaveChangesAsync();
        }
    }

    private async Task<LoginResponse> IssueTokensAsync(User user, Tenant? tenant = null)
    {
        var refreshTokenValue = jwt.GenerateRefreshToken();
        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
        });
        await db.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = jwt.GenerateAccessToken(user, tenant),
            RefreshToken = refreshTokenValue,
            ExpiresIn = jwt.AccessTokenExpiresIn,
            UserId = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Avatar = user.Avatar,
            Description = user.Description,
            TenantId = tenant?.Id ?? Guid.Empty,
            TenantName = tenant?.Name ?? string.Empty,
        };
    }
}
