using LibApi.Services.Books;
using LibApi.Services.Categories;
using LibApi.Services.Libraries;
using LibShared.ViewModels.Books;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibWebApi.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
        }

        [Route("single")]
        [HttpGet]
        public async Task<BookVM?> GetAsync(long id)
        {
            using Book? book = await Book.SingleAsync(id);
            if (book == null)
            {
                _logger.LogWarning("Le livre n'existe pas.");
                return null;
            }

            return book;
        }

        [Route("all")]
        [HttpGet]
        public async Task<IEnumerable<BookVM>> GetAsync()
        {
            IEnumerable<Book>? all = await Book.AllAsync();
            return all ?? Enumerable.Empty<BookVM>();
        }

        [Route("single/categorie")]
        [HttpGet]
        public async Task<Category?> GetSingleCategoryAsync([FromQuery] long idBook)
        {
            using Book? book = await Book.SingleAsync(idBook);
            if (book == null)
            {
                _logger.LogWarning("Le livre n'a pas pu être trouvé.");
                return null;
            }

            using Category? category = await book.GetCategorieAsync();
            return category;
        }
    }
}
