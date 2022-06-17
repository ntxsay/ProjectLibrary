using AppHelpers.Strings;
using LibApi.Services.Libraries;
using LibShared.ViewModels.Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrariesController : ControllerBase
    {
        private readonly ILogger<LibrariesController> _logger;

        public LibrariesController(ILogger<LibrariesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "{id}")]
        public async Task<IEnumerable<LibraryVM>> GetAsync(long? id = null)
        {
            if (id == null)
            {
                IEnumerable<Library>? all = await Library.GetAllAsync();
                return all ?? Enumerable.Empty<LibraryVM>();
            }
            else
            {
                Library? library = await Library.GetSingleAsync((long)id);
                if (library == null)
                {
                    _logger.LogWarning("La bibliothèque n'existe pas.");
                    return Enumerable.Empty<LibraryVM>();
                }

                return new LibraryVM[] { library };
            }
            
            
        }

        [HttpPost(Name = "Create")]
        public async Task<long> CreateFromVMAsync(LibraryVM viewModel)
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

        //[HttpPost(Name = "Create2")]
        //public async Task<long> CreateAsync([FromQuery] string Name, string? Description = null)
        //{
        //    if (Name.IsStringNullOrEmptyOrWhiteSpace())
        //    {
        //        _logger.LogWarning("Le nom de la bibliothèque ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
        //        return 0;
        //    }

        //    Library? library = await Library.CreateAsync(Name, Description);
        //    if (library == null)
        //    {
        //        _logger.LogWarning("La biliothèque n'a pas pu être créée.");
        //        return 0;
        //    }

        //    return library.Id;
        //}

       

        [HttpPatch(Name = "edit/{id}")]
        public async Task<bool> EditAsync(long id, string? newName, string? newDescription = null)
        {
            if (newName.IsStringNullOrEmptyOrWhiteSpace() && newDescription == null)
            {
                _logger.LogWarning("Le nouveau nom de la bibliothèque ou sa nouvelle description doit être renseignée.");
                return false;
            }

            Library? library = await Library.GetSingleAsync(id);
            if (library == null)
            {
                _logger.LogWarning("La bibliothèque n'existe pas.");
                return false;
            }

            return true;
        }

        [HttpDelete(Name = "delete/{id}")]
        [ActionName("delete/{id}")]
        public async Task<bool> DeleteAsync(IEnumerable<long> id)
        {
            if (id != null && id.Any())
            {
                IEnumerable<long> idNotDeleted = await Library.DeleteAsync(id);
                if (idNotDeleted != null && idNotDeleted.Any())
                {
                    _logger.LogWarning($"{idNotDeleted.Count()} bibliothèque(s) n'a ou n'ont pas été supprimées.");
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}
