using Microsoft.AspNetCore.Mvc;
using System.IO; 

namespace DatingApp.API.Controllers
{
    // [Route("")]
    // [Route("Fallback")]
    // [Route("Fallback/Index")]
    public class FallbackController : Controller
    { 
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML"); 
        }
    }
}