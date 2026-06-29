namespace Sso.Core.DTOs;

public class RegisterRequest
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? TenantName { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Avatar { get; set; }
    public string? Description { get; set; }
}

public class UpdateUserRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Avatar { get; set; }
    public string? Description { get; set; }
}

public class ChangePasswordRequest
{
    public string NewPassword { get; set; } = null!;
}

public class SyncBatchRequest
{
    public string TenantName { get; set; } = null!;
    public List<SyncUserItem> Users { get; set; } = new();
}

public class SyncUserItem
{
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Avatar { get; set; }
}

public class SyncBatchResult
{
    public int Created { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
    public List<SyncUserResultItem> Items { get; set; } = new();
}

public class SyncUserResultItem
{
    public string UserName { get; set; } = null!;
    public Guid SsoUserId { get; set; }
    public string Action { get; set; } = null!;
}
