namespace Sso.Core.DTOs;

public class TokenRequest
{
    public string Code { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string RedirectUri { get; set; } = null!;
}
