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
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LoginAPI.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly UserContext _userContext;
    private readonly IConfiguration _config;
    private readonly PasswordService _passwordService; // For hashing password
    private readonly IPasswordHasher<User> _passwordHasher; // For verifying the hashed password
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            // Sanitize user input (prevent XSS)
            var sanitizer = new HtmlSanitizerService();
            userDTO.UserName = sanitizer.Sanitize(userDTO.UserName);
            userDTO.Email = sanitizer.Sanitize(userDTO.Email);

            if (_userContext.Users.Any(u => u.UserName == userDTO.UserName))
            {
                return BadRequest("Username already taken.");
            }

            var userRole = await _userContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
            if (userRole == null)
            {
                return StatusCode(500, "User role not found.");
            }

            var user = new User
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                Password = userDTO.Password,
                Age = userDTO.Age
            };

            user.Password = _passwordService.HashPassword(user, user.Password);
            if (user.Password == null)
            {
                return StatusCode(500, "Error hashing password.");
            }

            _userContext.Users.Add(user);
            await _userContext.SaveChangesAsync();

            // Updating RoleUsers table
            _userContext.RoleUsers.Add(new RoleUser
            {
                UserId = user.Id,
                RoleId = userRole.Id
            });
            await _userContext.SaveChangesAsync();
            return Ok(new {message = "User registered successfully."});
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        try
        {
            // Find the user by username
            var user = await _userContext.Users
                .FirstOrDefaultAsync(u => u.UserName == login.UserName);

            // Check if user exists
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Find the role associated with the user
            var userRoleIds = await _userContext.RoleUsers
                .Where(ru => ru.UserId == user.Id)
                .Select(ru => ru.RoleId)
                .ToListAsync();
            var userRole = await _userContext.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .ToListAsync();

            // Finding the Keys that a user can have
            var allKeys = await _userContext.Keys.Select(k => k.KeyName).ToListAsync();

            var accessibleKeys = await (
                from ru in _userContext.RoleUsers
                where ru.UserId == user.Id
                join rk in _userContext.RoleKeys on ru.RoleId equals rk.RoleId
                join k in _userContext.Keys on rk.KeyId equals k.Id
                select k.KeyName
                ).Distinct().ToListAsync();

            // Creating the variable to store the keys that the user can have to generate the JWT token
            var accessibleKeyList = allKeys.ToDictionary(
                key => key,
                key => accessibleKeys.Contains(key) 
                );

            if (!userRole.Any())
            {
                return StatusCode(500, "User role not found.");
            }

            // Verify the password (compare the stored hash with the provided password)
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);

            if (result == PasswordVerificationResult.Success)
            {
                // Generate JWT token
                var token = GenerateJwtToken(user.UserName, userRole.First().RoleName, accessibleKeyList);
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid credentials.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    private string GenerateJwtToken(string username, string role, Dictionary<string, bool> keys)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("Keys", JsonConvert.SerializeObject(keys))
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