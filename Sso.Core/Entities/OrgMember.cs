namespace Sso.Core.Entities;

/// <summary>Thành viên trong cơ cấu tổ chức — liên kết theo SsoUserId (User.Id bên SSO), không phải AppUserId của app gốc.</summary>
public class OrgMember : BaseEntity
{
    public Guid OrgId { get; set; }
    public Guid SsoUserId { get; set; }
    public Guid? OrgRoleId { get; set; }

    public Org Org { get; set; } = null!;
    public User User { get; set; } = null!;
    public OrgRole? OrgRole { get; set; }
}
