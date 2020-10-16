using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaVirusApi.Models.Repo
{
    public class UserRepo : IUserRepo<Users>
    {
        IDbContext db;
        public UserRepo(IDbContext _db)
        {
            db = _db;
        }

        public async Task<Users> Find(int id)
        {
            var user = await db.Users.SingleOrDefaultAsync(s => s.Id == id);
            return user;
        }

        public async Task<Users> Login(string userName, string password)
        {
            var user = await db.Users.FirstOrDefaultAsync(s => s.UserName == userName);
            if (user == null)
            {
                return null;
            }
            if (!Verify(password, user.PasswordSalt, user.PasswordHash))
            {
                return null;
            }
            return user;
        }
        private bool Verify(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var unHashPassword = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));//System.Text.Encoding.UTF8.GetBytes(password) conveting the string to array of bytes
                for (int i = 0; i < unHashPassword.Length; i++)
                {
                    if (unHashPassword[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;

            }
        }
        public async Task<Users> Register(Users user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatPasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return user;
        }
        private void CreatPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));//System.Text.Encoding.UTF8.GetBytes(password) conveting the string to array of bytes
            }
        }
        public async Task<bool> UserExist(string userName)
        {
            if (await db.Users.AnyAsync(s => s.UserName == userName))
            {
                return true;
            }
            return false;
        }

        public  Task<bool> ChekToken(string token)
        {
            //  var tokenn = await HttpContext.GetTokenAsync("access_token");
            throw new NotImplementedException();
        }
    }
}
