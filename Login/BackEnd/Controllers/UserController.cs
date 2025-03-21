﻿using Data;
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
    [AllowAnonymous]
    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(u =>  u.UserName.Equals(username));
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

        // Update the user
        // Update the username if the username is null
        if (!string.IsNullOrEmpty(userDTO.UserName))
        {
            user.UserName = userDTO.UserName;
        } else
        {
            user.Email = user.Email;
        }
        // Update the email if the email is null
        if (!string.IsNullOrEmpty(userDTO.Email))
        {
            user.Email = userDTO.Email;
        } else
        {
            user.Email = user.Email;
        }
        // Update the password if the password is null
        if (!string.IsNullOrEmpty(userDTO.Password))
        {
            user.Password = _passwordService.HashPassword(user, userDTO.Password);
        } else
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
    [HttpPut("Promote")]
    public async Task<IActionResult> Promote(string username)
    {
        var user = _userContext.Users.FirstOrDefault(u => u.UserName.Equals(username));
        if (user == null)
        {
            return NotFound("There's no such user with that username");
        }

        // Find the "Admin" role in the Roles table
        var adminRole = await _userContext.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null)
        {
            return BadRequest("Admin role not found");
        }

        // Assign the RoleId instead of Role
        user.RoleId = adminRole.Id;
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
        var adminRole = await _userContext.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        // Check if there is a role named "Admin"
        if (adminRole == null)
        {
            return BadRequest("Admin role not found");
        }
        // Check if the user is an admin
        if (admin.RoleId != adminRole.Id)
        {
            return NotFound("The user is not admin!");
        }

        var userRole = await _userContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        //Check if there is a role named "User"
        if(userRole == null)
        {
            return BadRequest("User role not found");
        }

        admin.RoleId = userRole.Id;
        await _userContext.SaveChangesAsync();

        return Ok(new { Message = "Admin has been demoted!" });
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userContext.Users.AnyAsync(e => e.UserName == username);
    }
}


