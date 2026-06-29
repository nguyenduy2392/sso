using Sso.Core.DTOs;
using Sso.Core.Entities;

namespace Sso.Core.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUserNameAndTenantAsync(string userName, string tenantName);
    Task<User> CreateAsync(RegisterRequest request);
    Task<User?> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<bool> ChangePasswordAsync(Guid id, string newPassword);
    Task<SyncBatchResult> SyncBatchAsync(SyncBatchRequest request);
}
