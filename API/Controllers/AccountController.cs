﻿using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class AccountCotroller : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountCotroller(DataContext context, ITokenService tokenService)
{
            _context = context;
            _tokenService = tokenService;
        }
     [AllowAnonymous]
        [HttpPost("register")] // POST: api/account/register?username=dave&password=pwd
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("UserName is Taken");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
             UserName = registerDto.Username.ToLower(),
             PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
             PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
               Username = user.UserName,
               Token = _tokenService.CreateToken(user)
            };
        }

        private ActionResult<UserDto> BadRequest(string v)
        {
            throw new NotImplementedException();
        }
        [AllowAnonymous]
        [HttpPost("Login")]
public async Task<UserDto> Login(LoginDto loginDto)
{
    var user = await _context.Users.SingleOrDefaultAsync(x =>
    x.UserName == loginDto.Username);
    if (user == null) return Unauthorized();
    using var hmac = new HMACSHA512(user.PasswordSalt);
    var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
    for (int i = 0; i < ComputeHash.Length; i++)
    {
        if (ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
    }
     return new UserDto
            {
               Username = user.UserName,
               Token = _tokenService.CreateToken(user)
            };
}

        private UserDto Unauthorized(string v)
        {
            throw new NotImplementedException();
        }

        private UserDto Unauthorized()
        {
            throw new NotImplementedException();
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
