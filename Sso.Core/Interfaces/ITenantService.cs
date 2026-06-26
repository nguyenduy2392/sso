using Sso.Core.DTOs;
using Sso.Core.Entities;

namespace Sso.Core.Interfaces;

public interface ITenantService
{
    Task<List<Tenant>> GetAllAsync();
    Task<Tenant?> GetByIdAsync(Guid id);
    Task<Tenant> CreateAsync(CreateTenantRequest request);
    Task AddUserAsync(Guid tenantId, Guid userId, string role = "Member");
}
