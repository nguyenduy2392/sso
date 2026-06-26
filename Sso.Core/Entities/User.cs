using System.ComponentModel.DataAnnotations;

namespace Sso.Core.Entities;

public class User : BaseEntity
{
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [StringLength(100)]
    public string? Email { get; set; }
    [StringLength(20)]
    public string? Phone { get; set; }
    [StringLength(500)]
    public string? Avatar { get; set; }
    [StringLength(1000)]
    public string? Description { get; set; }
    public ICollection<UserTenant> UserTenants { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
