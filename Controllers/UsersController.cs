using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoronaVirusApi.Dtos;
using CoronaVirusApi.Models;
using CoronaVirusApi.Models.Repo;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoronaVirusApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        IDbContext db;
        private readonly IUserRepo<Users> usersRepo;
        private readonly IConfiguration config;
        public UsersController(IUserRepo<Users> _userRepo, IConfiguration _config, IDbContext _db)
        {


            db = _db;
            usersRepo = _userRepo;
            config = _config;
        }


        [HttpGet("Getusers")]
        public IEnumerable<Users> GetUsers()
        {
            var users = db.Users.ToList();
            return users;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {   //validation

            userRegisterDto.userName.ToLower();
            if (await usersRepo.UserExist(userRegisterDto.userName))//see if user is alredy exisit in db
            {
                return BadRequest("that user alrady exiset");
            }
            var userCreat = new Users
            {
                UserName = userRegisterDto.userName
            };
            await usersRepo.Register(userCreat, userRegisterDto.password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = await usersRepo.Login(userLoginDto.userName.ToLower(), userLoginDto.password);
            if (user == null)
            {
                return Unauthorized();
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }


        //[HttpGet("logout")]
        //public async Task<IActionResult> Logout(BannedTokens tokens)
        //{
        //    var token = await HttpContext.GetTokenAsync("access_token");
        //    tokens.Token = token;
        //    var tToken = new BannedTokens { Token = tokens.Token };
        //    // Console.WriteLine(tToken.Token);
        //    await db.BannedTokens.AddAsync(tToken);
        //   await db.SaveChangesAsync();
        //    return Ok();



        //}
    }
}