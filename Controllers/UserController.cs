using APIprojectBQU.Data;
using APIprojectBQU.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIprojectBQU.Controllers
{
    [Route("/api/controller")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly APIprojectDbContext aPIprojectDbContext;
        public UserController(APIprojectDbContext _APIprojectDbContext)
        {
            aPIprojectDbContext = _APIprojectDbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObject)
        {
            if (userObject == null)
                return BadRequest();

            await aPIprojectDbContext.Users.AddAsync(userObject);
            await aPIprojectDbContext.SaveChangesAsync();
                return Ok(new 
                { 
                    Message = "User registered" 
                });
            
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObject)
        {
            if (userObject == null)
            
                return BadRequest();

            var user = await aPIprojectDbContext.Users.FirstOrDefaultAsync(x => x.Email == userObject.Email && x.Password == userObject.Password);
            if (user == null)
                return NotFound(new { Message = "user not found" });
            
            return Ok(new 
            { 
                Message = "Login Success!" 
            });
        }
    }
}
