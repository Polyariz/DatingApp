using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{  
    [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
    //GET http://localhost:5000/api/values/5
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;
        }
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values = await  _context.Values.ToListAsync();

            return Ok(values);
        }
        //public IActionResult GetValues()
        //{
        //    var values = _context.Values.ToList();

        //    return Ok(values);
        //}
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    //throw new Exception("Test Exceptuon");
        //    return new string[] { "value1", "value2" };
        //}

        [AllowAnonymous]
        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var values = await _context.Values.FirstOrDefaultAsync(x => x.Id == id); 
            return Ok(values);
        }
        //public IActionResult Get(int id)
        //{
        //    var values = _context.Values.FirstOrDefault(x=>x.Id==id);

        //    return Ok(values);
        //}
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    //throw new Exception("Test Exception");
        //    return "value";
        //}

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
