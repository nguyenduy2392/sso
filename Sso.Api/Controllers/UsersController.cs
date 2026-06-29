using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sso.Core.DTOs;
using Sso.Core.Interfaces;

namespace Sso.Api.Controllers;

[Authorize]
[ApiController]
[Route("users")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await userService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = await userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                new { user.Id, user.UserName, user.Name, user.Email, user.Phone, user.Status });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await userService.UpdateAsync(id, request);
        return user is null
            ? NotFound(new { message = "User not found." })
            : Ok(new { user.Id, user.UserName, user.Name, user.Email, user.Phone, user.Avatar });
    }

    [AllowAnonymous]
    [HttpPut("{id:guid}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
    {
        var ok = await userService.ChangePasswordAsync(id, request.NewPassword);
        return ok ? Ok(new { message = "Password changed." }) : NotFound(new { message = "User not found." });
    }

    [AllowAnonymous]
    [HttpGet("by-username")]
    public async Task<IActionResult> GetByUserNameAndTenant([FromQuery] string userName, [FromQuery] string tenantName)
    {
        var user = await userService.GetByUserNameAndTenantAsync(userName, tenantName);
        return user is null
            ? NotFound(new { message = "User not found." })
            : Ok(new { user.Id, user.UserName, user.Name, user.Email, user.Phone, user.Avatar });
    }

    /// <summary>
    /// Sync batch users từ HRM tenant sang SSO.
    /// Password hash copy trực tiếp (cùng PBKDF2 + cùng salt).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("sync-batch")]
    public async Task<IActionResult> SyncBatch([FromBody] SyncBatchRequest request)
    {
        var result = await userService.SyncBatchAsync(request);
        return Ok(result);
    }
}
