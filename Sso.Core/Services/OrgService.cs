using Microsoft.EntityFrameworkCore;
using Sso.Core.Data;
using Sso.Core.DTOs;
using Sso.Core.Entities;
using Sso.Core.Enums;
using Sso.Core.Interfaces;

namespace Sso.Core.Services;

public class OrgService(SsoDbContext db) : IOrgService
{
    public async Task<Org> UpsertOrgAsync(UpsertOrgRequest request)
    {
        var tenant = await GetOrCreateTenantAsync(request.TenantName);

        var org = await db.Orgs.FindAsync(request.Id);
        if (org == null)
        {
            org = new Org { Id = request.Id };
            db.Orgs.Add(org);
        }

        org.TenantId = tenant.Id;
        org.Code = request.Code;
        org.Title = request.Title;
        org.ShortName = request.ShortName;
        org.Description = request.Description;
        org.ParentOrgId = request.ParentOrgId;
        org.Status = Status.Active;
        org.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return org;
    }

    public async Task<bool> DeactivateOrgAsync(Guid id)
    {
        var org = await db.Orgs.FindAsync(id);
        if (org == null) return false;

        org.Status = Status.Inactive;
        org.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<OrgRole> UpsertOrgRoleAsync(UpsertOrgRoleRequest request)
    {
        var tenant = await GetOrCreateTenantAsync(request.TenantName);

        var role = await db.OrgRoles.FindAsync(request.Id);
        if (role == null)
        {
            role = new OrgRole { Id = request.Id };
            db.OrgRoles.Add(role);
        }

        role.TenantId = tenant.Id;
        role.Code = request.Code;
        role.Name = request.Name;
        role.SortOrder = request.SortOrder;
        role.Status = Status.Active;
        role.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return role;
    }

    public async Task<bool> DeactivateOrgRoleAsync(Guid id)
    {
        var role = await db.OrgRoles.FindAsync(id);
        if (role == null) return false;

        role.Status = Status.Inactive;
        role.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<OrgMember?> UpsertOrgMemberAsync(UpsertOrgMemberRequest request)
    {
        var orgExists = await db.Orgs.AnyAsync(o => o.Id == request.OrgId);
        if (!orgExists) return null;

        var userExists = await db.Users.AnyAsync(u => u.Id == request.SsoUserId);
        if (!userExists) return null;

        var member = await db.OrgMembers.FindAsync(request.Id);
        if (member == null)
        {
            member = new OrgMember { Id = request.Id };
            db.OrgMembers.Add(member);
        }

        member.OrgId = request.OrgId;
        member.SsoUserId = request.SsoUserId;
        member.OrgRoleId = request.OrgRoleId;
        member.Status = Status.Active;
        member.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return member;
    }

    public async Task<bool> DeactivateOrgMemberAsync(Guid id)
    {
        var member = await db.OrgMembers.FindAsync(id);
        if (member == null) return false;

        member.Status = Status.Inactive;
        member.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ResolvedOrgMemberDto>> GetResolvedMembersAsync(string tenantName)
    {
        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Name == tenantName);
        if (tenant == null) return [];

        return await db.OrgMembers
            .Where(m => m.Status == Status.Active
                     && m.Org.TenantId == tenant.Id
                     && m.Org.Status == Status.Active)
            .Select(m => new ResolvedOrgMemberDto
            {
                SsoUserId = m.SsoUserId,
                OrgId = m.OrgId,
                OrgTitle = m.Org.Title,
                OrgShortName = m.Org.ShortName,
                OrgRoleId = m.OrgRoleId,
                OrgRoleName = m.OrgRole != null && m.OrgRole.Status == Status.Active ? m.OrgRole.Name : null
            })
            .ToListAsync();
    }

    private async Task<Tenant> GetOrCreateTenantAsync(string tenantName)
    {
        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Name == tenantName);
        if (tenant != null) return tenant;

        tenant = new Tenant { Name = tenantName };
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync();
        return tenant;
    }
}
