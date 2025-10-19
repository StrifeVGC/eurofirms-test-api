using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PruebaEurofirms.Api.Models;
using PruebaEurofirms.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/// <summary>
/// Controller responsible for authentication.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    /// <summary>
    /// The user manager from Identity framework.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// The configuration to read JwtKey.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="configuration">The configuration.</param>
    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Logins the specified request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var jwtKey = _configuration["JwtKey"];
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "JWT key not configured." });
        }

        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(jwtKey);
        }
        catch (FormatException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "JWT key is not a valid Base64 string." });
        }

        var signingKey = new SymmetricSecurityKey(keyBytes);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                // ensure claim value is non-null
                new Claim(ClaimTypes.Name, user.UserName ?? user.Id ?? string.Empty)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { token = tokenString });
    }
}
