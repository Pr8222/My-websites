using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
namespace LoginAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserContext _userContext;
    private readonly PasswordService _passwordService;

    public UserController(UserContext context)
    {
        _userContext = context;
        _passwordService = new PasswordService();
    }
    //Get: api/user all users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        return await _userContext.Users.ToListAsync();
    }

    //Get: api/user/userPage
    [HttpGet("userPage")]
    public async Task<ActionResult<User>> GetUser(string username)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

        if (user == null)
        {
            return NotFound();
        }

        return user;
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
            Age = userDTO.Age
        };
        user.Password = _passwordService.HashPassword(user, user.Password);

        _userContext.Users.Add(user);
        await _userContext.SaveChangesAsync();

        return Ok(CreatedAtAction(nameof(GetUser), new { id = user.Id }, user));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userContext.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _userContext.Users.Remove(user);
        await _userContext.SaveChangesAsync();

        return NoContent();
    }



}


