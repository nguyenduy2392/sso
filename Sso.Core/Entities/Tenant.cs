namespace Sso.Core.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<UserTenant> UserTenants { get; set; } = [];
}
