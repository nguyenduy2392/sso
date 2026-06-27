using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sso.Core.DTOs;
using Sso.Core.Interfaces;

namespace Sso.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService, IConfiguration configuration) : ControllerBase
{
    /// <summary>
    /// Browser GET /auth/login → redirect về FE page (giữ nguyên query string).
    /// Xảy ra khi nginx route /auth/* tới BE nhưng /auth/login là FE Angular route.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult LoginPage()
    {
        var frontendUrl = configuration["FrontendUrl"]?.TrimEnd('/') ?? "http://localhost:4201";
        var qs = Request.QueryString.HasValue ? Request.QueryString.Value : "";
        return Redirect($"{frontendUrl}/login{qs}");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await authService.LoginAsync(request);

            Response.Cookies.Append("sso_session", result.UserId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                Path = "/",
            });

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            var result = await authService.RefreshAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!);
        await authService.LogoutAsync(userId, request.RefreshToken);
        Response.Cookies.Delete("sso_session");
        return NoContent();
    }

    /// <summary>
    /// Single Sign-Out: xóa SSO session cookie rồi redirect về returnUrl.
    /// Được gọi từ các app con khi user đăng xuất.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("logout")]
    public IActionResult SsoLogout([FromQuery] string? returnUrl)
    {
        Response.Cookies.Delete("sso_session", new CookieOptions { Path = "/" });

        var allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? [];
        var redirect = allowedOrigins.Any(o => returnUrl?.StartsWith(o) == true)
            ? returnUrl!
            : "/auth/login";

        return Redirect(redirect);
    }
}
