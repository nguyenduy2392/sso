namespace Sso.Core.Entities;

public class AuthorizationCode : BaseEntity
{
    public string Code { get; set; } = null!;
    public Guid SsoUserId { get; set; }
    public string ClientId { get; set; } = null!;
    public string RedirectUri { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
}
