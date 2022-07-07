using LibApi.Extensions;
using LibApi.Services.Books;
using LibApi.Services.Categories;
using LibApi.Services.Libraries;
using LibShared;
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

        [Route("all/ordered")]
        [HttpGet]
        public async Task<BookRequestVM?> GetAsync(long idLibrary, OrderBy orderBy, SortBy sortBy, int maxItemsPerPage = 20, int gotoPage = 1)
        {
            try
            {
                IEnumerable<Book>? all = await Book.AllAsync(idLibrary);
                if (all != null && all.Any())
                {
                    IEnumerable<Book>? orderedItems = all.OrderItemsBy(orderBy, sortBy);
                    if (orderedItems != null && orderedItems.Any())
                    {
                        int countPage = orderedItems.CountPages(maxItemsPerPage);
                        if (gotoPage > countPage)
                        {
                            gotoPage = countPage;
                        }
                        else if (gotoPage < countPage)
                        {
                            gotoPage = 1;
                        }

                        IEnumerable<Book>? displayedItem = orderedItems.DisplayPage(maxItemsPerPage, gotoPage);
                        return new BookRequestVM()
                        {
                            List = displayedItem ?? Enumerable.Empty<BookVM>(),
                            CurrentPage = gotoPage,
                            NbPages = countPage,
                        };
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
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

        [Route("single/categorie/add")]
        [HttpPost]
        public async Task<bool> AddToCategoryAsync([FromQuery] long idBook, [FromQuery] long idCategory)
        {
            using Book? book = await Book.SingleAsync(idBook);
            if (book == null)
            {
                _logger.LogWarning("Le livre n'a pas pu être trouvé.");
                return false;
            }

            using Category? category = await Category.SingleAsync(idCategory);
            if (category == null)
            {
                _logger.LogWarning("La catégorie n'a pas pu être trouvé.");
                return false;
            }

            if (book.IdLibrary != category.IdLibrary)
            {
                _logger.LogWarning("Impossible d'ajouter ce livre à une catégorie provenant d'une autre bibliothèque.");
                return false;
            }

            return await book.AddToCategoryAsync(category);
        }
    }
}
