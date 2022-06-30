using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using LibApi.Services.Collections;
using LibApi.Services.Libraries;
using LibShared.ViewModels.Collections;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibWebApi.Controllers
{
    [Route("api/v2/[controller]")]
    public class CollectionsController : Controller
    {
        private readonly ILogger<CollectionsController> _logger;
        //private readonly LibrarySqLiteDbContext librarySqLiteDbContext;
        //public CollectionsController(ILogger<CollectionsController> logger, LibrarySqLiteDbContext _librarySqLiteDbContext)
        //{
            //_logger = logger;
            //librarySqLiteDbContext = _librarySqLiteDbContext;
        //}

        public CollectionsController(ILogger<CollectionsController> logger)
        {
            _logger = logger;
        }

        [Route("single")]
        [HttpGet]
        public async Task<CollectionVM?> GetAsync(long id)
        {
            using Collection? collection = await Collection.SingleAsync(id);
            if (collection == null)
            {
                _logger.LogError("La collection n'existe pas.");
                return null;
            }

            return collection;
        }

        [Route("all")]
        [HttpGet]
        public async Task<IEnumerable<CollectionVM>> GetAsync()
        {
            IEnumerable<CollectionVM>? all = await Collection.AllAsync();
            return all ?? Enumerable.Empty<CollectionVM>();
        }

        [Route("create")]
        [HttpPost]
        public async Task<CollectionVM?> CreateCollectionAsync([FromQuery] long idLibrary, [FromQuery] string Name, [FromQuery] string? Description = null)
        {
            if (Name.IsStringNullOrEmptyOrWhiteSpace())
            {
                _logger.LogError("Le nom de la collection ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                return null;
            }

            using Library? library = await Library.GetSingleAsync(idLibrary);
            if (library == null)
            {
                _logger.LogError("La bibliothèque n'existe pas.");
                return null;
            }

            using Collection? collection = await library.CreateCollectionAsync(Name, Description);
            if (collection == null)
            {
                _logger.LogError("La collection n'a pas pu être créée.");
                return null;
            }

            return collection;
        }

        [Route("edit")]
        [HttpPut]
        public async Task<CollectionVM?> EditAsync([FromQuery] long id, [FromQuery] string? newName, [FromQuery] string? newDescription = null)
        {
            if (newName.IsStringNullOrEmptyOrWhiteSpace() && newDescription == null)
            {
                _logger.LogWarning("Le nouveau nom de la collection ou sa nouvelle description doit être renseignée.");
                return null;
            }

            using Collection? collection = await Collection.SingleAsync(id);
            if (collection == null)
            {
                _logger.LogWarning("La collection n'existe pas.");
                return null;
            }

            bool result = await collection.UpdateAsync(newName, newDescription);
            if (result == false)
            {
                return null;
            }

            return collection;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> DeleteAsync([FromQuery] long id)
        {
            using Collection? collection = await Collection.SingleAsync(id);
            if (collection == null)
            {
                _logger.LogWarning("La collection n'existe pas.");
                return false;
            }

            return await collection.DeleteAsync();
        }
    }
}

