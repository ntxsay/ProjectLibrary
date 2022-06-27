using LibApi.Services.Categories;
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
                _logger.LogWarning("La catégorie n'existe pas.");
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
    }
}
