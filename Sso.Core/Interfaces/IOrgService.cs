using Sso.Core.DTOs;
using Sso.Core.Entities;

namespace Sso.Core.Interfaces;

public interface IOrgService
{
    Task<Org> UpsertOrgAsync(UpsertOrgRequest request);
    Task<bool> DeactivateOrgAsync(Guid id);

    Task<OrgRole> UpsertOrgRoleAsync(UpsertOrgRoleRequest request);
    Task<bool> DeactivateOrgRoleAsync(Guid id);

    /// <summary>null nếu OrgId hoặc SsoUserId không tồn tại.</summary>
    Task<OrgMember?> UpsertOrgMemberAsync(UpsertOrgMemberRequest request);
    Task<bool> DeactivateOrgMemberAsync(Guid id);

    /// <summary>Toàn bộ membership đang active của 1 tenant, đã join sẵn tên Org/OrgRole để app gọi dùng ngay.</summary>
    Task<List<ResolvedOrgMemberDto>> GetResolvedMembersAsync(string tenantName);
}
