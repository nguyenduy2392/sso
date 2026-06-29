using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sso.Core.DTOs;
using Sso.Core.Interfaces;

namespace Sso.Api.Controllers;

[ApiController]
[Route("auth")]
public class OAuthController(IOAuthService oauthService, IConfiguration configuration) : ControllerBase
{
    /// <summary>
    /// Bước 1: Client app redirect user sang đây.
    /// Nếu đã có session cookie → cấp code ngay và redirect về redirect_uri.
    /// Nếu chưa → redirect sang trang login với returnUrl.
    /// </summary>
    [HttpGet("authorize")]
    public async Task<IActionResult> Authorize(
        [FromQuery] string client_id,
        [FromQuery] string redirect_uri,
        [FromQuery] string state = "")
    {
        var sessionUserId = Request.Cookies["sso_session"];

        if (string.IsNullOrEmpty(sessionUserId) || !Guid.TryParse(sessionUserId, out var userId))
        {
            var frontendUrl = configuration["FrontendUrl"]?.TrimEnd('/') ?? "http://localhost:4201";
            var scheme = Request.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? Request.Scheme;
            var backendBase = $"{scheme}://{Request.Host}";
            var authorizeUrl = $"{backendBase}/auth/authorize?client_id={client_id}&redirect_uri={Uri.EscapeDataString(redirect_uri)}&state={state}";
            var returnUrl = Uri.EscapeDataString(authorizeUrl);
            return Redirect($"{frontendUrl}/auth/login?returnUrl={returnUrl}");
        }

        try
        {
            var redirectUrl = await oauthService.AuthorizeAsync(client_id, redirect_uri, state, userId);
            return Redirect(redirectUrl);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Bước 2: Client app BE gọi để đổi code lấy JWT.
    /// </summary>
    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] TokenRequest request)
    {
        try
        {
            var result = await oauthService.ExchangeCodeAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
