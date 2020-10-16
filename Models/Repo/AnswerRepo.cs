using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaVirusApi.Models.Repo
{
    public class AnswerRepo : IRepo<Ques>
    {
        IDbContext db;
        public AnswerRepo(IDbContext _db)
        {
            db = _db;
        }

        public bool Check(int id, bool UserAns)
        {
            var Q = db.Questions.SingleOrDefault(s => s.id == id);

            if (UserAns == Q.ans)
            {
                return true;
            }
            return false;
        }
    }
}
