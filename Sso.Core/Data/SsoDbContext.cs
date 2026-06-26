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

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<UserTenant>()
            .HasIndex(x => new { x.UserId, x.TenantId })
            .IsUnique();

        b.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

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
