using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseController
{

    [HttpPost("register")] // account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExist(registerDto.Username))
        {
            return BadRequest("username already exists");
        }

        using var hmca = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmca.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmca.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };

    }

    public async Task<bool> UserExist(string username)
    {
        return await context.Users.AnyAsync(record => record.UserName.ToLower() == username.ToLower());
    }

    [HttpPost("Login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(item => item.UserName == loginDto.Username.ToLower());
        if(user == null) return Unauthorized("Invalid username");

        using var hmcs = new HMACSHA512(user.PasswordSalt);
        var loginUserPasswordhash = hmcs.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < loginUserPasswordhash.Length; i++)
        {
            if(loginUserPasswordhash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
        }

        return new UserDto
        {
            Username = loginDto.Username,
            Token = tokenService.CreateToken(user)
        };
        
    }
}
