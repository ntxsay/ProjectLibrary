using System;
using System.Diagnostics;
using AppHelpers;
using AppHelpers.Serialization;
using AppHelpers.Strings;
using LibApi.Helpers;
using LibApi.Models.Local.SQLite;
using LibShared;
using LibShared.ViewModels.Libraries;
using Microsoft.EntityFrameworkCore;
using LibApi.Extensions;
using LibShared.ViewModels;
using LibApi.Services.Collections;
using AppHelpers.Dates;
using LibApi.Services.Categories;
using LibShared.ViewModels.Categories;
using LibApi.Services.Books;

namespace LibApi.Services.Libraries
{
	public sealed class Library : LibraryVM, IDisposable
	{
        /// <summary>
        /// Obtient une valeur booléenne indiquant si l'objet a déjà été effacé de la base de données
        /// </summary>
        public bool IsDeleted { get; private set; }

        readonly LibraryHelpers libraryHelpers = new();
        LibrarySqLiteDbContext context = new ();

        #region Constructeurs
        /// <summary>
        /// Possibilité d'instancier ce constructeur qu'en interne.
        /// </summary>
        private Library()
        {

        }

        private Library(LibraryVM viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel), "Le modèle de données ne peut pas être nulle.");
            }

            Id = viewModel.Id;
            Guid = viewModel.Guid;
            DateAjout = viewModel.DateAjout;
            DateEdition = viewModel.DateEdition;
            Name = viewModel.Name;
            Description = viewModel.Description;
        }

        #endregion

        #region Properties
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
                Logs.Log(nameof(Library), nameof(GetAllAsync), ex);
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
                Logs.Log(nameof(Library), nameof(GetSingleAsync), ex);
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
                Logs.Log(nameof(Library), nameof(GetSingleAsync), ex);
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
                Logs.Log(nameof(Library), nameof(DeleteAsync), ex);
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
                Logs.Log(nameof(Library), nameof(DeleteAsync), ex);
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
                Logs.Log(nameof(Library), nameof(CountAsync), ex);
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

                using LibrarySqLiteDbContext context = new();
                Tlibrary? existingItem = await context.Tlibraries.SingleOrDefaultAsync(c => c.Name.ToLower() == name.Trim().ToLower());
                if (existingItem != null)
                {
                    Logs.Log(nameof(Library), nameof(CreateAsync), $"La bibliothèque {name} existe déjà.");
                    if (openIfExist)
                    {
                        return ConvertToViewModel(existingItem);
                    }
                    else
                    {
                        throw new InvalidOperationException($"La bibliothèque {name} existe déjà.");
                    }
                }

                var _dateAjout = DateTime.UtcNow;
                var _guid = System.Guid.NewGuid();

                Tlibrary record = new()
                {
                    DateAjout = _dateAjout.ToString(),
                    Guid = _guid.ToString(),
                    Name = name.Trim(),
                    Description = description?.Trim(),
                };

                await context.Tlibraries.AddAsync(record);
                await context.SaveChangesAsync();

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(CreateAsync), ex);
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

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(CreateAsync), ex);
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
                Logs.Log(nameof(Library), nameof(UpdateAsync), ex);
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
                    throw new ArgumentNullException(nameof(Tlibrary), $"La bibliothèque n'existe pas avec l'id \"{Id}\".");
                }

                List<Tcollection>? tcollections = await context.Tcollections.Where(s => s.IdLibrary == Id).ToListAsync();
                if (tcollections.Any())
                {
                    context.Tcollections.RemoveRange(tcollections);
                }

                context.Tlibraries.Remove(tlibrary);
                _ = await context.SaveChangesAsync();

                IsDeleted = true;
                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(DeleteAsync), ex);
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
        public async Task<Collection?> AddCollectionAsync(string name, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                if (name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(name), "Le nom de la collection ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                Tcollection? existingItem = await context.Tcollections.SingleOrDefaultAsync(c => c.Name.ToLower() == name.ToLower() && c.IdLibrary == Id);
                if (existingItem != null)
                {
                    Logs.Log(nameof(Library), nameof(AddCollectionAsync), "Cette collection existe déjà");
                    if (openIfExist)
                    {
                        return Collection.ViewModelConverter(existingItem);
                    }
                    else
                    {
                        throw new InvalidOperationException($"La collection {name} existe déjà.");
                    }
                }

                Tcollection record = new()
                {
                    Name = name.Trim(),
                    IdLibrary = Id,
                    Description = description?.Trim(),
                };

                await context.Tcollections.AddAsync(record);
                await context.SaveChangesAsync();

                return Collection.ViewModelConverter(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(AddCollectionAsync), ex);
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

                var modelList = await context.Tcollections.Where(w => w.IdLibrary == Id).ToListAsync();
                if (modelList == null || !modelList.Any())
                {
                    return Enumerable.Empty<Collection>();
                }

                return modelList.Select(s => Collection.ViewModelConverter(s))!;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(GetAllCollectionsAsync), ex);
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

                Tcollection? record = await context.Tcollections.SingleOrDefaultAsync(s => s.Id == idCollection && s.IdLibrary == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"La collection n'existe pas avec l'id \"{idCollection}\".");
                }

                return Collection.ViewModelConverter(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(GetSingleCollectionAsync), ex);
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

                if (collectionName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException($"Le nom de la collection ne doit pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                Tcollection? record = await context.Tcollections.SingleOrDefaultAsync(s => s.Name.ToLower() == collectionName.Trim().ToLower() && s.IdLibrary == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"La collection n'existe pas avec le nom \"{collectionName}\".");
                }

                return Collection.ViewModelConverter(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(GetSingleCollectionAsync), ex);
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

                if (collectionNames == null || !collectionNames.Any())
                {
                    throw new ArgumentNullException($"Le tableau de nom ne doit pas être null, vide ou ne contenir que des espaces blancs.");
                }

                List<Tcollection> tcollections = new ();
                foreach (string collectionName in collectionNames)
                {
                    if (collectionName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        Logs.Log(nameof(Library), nameof(GetMultipleCollectionsAsync), $"Le nom de la collection ne doit pas être nulle, vide ou ne contenir que des espaces blancs.");
                        continue;
                    }

                    Tcollection? record = await context.Tcollections.SingleOrDefaultAsync(s => s.Name.ToLower() == collectionName.Trim().ToLower() && s.IdLibrary == Id);
                    if (record == null)
                    {
                        Logs.Log(nameof(Library), nameof(GetMultipleCollectionsAsync), $"La collection n'existe pas avec le nom \"{collectionName}\".");
                        continue;
                    }

                    tcollections.Add(record);
                }

                return tcollections.Select(s => Collection.ViewModelConverter(s))!;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(GetMultipleCollectionsAsync), ex);
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

                if (idCollections == null || !idCollections.Any())
                {
                    throw new ArgumentNullException($"Le tableau d'id ne doit pas être null et doit contenir au moins un élément.");
                }

                List<Tcollection> tcollections = new();
                foreach (long idCollection in idCollections)
                {
                    Tcollection? record = await context.Tcollections.SingleOrDefaultAsync(s => s.Id == idCollection && s.IdLibrary == Id);
                    if (record == null)
                    {
                        Logs.Log(nameof(Library), nameof(GetMultipleCollectionsAsync), $"La collection n'existe pas avec l'id \"{idCollection}\".");
                        continue;
                    }

                    tcollections.Add(record);
                }

                return tcollections.Select(s => Collection.ViewModelConverter(s))!;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(GetMultipleCollectionsAsync), ex);
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
        public async Task<Category?> AddCategoryAsync(string name, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                if (name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(name), "Le nom de la catégorie ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                TlibraryCategorie? existingItem = await context.TlibraryCategories.SingleOrDefaultAsync(c => c.Name.ToLower() == name.ToLower() && c.IdLibrary == Id);
                if (existingItem != null)
                {
                    Logs.Log(nameof(Library), nameof(AddCategoryAsync), "Cette catégorie existe déjà");
                    if (openIfExist)
                    {
                        return Category.ConvertToViewModel(existingItem);
                    }
                    else
                    {
                        throw new InvalidOperationException($"La catégorie {name} existe déjà.");
                    }
                }

                TlibraryCategorie record = new()
                {
                    Name = name.Trim(),
                    IdLibrary = Id,
                    Description = description?.Trim(),
                };

                await context.TlibraryCategories.AddAsync(record);
                await context.SaveChangesAsync();

                return Category.ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(AddCategoryAsync), ex);
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

                TlibraryCategorie? record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Id == idCategory && s.IdLibrary == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"La catégorie n'existe pas avec l'id \"{idCategory}\".");
                }

                return Category.ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(GetSingleCategoryAsync), ex);
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

                if (categoryName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException($"Le nom de la categorie ne doit pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                TlibraryCategorie? record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Name.ToLower() == categoryName.Trim().ToLower() && s.IdLibrary == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"La catégorie n'existe pas avec le nom \"{categoryName}\".");
                }

                return Category.ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(GetSingleCategoryAsync), ex);
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

                if (categoriesNames == null || !categoriesNames.Any())
                {
                    throw new ArgumentNullException($"Le tableau de nom ne doit pas être null, vide ou ne contenir que des espaces blancs.");
                }

                List<TlibraryCategorie> tlibraryCategories = new();
                foreach (string categoryName in categoriesNames)
                {
                    if (categoryName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        Logs.Log(nameof(Library), nameof(GetMultipleCategoriesAsync), $"Le nom de la catégorie ne doit pas être nulle, vide ou ne contenir que des espaces blancs.");
                        continue;
                    }

                    TlibraryCategorie? record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Name.ToLower() == categoryName.Trim().ToLower() && s.IdLibrary == Id);
                    if (record == null)
                    {
                        Logs.Log(nameof(Library), nameof(GetMultipleCategoriesAsync), $"La catégorie n'existe pas avec le nom \"{categoryName}\".");
                        continue;
                    }

                    tlibraryCategories.Add(record);
                }

                return tlibraryCategories.Select(s => Category.ConvertToViewModel(s))!;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Collection), nameof(GetMultipleCategoriesAsync), ex);
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

                if (idCategories == null || !idCategories.Any())
                {
                    throw new ArgumentNullException($"Le tableau d'id ne doit pas être null et doit contenir au moins un élément.");
                }

                List<TlibraryCategorie> tlibraryCategories = new();
                foreach (long idCategory in idCategories)
                {
                    TlibraryCategorie? record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Id == idCategory && s.IdLibrary == Id);
                    if (record == null)
                    {
                        Logs.Log(nameof(Library), nameof(GetMultipleCategoriesAsync), $"La catégorie n'existe pas avec l'id \"{idCategory}\".");
                        continue;
                    }

                    tlibraryCategories.Add(record);
                }

                return tlibraryCategories.Select(s => Category.ConvertToViewModel(s))!;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(GetMultipleCategoriesAsync), ex);
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
                Logs.Log(nameof(Library), nameof(GetCategoriesTreeAsync), ex);
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

                var modelList = await context.Tbooks.Where(w => w.IdLibrary == Id).ToListAsync();
                if (modelList == null || !modelList.Any())
                {
                    return Enumerable.Empty<Book>();
                }

                return modelList.Select(s => Book.ConvertToViewModel(s))!;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Library), exception:ex);
                return Enumerable.Empty<Book>();
            }
        }
        /// <summary>
        /// Ajoute une nouvelle catégorie à la bibliothèque.
        /// </summary>
        /// <param name="name">Nom de la collection</param>
        /// <param name="description">Description de la collection</param>
        /// <remarks>Si la collection existe, la collection existante sera retournée.</remarks>
        /// <returns></returns>
        public async Task<Book?> AddBookAsync(string title, string? lang = null, BookFormat? format = null, string? dateParution = null, string? notes = null, string? description = null, bool openIfExist = false)
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

                return await Book.CreateAsync(Id, title, lang, format, dateParution, notes, description, openIfExist);
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
                Logs.Log(nameof(Library), nameof(CountBooksAsync), ex);
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
                Logs.Log(nameof(Library), nameof(ConvertToViewModel), ex);
                return null;
            }
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}

