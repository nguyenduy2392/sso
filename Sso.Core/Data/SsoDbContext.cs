using Microsoft.EntityFrameworkCore;
using Sso.Core.Entities;

namespace Sso.Core.Data;

public class SsoDbContext(DbContextOptions<SsoDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<UserTenant> UserTenants => Set<UserTenant>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OAuthClient> OAuthClients => Set<OAuthClient>();
    public DbSet<AuthorizationCode> AuthorizationCodes => Set<AuthorizationCode>();
    public DbSet<Org> Orgs => Set<Org>();
    public DbSet<OrgRole> OrgRoles => Set<OrgRole>();
    public DbSet<OrgMember> OrgMembers => Set<OrgMember>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<UserTenant>()
            .HasIndex(x => new { x.UserId, x.TenantId })
            .IsUnique();

        b.Entity<Org>()
            .HasOne(o => o.Tenant)
            .WithMany()
            .HasForeignKey(o => o.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Org>()
            .HasOne(o => o.ParentOrg)
            .WithMany()
            .HasForeignKey(o => o.ParentOrgId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<OrgRole>()
            .HasOne(r => r.Tenant)
            .WithMany()
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<OrgMember>()
            .HasOne(m => m.Org)
            .WithMany()
            .HasForeignKey(m => m.OrgId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<OrgMember>()
            .HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.SsoUserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<OrgMember>()
            .HasOne(m => m.OrgRole)
            .WithMany()
            .HasForeignKey(m => m.OrgRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<User>()
            .HasIndex(u => u.UserName);

        b.Entity<Tenant>()
            .HasIndex(t => t.Name)
            .IsUnique();

        b.Entity<RefreshToken>()
            .HasIndex(r => r.Token)
            .IsUnique();

        b.Entity<OAuthClient>()
            .HasIndex(c => c.ClientId)
            .IsUnique();

        b.Entity<AuthorizationCode>()
            .HasIndex(c => c.Code)
            .IsUnique();
    }
}
