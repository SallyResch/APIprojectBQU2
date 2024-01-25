using APIprojectBQU.Data;
using APIprojectBQU.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace APIprojectBQU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly APIprojectDbContext _aPIprojectDbContext;

        public BooksController(APIprojectDbContext aPIprojectDbContext)
        {
            _aPIprojectDbContext = aPIprojectDbContext;
        }

        [HttpGet]
        public async Task <IActionResult> GetAllBooks() 
        {
           var books = await _aPIprojectDbContext.Books.ToListAsync();
           return Ok(books);
        }
        [HttpPost]
        public async Task <IActionResult> AddBook([FromBody] Book bookRequest) 
        {
            bookRequest.Id = Guid.NewGuid();

            await _aPIprojectDbContext.Books.AddAsync(bookRequest);
            await _aPIprojectDbContext.SaveChangesAsync();

            return Ok(bookRequest);
        }
    }
}
