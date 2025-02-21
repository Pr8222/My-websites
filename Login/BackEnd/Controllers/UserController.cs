using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;
using Models;
using LoginAPI.Services.PasswordService;
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
    [Authorize(Roles = "Admin, SuperAdmin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = _userContext.Users.Select(u => new {
            u.Id,
            u.UserName,
            u.Email,
            u.Age,
            u.Role
        }).ToList();

        return Ok(new
        {
            recordsTotal = users.Count,
            recordsFiltered = users.Count,
            data = users
        });
    }

    //Get: api/user/userPage
    [AllowAnonymous]
    [HttpGet("userPage")]
    public async Task<ActionResult<User>> GetUser(string username)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
        user.UserName = HtmlEncoder.Default.Encode(user.UserName);
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

    // Super Admin Promotes A Regular User To Admin User
    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("Promote")]
    public async Task<IActionResult> Promote(string username)
    {
        var user = _userContext.Users.FirstOrDefault(u => u.UserName.Equals(username));
        if (user == null)
        {
            return NotFound("There's no such user with that username");
        }

        user.Role = "Admin";
        await _userContext.SaveChangesAsync();

        return Ok(new {Message  = "User Promoted to admin!"});
    }

    // Super Admin Demotes An Admin To A Regular User
    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("Demote")]
    public async Task<IActionResult> Demote(string username) 
    {
        var admin = _userContext.Users.FirstOrDefault(a => a.UserName.Equals(username));

        if(admin == null)
        {
            return NotFound("There's no such user with that username");
        }
        if(admin.Role != "Admin")
        {
            return NotFound("The user is not admin!");
        }

        admin.Role = "User";
        await _userContext.SaveChangesAsync();

        return Ok(new { Message = "Admin has been demoted!" });
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userContext.Users.AnyAsync(e => e.UserName == username);
    }
}


