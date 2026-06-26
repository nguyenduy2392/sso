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
