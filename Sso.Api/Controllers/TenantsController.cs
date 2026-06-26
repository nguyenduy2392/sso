using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sso.Core.DTOs;
using Sso.Core.Interfaces;

namespace Sso.Api.Controllers;

[Authorize]
[ApiController]
[Route("tenants")]
public class TenantsController(ITenantService tenantService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await tenantService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tenant = await tenantService.GetByIdAsync(id);
        return tenant is null ? NotFound() : Ok(tenant);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        var tenant = await tenantService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, tenant);
    }

    [HttpPost("{tenantId:guid}/users/{userId:guid}")]
    public async Task<IActionResult> AddUser(Guid tenantId, Guid userId, [FromQuery] string role = "Member")
    {
        try
        {
            await tenantService.AddUserAsync(tenantId, userId, role);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
