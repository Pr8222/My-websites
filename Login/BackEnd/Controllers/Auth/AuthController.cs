using Data;
using Models;
using LoginAPI.Services.HtmlSanitizerService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginAPI.Services.PasswordService;
namespace LoginAPI.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly UserContext _userContext;
    private readonly IConfiguration _config;
    private readonly PasswordService _passwordService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly HtmlSanitizerService _sanitizer;
    public AuthController(UserContext context, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
    {
        _userContext = context;
        _config = configuration;
        _passwordHasher = passwordHasher;
        _passwordService = new PasswordService();
        _sanitizer = new HtmlSanitizerService();
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
    {
        if (_userContext.Users.Any(u => u.UserName == userDTO.UserName))
        {
            return BadRequest("Username already taken.");
        }

        var user = new User
        {
            UserName = userDTO.UserName,
            Email = userDTO.Email,
            Password = userDTO.Password,
            Age = userDTO.Age,
            Role = "User"
        };
        
        user.Password = _passwordService.HashPassword(user, user.Password);

        _userContext.Users.Add(user);
        await _userContext.SaveChangesAsync();

        return Ok();
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
            var token = GenerateJwtToken(user.UserName, user.Role);
            return Ok(new { Token = token });
        }
        return Unauthorized("Invalid credentials.");
    }

    private string GenerateJwtToken(string username, string role)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddHours(1),

            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],

            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}