using LibApi.Services.Libraries;
using LibShared.ViewModels.Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly ILogger<LibraryController> _logger;

        public LibraryController(ILogger<LibraryController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "All")]
        public async Task<IEnumerable<LibraryVM>> GetAllAsync()
        {
            IEnumerable<Library>? all = await Library.GetAllAsync();
            return all;
        }

        [HttpPost(Name = "Create")]
        public async Task<long> CreateAsync(LibraryVM viewModel)
        {
            if (viewModel == null)
            {
                _logger.LogWarning("Le modèle de vue n'est pas valide.");
                return 0;
            }

            Library? library = await Library.CreateAsync(viewModel);
            if (library == null)
            {
                _logger.LogWarning("La biliothèque n'a pas pu être créée.");
                return 0;
            }
            
            return library.Id;
        }
    }
}
