using Data;
using Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace LoginAPI.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly UserContext _userContext;
    private readonly IConfiguration _config;
    private readonly IPasswordHasher<User> _passwordHasher;
    public AuthController(UserContext context, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
    {
        _userContext = context;
        _config = configuration;
        _passwordHasher = passwordHasher;
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] Login login)
    {
        var user = _userContext.Users.FirstOrDefault(u => u.UserName == login.UserName);
        if (user == null) 
        {
            return Unauthorized("Invalid username or password.");
        }
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);
        if (result == PasswordVerificationResult.Success)
        {
            var token = GenerateJwtToken(user.UserName);
            return Ok(new { Token = token });
        }
        return Unauthorized("Invalid credentials.");
    }

    private string GenerateJwtToken(string username)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}