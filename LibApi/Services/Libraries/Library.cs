using AppHelpers;
using AppHelpers.Dates;
using AppHelpers.Strings;
using LibApi.Helpers;
using LibApi.Models.Local.SQLite;
using LibApi.Services.Books;
using LibApi.Services.Categories;
using LibApi.Services.Collections;
using LibShared;
using LibShared.ViewModels.Libraries;
using Microsoft.EntityFrameworkCore;

namespace LibApi.Services.Libraries
{
    public sealed class Library : LibraryVM, IDisposable
	{
        /// <summary>
        /// Obtient une valeur booléenne indiquant si l'objet a déjà été effacé de la base de données
        /// </summary>
        public bool IsDeleted { get; private set; }

        readonly LibraryHelpers libraryHelpers = new();
        readonly LibrarySqLiteDbContext context = new ();

        private Library()
        {

        }

        #region Properties
        public new long Id
        {
            get => _Id;
            private set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged();
                }
            }
        }

        public new DateTime DateAjout
        {
            get => _DateAjout;
            private set
            {
                if (_DateAjout != value)
                {
                    _DateAjout = value;
                    OnPropertyChanged();
                }
            }
        }

        public new DateTime? DateEdition
        {
            get => _DateEdition;
            private set
            {
                if (_DateEdition != value)
                {
                    _DateEdition = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string Name
        {
            get => _Name;
            private set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string? Description
        {
            get => _Description;
            private set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// Obtient tous les livres de la bibliothèque trié par nom et par ordre croissant ou décroissant
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Tbook>> OrderBooksByNameAsync(OrderBy orderBy = OrderBy.Ascending) => await libraryHelpers.OrderAsync<Tbook>(SortBy.Name, orderBy);


        #region Static Methods
        /// <summary>
            /// Obtient toutes bibliothèques
            /// </summary>
            /// <returns></returns>
        public static async Task<IEnumerable<Library>> GetAllAsync()
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                var modelList = await context.Tlibraries.ToListAsync();
                if (modelList == null || !modelList.Any())
                {
                    return Enumerable.Empty<Library>();
                }

                return modelList.Select(s => ConvertToViewModel(s))!;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return Enumerable.Empty<Library>();
            }
        }

        /// <summary>
        /// Obtient une bibliothèque
        /// </summary>
        /// <param name="idLibrary"></param>
        /// <returns></returns>
        public static async Task<Library?> GetSingleAsync(long idLibrary)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                var item = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == idLibrary);
                if (item == null)
                {
                    return null;
                }
                return ConvertToViewModel(item);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }

        public static async Task<Library?> GetSingleAsync(string libraryName)
        {
            try
            {
                if (libraryName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(libraryName), "Le nom de la bibliothèque ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();
                var item = await context.Tlibraries.SingleOrDefaultAsync(s => s.Name.ToLower() == libraryName.Trim().ToLower());
                if (item == null)
                {
                    return null;
                }

                return ConvertToViewModel(item);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }

        public static async Task<IEnumerable<long>> DeleteAsync(IEnumerable<long> idList)
        {
            List<long> idsNotDeleted = new ();
            try
            {
                if (idList == null || !idList.Any())
                {
                    return idList;
                }

                using LibrarySqLiteDbContext context = new();
                foreach (long id in idList)
                {
                    if (!await DeleteAsync(context, id))
                    {
                        idsNotDeleted.Add(id);
                    }
                }
                
                return idsNotDeleted;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return idsNotDeleted;
            }
        }

        public static async Task<bool> DeleteAsync(LibrarySqLiteDbContext context, long id)
        {
            try
            {
                Tlibrary? tlibrary = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == id);
                if (tlibrary == null)
                {
                    throw new ArgumentNullException(nameof(Tlibrary), $"La bibliothèque n'existe pas avec l'id \"{id}\".");
                }

                List<Tcollection>? tcollections = await context.Tcollections.Where(s => s.IdLibrary == id).ToListAsync();
                if (tcollections.Any())
                {
                    context.Tcollections.RemoveRange(tcollections);
                }

                context.Tlibraries.Remove(tlibrary);
                _ = await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return false;
            }
        }

        /// <summary>
        /// Compte le nombre de bibliothèque dans la base de données
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                return await context.Tlibraries.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return 0;
            }
        }
        #endregion

        #region CRUD
        /// <summary>
        /// Ajoute une nouvelle bibliothèque dans la base de données puis retourne un objet <see cref="Library"/>
        /// </summary>
        /// <param name="name">Nom de la nouvelle bibliothèque</param>
        /// <param name="description"></param>
        /// <param name="openIfExist">[**Applicable uniquement si la bibliothèque existe déjà] : si true alors retourne la bibliotèque existante sinon lève une <see cref="InvalidOperationException"/>.</param>
        /// <returns></returns>
        public static async Task<Library?> CreateAsync(string name, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(name), "Le nom de la bibliothèque ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                return await CreateAsync(new LibraryVM()
                {
                    Name = name.Trim(),
                    Description = description?.Trim()
                }, openIfExist);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception:ex);
                return null;
            }
        }

        public static async Task<Library?> CreateAsync(LibraryVM viewModel, bool openIfExist = false)
        {
            try
            {
                if (viewModel == null)
                {
                    throw new ArgumentNullException(nameof(viewModel), "Le modèle de vue ne peut pas être null.");
                }

                if (viewModel.Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(viewModel.Name), "Le nom de la bibliothèque ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();
                Tlibrary? existingItem = await context.Tlibraries.SingleOrDefaultAsync(c => c.Name.ToLower() == viewModel.Name.Trim().ToLower());
                if (existingItem != null)
                {
                    Logs.Log(nameof(Library), nameof(CreateAsync), $"La bibliothèque {viewModel.Name} existe déjà.");
                    if (openIfExist)
                    {
                        return ConvertToViewModel(existingItem);
                    }
                    else
                    {
                        throw new InvalidOperationException($"La bibliothèque {viewModel.Name} existe déjà.");
                    }
                }

                var _dateAjout = DateTime.UtcNow;
                var _guid = System.Guid.NewGuid();

                Tlibrary record = new()
                {
                    DateAjout = _dateAjout.ToString(),
                    Guid = _guid.ToString(),
                    Name = viewModel.Name.Trim(),
                    Description = viewModel.Description?.Trim(),
                };

                await context.Tlibraries.AddAsync(record);
                await context.SaveChangesAsync();

                InputOutput inputOutput = new();
                inputOutput.GetOrCreateDefaultFolderItem(_guid, DefaultFolders.Libraries);

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }


        /// <summary>
        /// Met à jour la bibliothèque dans la base de données
        /// </summary>
        /// <remarks>Remarque : si aucun paramètre n'est renseigné alors une <see cref="InvalidOperationException"/> est levée et annule ainsi l'opération de mise à jour.</remarks>
        /// <param name="newName"></param>
        /// <param name="newDescription">Nouvelle description. Si le paramètre est null alors la modification de ce paramètre est ingoré.</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(string? newName, string? newDescription = null)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (newName.IsStringNullOrEmptyOrWhiteSpace() && newDescription == null)
                {
                    throw new InvalidOperationException("Le nouveau nom de la bibliothèque ou sa nouvelle description doit être renseignée.");
                }

                Tlibrary? tlibrary = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == Id);
                if (tlibrary == null)
                {
                    throw new NullReferenceException($"La bibliothèque n'existe pas avec l'id \"{Id}\".");
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    bool isExist = await context.Tlibraries.AnyAsync(c => c.Id != Id && c.Name.ToLower() == newName.Trim().ToLower())!;
                    if (isExist)
                    {
                        Logs.Log(nameof(Library), nameof(UpdateAsync), "Cette bibliothèque existe déjà");
                        return false;
                    }
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    tlibrary.Name = newName.Trim();
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (newDescription != null)
                {
                    tlibrary.Description = newDescription?.Trim();
                }

                DateTime dateEdition = DateTime.Now;
                tlibrary.DateEdition = dateEdition.ToString();

                context.Tlibraries.Update(tlibrary);
                _ = await context.SaveChangesAsync();

                DateEdition = dateEdition;

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Name = tlibrary.Name;
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (newDescription != null)
                {
                    Description = tlibrary.Description;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return false;
            }
        }

        /// <summary>
        /// Supprime la bibliothèque de la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            try
            {
                if (IsDeleted)
                {
                    throw new NotSupportedException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                Tlibrary? tlibrary = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == Id);
                if (tlibrary == null)
                {
                    throw new NullReferenceException($"La bibliothèque n'existe pas avec l'id \"{Id}\".");
                }

                _ = await Book.DeleteAsync(idLibrary: Id);
                _ = await Category.DeleteAsync(idLibrary: Id);
                _ = await Collection.DeleteAsync(idLibrary: Id);
                

                context.Tlibraries.Remove(tlibrary);
                _ = await context.SaveChangesAsync();

                IsDeleted = true;

                if (Guid.TryParse(tlibrary.Guid, out Guid guid))
                {
                    InputOutput inputOutput = new();
                    inputOutput.GetOrCreateDefaultFolderItem(guid, DefaultFolders.Libraries);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return false;
            }
        }

        #endregion

        #region Collections
        /// <summary>
        /// Ajoute une nouvelle collection à la bibliothèque.
        /// </summary>
        /// <param name="name">Nom de la collection</param>
        /// <param name="description">Description de la collection</param>
        /// <remarks>Si la collection existe, la collection existante sera retournée.</remarks>
        /// <returns></returns>
        public async Task<Collection?> CreateCollectionAsync(string name, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Collection.CreateAsync(Id, name, description, openIfExist);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }

        /// <summary>
        /// Récupère toutes les collections de la bibliothèque actuelle
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Collection>> GetAllCollectionsAsync()
        {
            try
            {
                if (IsDeleted)
                {
                    throw new NotSupportedException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Collection.AllAsync(idLibrary: Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return Enumerable.Empty<Collection>();
            }
        } 

        public async Task<Collection?> GetSingleCollectionAsync(long idCollection)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Collection.SingleAsync(id: idCollection, idLibrary: Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }

        public async Task<Collection?> GetSingleCollectionAsync(string collectionName)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Collection.SingleAsync(name: collectionName, idLibrary: Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }

        public async Task<IEnumerable<Collection>> GetMultipleCollectionsAsync(string[] collectionNames)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Collection.MultiplesAsync(names: collectionNames, idLibrary: Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return Enumerable.Empty<Collection>();
            }
        }

        public async Task<IEnumerable<Collection>> GetMultipleCollectionsAsync(long[] idCollections)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Collection.MultiplesAsync(idArray: idCollections, idLibrary: Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return Enumerable.Empty<Collection>();
            }
        }

        #endregion

        #region Catégories
        /// <summary>
        /// Ajoute une nouvelle catégorie à la bibliothèque.
        /// </summary>
        /// <param name="name">Nom de la collection</param>
        /// <param name="description">Description de la collection</param>
        /// <remarks>Si la collection existe, la collection existante sera retournée.</remarks>
        /// <returns></returns>
        public async Task<Category?> CreateCategoryAsync(string name, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Category.CreateAsync(idLibrary: Id, name: name, description: description, idParentCategorie: null, openIfExist: openIfExist);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }

        public async Task<Category?> GetSingleCategoryAsync(long idCategory)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Category.SingleAsync(idCategory, Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }

        public async Task<Category?> GetSingleCategoryAsync(string categoryName)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Category.SingleAsync(categoryName, Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }

        public async Task<IEnumerable<Category>> GetMultipleCategoriesAsync(string[] categoriesNames)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Category.MultiplesAsync(names: categoriesNames, idLibrary: Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return Enumerable.Empty<Category>();
            }
        }

        public async Task<IEnumerable<Category>> GetMultipleCategoriesAsync(long[] idCategories)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Category.MultiplesAsync(idArray: idCategories, idLibrary: Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return Enumerable.Empty<Category>();
            }
        }

        /// <summary>
        /// Obtient toute l'arborescence des catégories enfants de cette bibliothèque.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Category>> GetCategoriesTreeAsync()
        {
            try
            {
                List<TlibraryCategorie> Parentcategories = await context.TlibraryCategories.Where(w => w.IdParentCategorie == null && w.IdLibrary == Id).ToListAsync();
                if (Parentcategories != null && Parentcategories.Any())
                {
                    List<Category?> categories = Parentcategories.Select(s => Category.ConvertToViewModel(s)).Where(w => w != null).ToList();
                    foreach (var category in categories)
                    {
                        if (category != null)
                        {
                            await Category.GetChildCategoriesAsync(category, Id);
                        }
                    }

                    return categories;
                }

                return Enumerable.Empty<Category>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return Enumerable.Empty<Category>();
            }
        }
        #endregion

        #region Livres
        /// <summary>
        /// Récupère tous les libres de la bibliothèque actuelle
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Book.AllAsync(idLibrary: Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return Enumerable.Empty<Book>();
            }
        }

        /// <summary>
        /// Récupère le premier livre à partir des paramètres 
        /// </summary>
        /// <param name="titleName">Titre du livre</param>
        /// <param name="lang">Langue du livre</param>
        /// <param name="format">Format du livre</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Book?> GetSingleBookAsync(string titleName, string? lang = null, BookFormat? format = null)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await Book.SingleAsync(titleName:titleName, lang:lang, format:format, idLibrary:Id);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }

        /// <summary>
        /// Crée un livre puis l'ajoute à la bibliothèque.
        /// </summary>
        /// <returns></returns>
        public async Task<Book?> CreateBookAsync(string title, string? lang = null, BookFormat? format = null, string? notes = null, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                if (title.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(title), "Le nom du livre ne peut pas être nul, vide ou ne contenir que des espaces blancs.");
                }

                return await Book.CreateAsync(Id, title, lang, format, notes, description, openIfExist);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Library), exception: ex);
                return null;
            }
        }


        /// <summary>
        /// Compte le nombre de livres dans cette bibliothèque
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> CountBooksAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new NotSupportedException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                return await context.Tbooks.CountAsync(w => w.IdLibrary == Id, cancellationToken);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return 0;
            }
        }

        #endregion


        /// <summary>
        /// Convertit un model en Model de vue
        /// </summary>
        /// <param name="model">Modèle de données</param>
        /// <returns></returns>
        public static Library? ConvertToViewModel(Tlibrary model)
        {
            try
            {
                if (model == null) return null;

                var isGuidCorrect = Guid.TryParse(model.Guid, out Guid guid);
                if (isGuidCorrect == false) return null;

                Library viewModel = new ()
                {
                    Id = model.Id,
                    DateAjout = DateHelpers.Converter.GetDateFromString(model.DateAjout),
                    DateEdition = DateHelpers.Converter.GetNullableDateFromString(model.DateEdition),
                    Description = model.Description,
                    Name = model.Name,
                    Guid = isGuidCorrect ? guid : Guid.Empty,
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), exception: ex);
                return null;
            }
        }

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }

        public async Task DisposeAsync()
        {
            if (context != null)
            {
                await context.DisposeAsync();
            }
        }
    }
}

