using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;
using Models;
using LoginAPI.Services.PasswordService;
using LoginAPI.Attributes;

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
    [HasKey("ViewUsers")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = _userContext.Users.Select(u => new {
            u.Id,
            u.UserName,
            u.Email,
            u.Age,
            Role = _userContext.RoleUsers
                .Where(ru => ru.UserId == u.Id)
                .Select(ru => _userContext.Roles.FirstOrDefault(r => r.Id == ru.RoleId).RoleName)
                .FirstOrDefault(),
            Keys = _userContext.RoleKeys
                .Where(rk => _userContext.RoleUsers
                            .Where(ru => ru.UserId == u.Id)
                            .Select(ru => ru.RoleId)
                            .Contains(rk.RoleId))
                        .Select(rk => rk.Key.KeyName)
                        .ToList()
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
    [HasKey("ShowCurrentUser")]
    public async Task<ActionResult<UserRoleDTO>> GetUser(string username)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
        user.UserName = HtmlEncoder.Default.Encode(user.UserName);
        if (user == null)
        {
            return NotFound();
        }

        var userData = new UserRoleDTO
        {
            UserName = user.UserName,
            Email = user.Email,
            Age = user.Age,
            Role = _userContext.RoleUsers
                .Where(ru => ru.UserId == user.Id)
                .Select(ru => _userContext.Roles.FirstOrDefault(r => r.Id == ru.RoleId).RoleName)
                .FirstOrDefault()
        };

        return userData;
    }

    // Show keys
    [Authorize(Roles = "SuperAdmin")]
    [HasKey("ShowKeys")]
    [HttpGet("ShowKeys")]
    public async Task<ActionResult<IEnumerable<User>>> GetKeys()
    {
        var keys = _userContext.Keys.Select(k => new
        {
            k.Id,
            k.KeyName,
            k.FriendlyKeyName
        }).ToList();

        return Ok(keys);
    }

    [AllowAnonymous]
    [HttpDelete("DeleteUser")]
    [HasKey("DeleteCurrentUser")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));
        if (user == null)
        {
            return NotFound();
        }

        _userContext.Users.Remove(user);
        await _userContext.SaveChangesAsync();

        return NoContent();
    }

    [HasKey("EditCurrentUser")]
    [HttpPut("EditUser")]
    public async Task<IActionResult> Edit(string username, [FromBody] UserDTO userDTO)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));
        if (user == null)
        {
            return BadRequest("User not found.");
        }
        // Checks that if the new username is already taken or not
        if(_userContext.Users.Any(u => u.UserName == userDTO.UserName)){
            return BadRequest("Username already taken.");
        }
        // Update the user
        // Update the username if the username is null
        if (!string.IsNullOrEmpty(userDTO.UserName))
        {
            user.UserName = userDTO.UserName;
        }
        else
        {
            user.Email = user.Email;
        }
        // Update the email if the email is null
        if (!string.IsNullOrEmpty(userDTO.Email))
        {
            user.Email = userDTO.Email;
        }
        else
        {
            user.Email = user.Email;
        }
        // Update the age if the age is null
        if (userDTO.Age != 0)
        {
            user.Age = userDTO.Age;
        }
        else
        {
            user.Age = user.Age;
        }
        // Update the password if the password is null
        if (!string.IsNullOrEmpty(userDTO.Password))
        {
            user.Password = _passwordService.HashPassword(user, userDTO.Password);
        }
        else
        {
            user.Password = user.Password;
        }

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
    [HasKey("PromoteUsers")]
    [HttpPut("Promote")]
    public async Task<IActionResult> Promote(string username)
    {
        var user = _userContext.Users.FirstOrDefault(u => u.UserName.Equals(username));
        if (user == null)
        {
            return NotFound("There's no such user with that username");
        }

        // Find the "Admin" role in the Roles table
        var adminRole = await _userContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
        if (adminRole == null)
        {
            return BadRequest("Admin role not found");
        }

        var existingRoleLinks = _userContext.RoleUsers.Where(ru => ru.UserId == user.Id);
        _userContext.RoleUsers.RemoveRange(existingRoleLinks);

        // Assign new role user
        _userContext.RoleUsers.Add(new RoleUser
        {
            UserId = user.Id,
            RoleId = adminRole.Id
        });

        await _userContext.SaveChangesAsync();

        return Ok(new { Message = "User Promoted to admin!" });
    }

    // Super Admin Demotes An Admin To A Regular User
    [Authorize(Roles = "SuperAdmin")]
    [HasKey("DemoteUsers")]
    [HttpPut("Demote")]
    public async Task<IActionResult> Demote(string username)
    {
        var admin = _userContext.Users.FirstOrDefault(a => a.UserName.Equals(username));

        if (admin == null)
        {
            return NotFound("There's no such user with that username");
        }
        var adminRole = await _userContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
        // Check if there is a role named "Admin"
        if (adminRole == null)
        {
            return BadRequest("Admin role not found");
        }
        var junctionRole = await _userContext.RoleUsers.Where(ru => ru.UserId == admin.Id).FirstOrDefaultAsync();
        // Check if the user is an admin
        if (junctionRole.RoleId != adminRole.Id)
        {
            return NotFound("The user is not admin!");
        }

        var userRole = await _userContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
        //Check if there is a role named "User"
        if (userRole == null)
        {
            return BadRequest("User role not found");
        }

        // Removing Existing RoleUser
        var existingRoleLinks = _userContext.RoleUsers.Where(ru => ru.UserId == admin.Id);
        _userContext.RoleUsers.RemoveRange(existingRoleLinks);

        // Assining new role to the demoted admin
        _userContext.RoleUsers.Add(new RoleUser
        {
            UserId = admin.Id,
            RoleId = userRole.Id
        });

        await _userContext.SaveChangesAsync();

        return Ok(new { Message = "Admin has been demoted!" });
    }
    // This function checks if the user exists
    private async Task<bool> UserExists(string username)
    {
        return await _userContext.Users.AnyAsync(e => e.UserName == username);
    }
    // This function checks for the keys that a user has
    private async Task<bool> UserHasAccess(string userId, string keyName)
    {
        var hasAccess = await _userContext.RoleUsers
            .Where(ru => ru.UserId == userId)
            .SelectMany(ru => _userContext.RoleKeys.Where(rk => rk.RoleId == ru.RoleId))
            .AnyAsync(rk => _userContext.Keys.Any(k => k.Id == rk.KeyId && k.KeyName == keyName));

        return hasAccess;
    }
}


