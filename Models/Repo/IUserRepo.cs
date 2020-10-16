
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaVirusApi.Models.Repo
{
    public interface IUserRepo<TEntity>
    {
        Task<TEntity> Register(Users user, string password);
        Task<TEntity> Login(string userName, string password);
        Task<bool> UserExist(string userName);
        Task<TEntity> Find(int id);
        Task<bool> ChekToken(string token);
    }
}
