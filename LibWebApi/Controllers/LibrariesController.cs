using AppHelpers.Strings;
using LibApi.Extensions;
using LibApi.Services.Books;
using LibApi.Services.Categories;
using LibApi.Services.Collections;
using LibApi.Services.Libraries;
using LibShared;
using LibShared.ViewModels.Books;
using LibShared.ViewModels.Categories;
using LibShared.ViewModels.Collections;
using LibShared.ViewModels.Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibWebApi.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class LibrariesController : ControllerBase
    {
        private readonly ILogger<LibrariesController> _logger;

        public LibrariesController(ILogger<LibrariesController> logger)
        {
            _logger = logger;
        }

        [Route("single")]
        [HttpGet]
        public async Task<IEnumerable<LibraryVM>> GetAsync(long id)
        {
            using Library? library = await Library.GetSingleAsync(id);
            if (library == null)
            {
                _logger.LogWarning("La bibliothèque n'existe pas.");
                return Enumerable.Empty<LibraryVM>();
            }

            return new LibraryVM[] { library };
        }

        [Route("all")]
        [HttpGet]
        public async Task<IEnumerable<LibraryVM>> GetAsync()
        {
            IEnumerable<Library>? all = await Library.GetAllAsync();
            return all ?? Enumerable.Empty<LibraryVM>();
        }

        [Route("all/ordered")]
        [HttpGet]
        public async Task<LibraryRequestVM?> GetAsync(OrderBy orderBy, SortBy sortBy, int maxItemsPerPage = 20, int gotoPage = 1)
        {
            try
            {
                IEnumerable<Library>? all = await Library.GetAllAsync();
                if (all != null && all.Any())
                {
                    IEnumerable<Library>? orderedItems = all.OrderItemsBy(orderBy, sortBy);
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

                        IEnumerable<Library>? displayedItem = orderedItems.DisplayPage(maxItemsPerPage, gotoPage);
                        return new LibraryRequestVM()
                        {
                            List = displayedItem ?? Enumerable.Empty<LibraryVM>(),
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

        [Route("first")]
        [HttpGet]
        public async Task<LibraryVM?> GetFirstAsync()
        {
            IEnumerable<Library>? all = await Library.GetAllAsync();
            return all.FirstOrDefault() ?? null;
        }

        [Route("collections/all")]
        [HttpGet]
        public async Task<IEnumerable<CollectionVM>> GetCollectionsAsync(long idLibrary)
        {
            using Library? library = await Library.GetSingleAsync(idLibrary);
            if (library == null)
            {
                return Enumerable.Empty<CollectionVM>();
            }
            return await library.GetAllCollectionsAsync();
        }

        [Route("books/all")]
        [HttpGet]
        public async Task<IEnumerable<BookVM>> GetBooksAsync(long idLibrary)
        {
            using Library? library = await Library.GetSingleAsync(idLibrary);
            if (library == null)
            {
                return Enumerable.Empty<BookVM>();
            }
            return await library.GetAllBooksAsync();
        }

        [Route("books/single")]
        [HttpGet]
        public async Task<BookVM?> GetSingleBookAsync([FromQuery] long idLibrary, [FromQuery] string title, [FromQuery] string? lang = null, [FromQuery] BookFormat? format = null)
        {
            if (title.IsStringNullOrEmptyOrWhiteSpace())
            {
                _logger.LogWarning("Le titre du livre ne peut pas être null, vide ou ne contenir que des espaces blancs.");
                return null;
            }

            using Library? library = await Library.GetSingleAsync(idLibrary);
            if (library == null)
            {
                _logger.LogWarning("Le livre n'a pas pu être trouvé.");
                return null;
            }

            using Book? book = await library.GetSingleBookAsync(title, lang, format);
            return book;
        }

        [Route("create/view-model")]
        [HttpPost]
        public async Task<LibraryVM?> CreateFromVMAsync(LibraryVM viewModel)
        {
            if (viewModel == null)
            {
                _logger.LogWarning("Le modèle de vue n'est pas valide.");
                return null;
            }

            using Library? library = await Library.CreateAsync(viewModel);
            if (library == null)
            {
                _logger.LogWarning("La bibliothèque n'a pas pu être créée.");
                return null;
            }

            return library;
        }

        [Route("create")]
        [HttpPost]
        public async Task<LibraryVM?> CreateAsync([FromQuery] string Name, [FromQuery] string? Description = null)
        {
            if (Name.IsStringNullOrEmptyOrWhiteSpace())
            {
                _logger.LogWarning("Le nom de la bibliothèque ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                return null;
            }

            using Library? library = await Library.CreateAsync(Name, Description);
            if (library == null)
            {
                _logger.LogWarning("La biliothèque n'a pas pu être créée.");
                return null;
            }

            return library;
        }

        [Route("collections/create")]
        [HttpPost]
        public async Task<CollectionVM?> CreateCollectionAsync([FromQuery] long idLibrary, [FromQuery] string Name, [FromQuery] string? Description = null)
        {
            if (Name.IsStringNullOrEmptyOrWhiteSpace())
            {
                _logger.LogWarning("Le nom de la collection ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                return null;
            }

            using Library? library = await Library.GetSingleAsync(idLibrary);
            if (library == null)
            {
                return null;
            }

            using Collection? collection = await library.CreateCollectionAsync(Name, Description);
            if (collection == null)
            {
                _logger.LogWarning("La collection n'a pas pu être créée.");
                return null;
            }

            return collection;
        }

        [Route("categories/create")]
        [HttpPost]
        public async Task<CategoryVM?> CreateCategoryAsync([FromQuery] long idLibrary, [FromQuery] string Name, [FromQuery] string? Description = null)
        {
            if (Name.IsStringNullOrEmptyOrWhiteSpace())
            {
                _logger.LogWarning("Le nom de la catégorie ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                return null;
            }

            using Library? library = await Library.GetSingleAsync(idLibrary);
            if (library == null)
            {
                return null;
            }

            using Category? category = await library.CreateCategoryAsync(Name, Description);
            if (category == null)
            {
                _logger.LogWarning("La categorie n'a pas pu être créée.");
                return null;
            }

            return category;
        }

        [Route("books/create")]
        [HttpPost]
        public async Task<BookVM?> CreateBookAsync([FromQuery] long idLibrary, [FromQuery] string title, [FromQuery] string? lang = null, [FromQuery] BookFormat? format = null, [FromQuery] string? Description = null)
        {
            if (title.IsStringNullOrEmptyOrWhiteSpace())
            {
                _logger.LogWarning("Le titre du livre ne peut pas être null, vide ou ne contenir que des espaces blancs.");
                return null;
            }

            using Library? library = await Library.GetSingleAsync(idLibrary);
            if (library == null)
            {
                _logger.LogWarning("Le livre n'a pas pu être créée.");
                return null;
            }

            using Book? book = await library.CreateBookAsync(title, lang, format);
            return book;
        }

        [Route("edit")]
        [HttpPut]
        public async Task<LibraryVM?> EditAsync([FromQuery] long id, [FromQuery] string? newName, [FromQuery] string? newDescription = null)
        {
            if (newName.IsStringNullOrEmptyOrWhiteSpace() && newDescription == null)
            {
                _logger.LogWarning("Le nouveau nom de la bibliothèque ou sa nouvelle description doit être renseignée.");
                return null;
            }

            using Library? library = await Library.GetSingleAsync(id);
            if (library == null)
            {
                _logger.LogWarning("La bibliothèque n'existe pas.");
                return null;
            }

            var result = await library.UpdateAsync(newName, newDescription);
            if (result == false)
            {
                return null;
            }
            return library;
        }

        //[HttpDelete(Name = "{id}")]
        //public async Task<bool> DeleteAsync(IEnumerable<long> id)
        //{
        //    if (id != null && id.Any())
        //    {
        //        IEnumerable<long> idNotDeleted = await Library.DeleteAsync(id);
        //        if (idNotDeleted != null && idNotDeleted.Any())
        //        {
        //            _logger.LogWarning($"{idNotDeleted.Count()} bibliothèque(s) n'a ou n'ont pas été supprimées.");
        //            return false;
        //        }

        //        return true;
        //    }
        //    return false;
        //}
    }
}
