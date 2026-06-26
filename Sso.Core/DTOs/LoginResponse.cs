namespace Sso.Core.DTOs;

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Avatar { get; set; }
    public string? Description { get; set; }
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = null!;
}
