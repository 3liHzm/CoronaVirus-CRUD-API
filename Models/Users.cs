using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaVirusApi.Models
{
    public class Users
    {
       public int Id { get; set; }
        public string UserName { get; set; }
        public string password { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int RAnswers { get; set; }
    }
}
