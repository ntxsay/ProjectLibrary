using AppHelpers.Strings;
using LibApi.Services.Categories;
using LibApi.Services.Libraries;
using LibShared.ViewModels.Categories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibWebApi.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ILogger<CategoriesController> logger)
        {
            _logger = logger;
        }

        [Route("single")]
        [HttpGet]
        public async Task<CategoryVM?> GetAsync(long id)
        {
            using Category? category = await Category.SingleAsync(id);
            if (category == null)
            {
                _logger.LogError("La catégorie n'existe pas.");
                return null;
            }

            return category;
        }

        [Route("all")]
        [HttpGet]
        public async Task<IEnumerable<CategoryVM>> GetAsync()
        {
            IEnumerable<CategoryVM>? all = await Category.AllAsync();
            return all ?? Enumerable.Empty<CategoryVM>();
        }

        [Route("create")]
        [HttpPost]
        public async Task<CategoryVM?> CreateCollectionAsync([FromQuery] long idLibrary, [FromQuery] string Name, [FromQuery] string? Description = null)
        {
            if (Name.IsStringNullOrEmptyOrWhiteSpace())
            {
                _logger.LogError("Le nom de la catégorie ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                return null;
            }

            using Library? library = await Library.GetSingleAsync(idLibrary);
            if (library == null)
            {
                _logger.LogError("La bibliothèque n'existe pas.");
                return null;
            }

            Category? category = await library.CreateCategoryAsync(Name, Description);
            if (category == null)
            {
                _logger.LogError("La catégorie n'a pas pu être créée.");
                return null;
            }

            return category;
        }
    }
}
