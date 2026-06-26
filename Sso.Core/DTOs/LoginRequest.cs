namespace Sso.Core.DTOs;

public class LoginRequest
{
    public string TenantName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
