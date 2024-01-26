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
        private readonly APIprojectDbContext aPIprojectDbContext;

        public BooksController(APIprojectDbContext _APIprojectDbContext)
        {
           aPIprojectDbContext = _APIprojectDbContext;
        }
        //Get method to get all books in database
        [HttpGet]
        public async Task <IActionResult> GetAllBooks() 
        {
           var books = await aPIprojectDbContext.Books.ToListAsync();
            return Ok(books);
        }
        //Post method for adding new book
        [HttpPost]
        public async Task <IActionResult> AddBook([FromBody] Book bookRequest) 
        {
            bookRequest.Id = Guid.NewGuid();

            await aPIprojectDbContext.Books.AddAsync(bookRequest);
            await aPIprojectDbContext.SaveChangesAsync();

            return Ok(bookRequest);
        }

        //Get method for one book
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task <IActionResult> GetBook([FromRoute]Guid id)
        {
           var book= await aPIprojectDbContext.Books.FirstOrDefaultAsync(x => x.Id == id);

            if(book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        //Put method for edit one book
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateBook([FromRoute] Guid id, Book updateBookRequest)
        {
           var book = await aPIprojectDbContext.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }
            book.Title = updateBookRequest.Title;
            book.Author = updateBookRequest.Author;
            book.Genre = updateBookRequest.Genre;
            book.Published = updateBookRequest.Published;

            await aPIprojectDbContext.SaveChangesAsync();

            return Ok(book);
        }
        //Delete method for deleting a specific book
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteBook([FromRoute] Guid id)
        {
            var book = await aPIprojectDbContext.Books.FindAsync(id);
            if(book == null)
            {
                return NotFound();
            }
            aPIprojectDbContext.Books.Remove(book);
            await aPIprojectDbContext.SaveChangesAsync();
            return Ok(book);
        }
    }
}
