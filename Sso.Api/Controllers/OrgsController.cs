using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sso.Core.DTOs;
using Sso.Core.Interfaces;

namespace Sso.Api.Controllers;

/// <summary>
/// Nhận đồng bộ cơ cấu tổ chức (org/chức danh/thành viên) từ app gọi (HRM). Toàn bộ endpoint là
/// proxy ghi 1 chiều — cùng auth posture (không xác thực) với UsersController hiện có.
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("orgs")]
public class OrgsController(IOrgService orgService) : ControllerBase
{
    [HttpPost("upsert")]
    public async Task<IActionResult> UpsertOrg([FromBody] UpsertOrgRequest request)
    {
        var org = await orgService.UpsertOrgAsync(request);
        return Ok(new { org.Id, org.Code, org.Title, org.ShortName, org.ParentOrgId, org.Status });
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateOrg(Guid id)
    {
        var ok = await orgService.DeactivateOrgAsync(id);
        return ok ? Ok(new { message = "Org deactivated." }) : NotFound(new { message = "Org not found." });
    }

    [HttpPost("roles/upsert")]
    public async Task<IActionResult> UpsertOrgRole([FromBody] UpsertOrgRoleRequest request)
    {
        var role = await orgService.UpsertOrgRoleAsync(request);
        return Ok(new { role.Id, role.Code, role.Name, role.SortOrder, role.Status });
    }

    [HttpPost("roles/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateOrgRole(Guid id)
    {
        var ok = await orgService.DeactivateOrgRoleAsync(id);
        return ok ? Ok(new { message = "OrgRole deactivated." }) : NotFound(new { message = "OrgRole not found." });
    }

    [HttpPost("members/upsert")]
    public async Task<IActionResult> UpsertOrgMember([FromBody] UpsertOrgMemberRequest request)
    {
        var member = await orgService.UpsertOrgMemberAsync(request);
        return member is null
            ? NotFound(new { message = "Org hoặc SSO user không tồn tại." })
            : Ok(new { member.Id, member.OrgId, member.SsoUserId, member.OrgRoleId, member.Status });
    }

    [HttpPost("members/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateOrgMember(Guid id)
    {
        var ok = await orgService.DeactivateOrgMemberAsync(id);
        return ok ? Ok(new { message = "OrgMember deactivated." }) : NotFound(new { message = "OrgMember not found." });
    }

    /// <summary>
    /// Toàn bộ membership đang active của 1 tenant, đã join sẵn tên Org/OrgRole — app gọi (vd. Cloud)
    /// dùng để ghép vào danh sách user của chính nó theo SsoUserId.
    /// </summary>
    [HttpGet("members/resolved")]
    public async Task<IActionResult> GetResolvedMembers([FromQuery] string tenantName)
    {
        if (string.IsNullOrWhiteSpace(tenantName))
            return BadRequest(new { message = "tenantName không được để trống." });

        var members = await orgService.GetResolvedMembersAsync(tenantName);
        return Ok(members);
    }
}
