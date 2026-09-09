namespace Sso.Core.Entities;

/// <summary>"Chức danh" — vai trò/chức vụ trong 1 tenant, gán cho thành viên qua OrgMember.</summary>
public class OrgRole : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Tenant Tenant { get; set; } = null!;
}
