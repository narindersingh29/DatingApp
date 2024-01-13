﻿using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountCotroller : BaseApiController
    {
        private readonly DataContext _context;

        public AccountCotroller(DataContext context)
{
            _context = context;
        }
        [HttpPost("register")] //api/account/register?username=dave&password=pwd
        public async Task<ActionResult<AppUser>> Register(string username, string password)
        {
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
             UserName = username,
             PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
             PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
