using Sso.Core.Enums;

namespace Sso.Core.Entities;

public class UserTenant : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public UserTenantRole Role { get; set; } = UserTenantRole.Member;
    public User User { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}
