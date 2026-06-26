namespace Sso.Core.Entities;

public class OAuthClient : BaseEntity
{
    public string ClientId { get; set; } = null!;
    public string ClientSecretHash { get; set; } = null!;
    public string Name { get; set; } = null!;
    // JSON array of allowed redirect URIs
    public string AllowedRedirectUris { get; set; } = "[]";
}
