using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
namespace LoginAPI.Controllers;

[Authorize]
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

    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(u =>  !u.UserName.Equals(username));
        if (user == null)
        {
            return NotFound();
        }

        _userContext.Users.Remove(user);
        await _userContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("EditUser")]
    public async Task<IActionResult> Edit(string username, [FromBody] UserDTO userDTO)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        // Update the existing user object instead of creating a new one
        user.UserName = userDTO.UserName;
        user.Email = userDTO.Email;
        user.Password = _passwordService.HashPassword(user, userDTO.Password);
        user.Age = userDTO.Age;

        _userContext.Entry(user).State = EntityState.Modified;

        try
        {
            await _userContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await UserExists(username)) // Ensure this is awaited
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userContext.Users.AnyAsync(e => e.UserName == username);
    }
}


