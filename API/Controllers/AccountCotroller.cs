﻿using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountCotroller : BaseApiController
    {
        private readonly DataContext _context;

        public AccountCotroller(DataContext context)
{
            _context = context;
        }
        [HttpPost("register")] // POST: api/account/register?username=dave&password=pwd
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
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
            return user;
        }

        private ActionResult<AppUser> BadRequest(string v)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}