namespace Sso.Core.DTOs;

/// <summary>Upsert theo Id — dùng chung Guid Id với bảng gốc bên app gọi (vd. HRM.AppOrg.Id).</summary>
public class UpsertOrgRequest
{
    public Guid Id { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string? Description { get; set; }
    public Guid? ParentOrgId { get; set; }
}

public class UpsertOrgRoleRequest
{
    public Guid Id { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

/// <summary>SsoUserId là User.Id bên SSO (đã resolve từ AppUser.SsoId phía app gọi) — không phải AppUserId.</summary>
public class UpsertOrgMemberRequest
{
    public Guid Id { get; set; }
    public Guid OrgId { get; set; }
    public Guid SsoUserId { get; set; }
    public Guid? OrgRoleId { get; set; }
}

/// <summary>
/// 1 dòng = 1 membership đã join sẵn tên hiển thị (Org.Title/ShortName, OrgRole.Name) — app gọi
/// (vd. Cloud) dùng để ghép vào danh sách user của chính nó theo SsoUserId, không cần gọi thêm.
/// </summary>
public class ResolvedOrgMemberDto
{
    public Guid SsoUserId { get; set; }
    public Guid OrgId { get; set; }
    public string OrgTitle { get; set; } = string.Empty;
    public string? OrgShortName { get; set; }
    public Guid? OrgRoleId { get; set; }
    public string? OrgRoleName { get; set; }
}
