using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoronaVirusApi.Dtos;
using CoronaVirusApi.Models;
using CoronaVirusApi.Models.Repo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoronaVirusApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        IDbContext db;
        private readonly IRepo<Ques> ARepo;
        private readonly IConfiguration config;
        private readonly IUserRepo<Users> userRepo;


        public MainController(IRepo<Ques> _ARepo, IConfiguration _config, IDbContext _db, IUserRepo<Users> _userRepo)
        {
            db = _db;
            ARepo = _ARepo;
            config = _config;
            userRepo = _userRepo;

        }
        // api/Main/Statistics
        [AllowAnonymous]

        [HttpGet("Statistics")]

        public async Task<IList<Statistics>> Statistics()
        {
            var st = await db.statistics.ToListAsync();
            return st;
        }

        //api/Main/Questions

        [HttpGet("Questions")]
       //IList<Ques>
        public async Task<IActionResult> GetQues()
        {
            var QList = await db.Questions.ToListAsync();
       
            if (TokenIsValid().Result== false)
            {
                return Unauthorized();
            }       
            return Ok (QList);
        }


        //get qusetion by id
        //api/Main/2
        [HttpGet("Questions/{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            if (TokenIsValid().Result == false)
            {
                return Unauthorized();
            }
            var q = await db.Questions.FirstOrDefaultAsync(s => s.id == id);
            if (q == null)
            {
                return NotFound("Id not found");
            }
 
            return Ok(q);
        }

        //api/Main/Answer

        [HttpPost("Answer")]
        public async Task<IActionResult> Answer(AnswerDto userAns)
        {
            if (TokenIsValid().Result == false)
            {
                return Unauthorized();
            }
            bool check = ARepo.Check(userAns.id, userAns.ans);
            if (check == true)
            {
                var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await userRepo.Find(userId);
                user.RAnswers++;
                await db.SaveChangesAsync();
            }

            return await Task.FromResult(Ok(check));
        }

        // api/Main/result

        [HttpGet("result")]
        public async Task<IActionResult> RaightAns()
        {
            if (TokenIsValid().Result == false)
            {
                return Unauthorized();
            }
       
            var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var RaightAnswers = await userRepo.Find(userId);
            var counter = RaightAnswers.RAnswers;
            RaightAnswers.RAnswers = 0;
            await db.SaveChangesAsync();
            return Ok(counter);
   
        }

        //api/Main/Statistics/add
        [AllowAnonymous]

        [HttpPost("Statistics/add")]
        public async Task<ActionResult> StatAdd(Statistics S)
        {
            var NewS = new Statistics { country = S.country, cases = S.cases, cured = S.cured, deaths = S.deaths };
            await db.statistics.AddAsync(NewS);
            await db.SaveChangesAsync();
            return Ok();
        }

        //api/Main/add
        [AllowAnonymous]

        [HttpPost("Questions/Add")]
        public async Task<ActionResult> Add(Ques Q)
        {

            var NewQ = new Ques { question = Q.question, ans = Q.ans };
            await db.Questions.AddAsync(NewQ);
            await db.SaveChangesAsync();
            return Ok();
        }

        public async Task< bool> TokenIsValid()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var isBan = await db.BannedTokens.SingleOrDefaultAsync(r => r.Token == token);
            if (isBan != null)
            {
                return false;
            }
            return true;
        }
    }
}