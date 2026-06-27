using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sso.Core.Data;
using Sso.Core.DTOs;
using Sso.Core.Entities;
using Sso.Core.Helpers;
using Sso.Core.Interfaces;

namespace Sso.Core.Services;

public class UserService(SsoDbContext db, IConfiguration config) : IUserService
{
    private readonly string _salt = config["PasswordSalt"] ?? "gcrfigzhwm";
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
            PasswordHash = PasswordHelper.HashPassword(request.Password, _salt),
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

        user.PasswordHash = PasswordHelper.HashPassword(newPassword, _salt);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<SyncBatchResult> SyncBatchAsync(SyncBatchRequest request)
    {
        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Name == request.TenantName);
        if (tenant == null)
        {
            tenant = new Tenant { Name = request.TenantName };
            db.Tenants.Add(tenant);
            await db.SaveChangesAsync();
        }

        var result = new SyncBatchResult();

        foreach (var item in request.Users)
        {
            var existing = await db.UserTenants
                .Include(ut => ut.User)
                .FirstOrDefaultAsync(ut =>
                    ut.TenantId == tenant.Id &&
                    ut.User.UserName == item.UserName);

            if (existing != null)
            {
                // Update profile + password hash
                existing.User.PasswordHash = item.PasswordHash;
                if (!string.IsNullOrEmpty(item.Name)) existing.User.Name = item.Name;
                if (!string.IsNullOrEmpty(item.Email)) existing.User.Email = item.Email;
                if (!string.IsNullOrEmpty(item.Phone)) existing.User.Phone = item.Phone;
                if (!string.IsNullOrEmpty(item.Avatar)) existing.User.Avatar = item.Avatar;
                result.Updated++;
                result.Items.Add(new SyncUserResultItem
                {
                    UserName = item.UserName,
                    SsoUserId = existing.User.Id,
                    Action = "updated"
                });
            }
            else
            {
                // Check if username exists in another tenant
                var userByName = await db.Users.FirstOrDefaultAsync(u => u.UserName == item.UserName);
                if (userByName != null)
                {
                    // Same username different tenant — add UserTenant link
                    userByName.PasswordHash = item.PasswordHash;
                    db.UserTenants.Add(new UserTenant { UserId = userByName.Id, TenantId = tenant.Id });
                    result.Updated++;
                    result.Items.Add(new SyncUserResultItem
                    {
                        UserName = item.UserName,
                        SsoUserId = userByName.Id,
                        Action = "linked"
                    });
                }
                else
                {
                    // Create new user
                    var newUser = new User
                    {
                        UserName = item.UserName,
                        PasswordHash = item.PasswordHash,
                        Name = item.Name ?? string.Empty,
                        Email = item.Email,
                        Phone = item.Phone,
                        Avatar = item.Avatar,
                    };
                    db.Users.Add(newUser);
                    await db.SaveChangesAsync();

                    db.UserTenants.Add(new UserTenant { UserId = newUser.Id, TenantId = tenant.Id });
                    result.Created++;
                    result.Items.Add(new SyncUserResultItem
                    {
                        UserName = item.UserName,
                        SsoUserId = newUser.Id,
                        Action = "created"
                    });
                }
            }
        }

        await db.SaveChangesAsync();
        return result;
    }
}
