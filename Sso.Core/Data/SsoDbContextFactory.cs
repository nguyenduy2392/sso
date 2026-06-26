using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sso.Core.Data;

public class SsoDbContextFactory : IDesignTimeDbContextFactory<SsoDbContext>
{
    public SsoDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SSO_CONNECTION_STRING")
            ?? "Server=42.118.102.113,1434;Database=SsoDB;User Id=sa;Password=EcoTech@!123;MultipleActiveResultSets=true;TrustServerCertificate=True;";

        var opt = new DbContextOptionsBuilder<SsoDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new SsoDbContext(opt);
    }
}
