namespace Sso.Core.Entities;

public class Org : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? Description { get; set; }
    public Guid? ParentOrgId { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public Org? ParentOrg { get; set; }
}
