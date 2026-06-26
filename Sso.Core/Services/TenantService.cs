using Microsoft.EntityFrameworkCore;
using Sso.Core.Data;
using Sso.Core.DTOs;
using Sso.Core.Entities;
using Sso.Core.Enums;
using Sso.Core.Interfaces;

namespace Sso.Core.Services;

public class TenantService(SsoDbContext db) : ITenantService
{
    public Task<List<Tenant>> GetAllAsync() =>
        db.Tenants.AsNoTracking().ToListAsync();

    public Task<Tenant?> GetByIdAsync(Guid id) =>
        db.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Tenant> CreateAsync(CreateTenantRequest request)
    {
        if (await db.Tenants.AnyAsync(t => t.Name == request.Name))
            throw new InvalidOperationException($"Tenant '{request.Name}' đã tồn tại.");

        var tenant = new Tenant { Name = request.Name };
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();
        return tenant;
    }

    public async Task AddUserAsync(Guid tenantId, Guid userId, string role = "Member")
    {
        if (await db.UserTenants.AnyAsync(ut => ut.TenantId == tenantId && ut.UserId == userId))
            throw new InvalidOperationException("User already belongs to this tenant");

        var parsedRole = Enum.TryParse<UserTenantRole>(role, ignoreCase: true, out var r) ? r : UserTenantRole.Member;
        db.UserTenants.Add(new UserTenant { TenantId = tenantId, UserId = userId, Role = parsedRole });
        await db.SaveChangesAsync();
    }
}
