using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaVirusApi.Models
{
    public class IDbContext : DbContext
    {
        public IDbContext(DbContextOptions<IDbContext> options) : base(options)
        {

        }
        public DbSet<Ques> Questions { get; set; }
        public DbSet<Statistics> statistics { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<BannedTokens> BannedTokens { get; set; }
    }
}
