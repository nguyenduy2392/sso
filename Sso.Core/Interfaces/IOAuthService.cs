using Sso.Core.DTOs;

namespace Sso.Core.Interfaces;

public interface IOAuthService
{
    /// <summary>Validate client + redirect_uri, issue authorization code. Returns the redirect URL.</summary>
    Task<string> AuthorizeAsync(string clientId, string redirectUri, string state, Guid ssoUserId);

    /// <summary>Exchange authorization code for JWT access token.</summary>
    Task<TokenResponse> ExchangeCodeAsync(TokenRequest request);
}
