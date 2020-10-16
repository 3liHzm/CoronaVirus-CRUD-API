using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaVirusApi.Models.Repo
{
    public interface IRepo<TEntity>
    {
        //bool Answer(int id, bool ans);
        bool Check(int id, bool UserAns);

    }
}
