using Microsoft.EntityFrameworkCore;
using Sso.Core.Data;
using Sso.Core.DTOs;
using Sso.Core.Entities;
using Sso.Core.Interfaces;

namespace Sso.Core.Services;

public class UserService(SsoDbContext db) : IUserService
{
    public Task<List<User>> GetAllAsync() =>
        db.Users.AsNoTracking().ToListAsync();

    public Task<User?> GetByIdAsync(Guid id) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByUserNameAndTenantAsync(string userName, string tenantName)
    {
        return await db.UserTenants
            .Include(ut => ut.User)
            .Include(ut => ut.Tenant)
            .Where(ut => ut.User.UserName == userName
                      && ut.Tenant.Name == tenantName
                      && ut.User.Status == Enums.Status.Active)
            .Select(ut => ut.User)
            .FirstOrDefaultAsync();
    }

    public async Task<User> CreateAsync(RegisterRequest request)
    {
        var existingUser = string.IsNullOrEmpty(request.TenantName)
            ? await db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName)
            : await GetByUserNameAndTenantAsync(request.UserName, request.TenantName);

        if (existingUser != null)
            throw new InvalidOperationException($"UserName '{request.UserName}' already exists");

        var user = new User
        {
            UserName = request.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name ?? string.Empty,
            Email = request.Email,
            Phone = request.Phone,
            Avatar = request.Avatar,
            Description = request.Description,
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(request.TenantName))
        {
            var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Name == request.TenantName);
            if (tenant != null)
            {
                db.UserTenants.Add(new UserTenant { UserId = user.Id, TenantId = tenant.Id });
                await db.SaveChangesAsync();
            }
        }

        return user;
    }

    public async Task<User?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return null;

        if (request.Name != null) user.Name = request.Name;
        if (request.Email != null) user.Email = request.Email;
        if (request.Phone != null) user.Phone = request.Phone;
        if (request.Avatar != null) user.Avatar = request.Avatar;
        if (request.Description != null) user.Description = request.Description;

        await db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ChangePasswordAsync(Guid id, string newPassword)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await db.SaveChangesAsync();
        return true;
    }
}
