using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using DatingApp.Api.Entities;
using DatingApp.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = _context.Users
                .SingleOrDefaultAsync(q => q.UserName == loginDto.Username).Result;

            if (user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            return new UserDto
            {
                Username = loginDto.Username,
                Token = _tokenService.CreateToken(user)
            };

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

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
                Username = registerDto.Username,
                Token = _tokenService.CreateToken(user)
            };
        }

        private Task<bool> UserExists(string username)
        {
            return _context.Users.AnyAsync(q => q.UserName == username.ToLower());

        }
    }
}
