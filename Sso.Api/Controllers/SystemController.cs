using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sso.Core.Data;

namespace Sso.Api.Controllers;

[ApiController]
[Route("api/system")]
[AllowAnonymous]
public class SystemController(SsoDbContext db) : ControllerBase
{
    /// <summary>
    /// Chạy EF migration cho SsoDB — gọi từ EcoControl qua HMAC hoặc loopback.
    /// </summary>
    [HttpPost("migration/run")]
    public async Task<IActionResult> RunMigration()
    {
        var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
        if (pending.Count == 0)
            return Ok(new { applied = Array.Empty<string>(), count = 0, message = "SsoDB đã ở phiên bản mới nhất." });

        await db.Database.MigrateAsync();
        return Ok(new { applied = pending, count = pending.Count, message = $"Đã áp dụng {pending.Count} migration." });
    }

    [HttpPost("tenant/rename")]
    public async Task<IActionResult> RenameTenant([FromQuery] string oldName, [FromQuery] string newName)
    {
        if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName))
            return BadRequest(new { success = false, message = "oldName và newName không được để trống." });

        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Name == oldName);
        if (tenant == null)
            return Ok(new { success = false, message = $"Không tìm thấy tenant '{oldName}' trong SSO." });

        tenant.Name = newName;
        await db.SaveChangesAsync();
        return Ok(new { success = true, message = $"Đã đổi tên tenant SSO '{oldName}' → '{newName}'." });
    }
}
