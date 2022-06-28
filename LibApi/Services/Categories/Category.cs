using AppHelpers;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using LibShared.ViewModels.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApi.Services.Categories
{
    public sealed class Category : CategoryVM, IDisposable
    {
        /// <summary>
        /// Obtient une valeur booléenne indiquant si l'objet a déjà été effacé de la base de données
        /// </summary>
        public bool IsDeleted { get; private set; }
        LibrarySqLiteDbContext context = new ();

        private Category()
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

        private IEnumerable<Category> _Childs = Enumerable.Empty<Category>();
        public IEnumerable<Category> Childs
        {
            get => _Childs;
            private set
            {
                if (_Childs != value)
                {
                    _Childs = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region CRUD
        /// <summary>
        /// Ajoute une nouvelle sous-catégorie à la catégorie actuelle.
        /// </summary>
        /// <param name="name">Nom de la collection</param>
        /// <param name="description">Description de la collection</param>
        /// <remarks>Si la collection existe, la collection existante sera retournée.</remarks>
        /// <returns></returns>
        public async Task<Category?> AddSubCategoryAsync(string name, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La catégorie {Name} a déjà été supprimée.");
                }

                if (name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(name), "Le nom de la sous-catégorie ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                TlibraryCategorie? existingItem = await context.TlibraryCategories.SingleOrDefaultAsync(c => c.Name.ToLower() == name.ToLower() && c.IdParentCategorie == Id && c.IdLibrary == IdLibrary);
                if (existingItem != null)
                {
                    Logs.Log(nameof(Category), nameof(AddSubCategoryAsync), "Une catégorie/sous-catégorie portant le même nom existe déjà");
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
                    IdLibrary = IdLibrary,
                    IdParentCategorie = Id,
                    Description = description?.Trim(),
                };

                await context.TlibraryCategories.AddAsync(record);
                await context.SaveChangesAsync();

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), nameof(AddSubCategoryAsync), ex);
                return null;
            }
        }

        /// <summary>
        /// Met à jour la catégorie dans la base de données
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
                    throw new InvalidOperationException($"La catégorie {Name} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (newName.IsStringNullOrEmptyOrWhiteSpace() && newDescription == null)
                {
                    throw new InvalidOperationException("Le nouveau nom de la catégorie ou sa nouvelle description doivent être renseignées.");
                }

                TlibraryCategorie? record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new ArgumentException(nameof(record), $"La catégorie n'existe pas avec l'id \"{Id}\".");
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    bool isExist = await context.TlibraryCategories.AnyAsync(c => c.Id != Id && c.Name.ToLower() == newName.Trim().ToLower())!;
                    if (isExist)
                    {
                        throw new ArgumentException($"Cette catégorie existe déjà.");
                    }
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    record.Name = newName.Trim();
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (newDescription != null)
                {
                    record.Description = newDescription.Trim();
                }

                context.TlibraryCategories.Update(record);
                _ = await context.SaveChangesAsync();

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Name = record.Name;
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (newDescription != null)
                {
                    Description = record.Description;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), nameof(UpdateAsync), ex);
                return false;
            }
        }

        /// <summary>
        /// Supprime la catégorie de la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La catégorie {Name} a déjà été supprimée.");
                }

                TlibraryCategorie? record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new ArgumentNullException(nameof(Tcollection), $"La catégorie n'existe pas avec l'id \"{Id}\".");
                }

                List< TlibraryCategorie> recordChilds = await context.TlibraryCategories.Where(s => s.IdParentCategorie == Id).ToListAsync();
                if (recordChilds.Any())
                {
                    recordChilds.ForEach(f => f.IdParentCategorie = null);
                    context.TlibraryCategories.UpdateRange(recordChilds);
                }

                context.TlibraryCategories.Remove(record);
                _ = await context.SaveChangesAsync();

                IsDeleted = true;

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), nameof(DeleteAsync), ex);
                return false;
            }
        }

        #endregion


        /// <summary>
        /// Obtient toute l'arborescence des catégories enfants de cette catégorie.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Category>> GetCategoriesTreeAsync()
        {
            try
            {
                List<TlibraryCategorie> Parentcategories = await context.TlibraryCategories.Where(w => w.IdParentCategorie == Id && w.IdLibrary == IdLibrary).ToListAsync();
                if (Parentcategories != null && Parentcategories.Any())
                {
                    List<Category?> categories = Parentcategories.Select(s => ConvertToViewModel(s)).Where(w => w != null).ToList();
                    foreach (var category in categories)
                    {
                        if (category != null)
                        {
                            await Category.GetChildCategoriesAsync(category, IdLibrary);
                        }
                    }

                    return categories;
                }

                return Enumerable.Empty<Category>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), nameof(GetCategoriesTreeAsync), ex);
                return Enumerable.Empty<Category>();
            }
        }

        internal static async Task GetChildCategoriesAsync(Category parentCategory, long idLibrary)
        {
            try
            {
                if (parentCategory == null)
                {
                    return;
                }

                using LibrarySqLiteDbContext context = new();

                List<TlibraryCategorie> Parentcategories = await context.TlibraryCategories.Where(w => w.IdParentCategorie == parentCategory.Id && w.IdLibrary == idLibrary).ToListAsync();
                if (Parentcategories != null && Parentcategories.Any())
                {
                    List<Category> categories = new();
                    foreach (var category in Parentcategories)
                    {
                        var _category = ConvertToViewModel(category);
                        if (_category != null)
                        {
                            categories.Add(_category);
                        }
                    }

                    if (categories.Count > 0)
                    {
                        parentCategory.Childs = categories;
                        if (parentCategory.Childs != null && parentCategory.Childs.Any())
                        {
                            foreach (var child in parentCategory.Childs)
                            {
                                await GetChildCategoriesAsync(child, idLibrary);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), nameof(GetChildCategoriesAsync), ex);
                return;
            }
        }

        /// <summary>
        /// Insère cette catégorie dans une autre
        /// </summary>
        /// <param name="parentCategory">Catégorie parente</param>
        /// <returns>Retourne une valeur booléenne</returns>
        public async Task<bool> InsertInAnotherCategoryAsync(Category parentCategory)
        {
            try
            {
                if (parentCategory.IdLibrary != IdLibrary)
                {
                    throw new NotSupportedException("Impossible d'insérer une catégorie dans une autre provenant d'une autre bibliothèque.");
                }

                if (parentCategory.IdParentCategory == Id)
                {
                    throw new NotSupportedException("Impossible d'insérer une catégorie parente dans une catégorie enfant.");
                }

                bool isAlreadyExist = await context.TlibraryCategories.AnyAsync(c => c.Name.ToLower() == Name.ToLower() && c.IdParentCategorie == parentCategory.Id && c.IdLibrary == IdLibrary);
                if (isAlreadyExist == true)
                {
                    throw new NotSupportedException("Une catégorie du même nom existe déjà à cet emplacement");
                }

                TlibraryCategorie? record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new ArgumentNullException(nameof(record), $"La catégorie n'existe pas avec l'id \"{Id}\".");
                }

                record.IdParentCategorie = parentCategory.Id;
                context.TlibraryCategories.Update(record);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), nameof(InsertInAnotherCategoryAsync), ex);
                return false;
            }
        }

        #region Static methods
        public static async Task<Category?> CreateAsync(long idLibrary, CategoryVM viewModel, bool openIfExist = false)
        {
            try
            {
                if (viewModel == null)
                {
                    throw new ArgumentNullException(nameof(viewModel), "Le modèle de vue ne peut pas être null.");
                }

                if (viewModel.Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(viewModel.Name), "Le nom de la catégorie ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();

                TlibraryCategorie? existingItem = await context.TlibraryCategories.SingleOrDefaultAsync(c => c.Name.ToLower() == viewModel.Name.Trim().ToLower() && c.IdParentCategorie == viewModel.IdParentCategory && c.IdLibrary == idLibrary);
                if (existingItem != null)
                {
                    Logs.Log(nameof(Category), message: "Cette catégorie existe déjà");
                    if (openIfExist)
                    {
                        return ConvertToViewModel(existingItem);
                    }
                    else
                    {
                        throw new InvalidOperationException($"La catégorie {viewModel.Name} existe déjà.");
                    }
                }

                if (viewModel.IdParentCategory != null)
                {
                    bool isParentCategorieExist = await context.TlibraryCategories.AnyAsync(c => c.Id == viewModel.IdParentCategory && c.IdLibrary == idLibrary);
                    if (isParentCategorieExist == false)
                    {
                        throw new InvalidOperationException($"La catégorie parente n'existe pas.");
                    }
                }

                TlibraryCategorie record = new()
                {
                    Name = viewModel.Name.Trim(),
                    IdLibrary = idLibrary,
                    IdParentCategorie = viewModel.IdParentCategory,
                    Description = viewModel.Description?.Trim(),
                };

                await context.TlibraryCategories.AddAsync(record);
                await context.SaveChangesAsync();

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), exception: ex);
                return null;
            }
        }

        public static async Task<Category?> CreateAsync(long idLibrary, string name, string? description = null, long? idParentCategorie = null, bool openIfExist = false)
        {
            try
            {
                if (name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(name), "Le nom de la catégorie ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                return await CreateAsync(idLibrary, new CategoryVM()
                {
                    IdParentCategory = idParentCategorie,
                    Name = name.Trim(),
                    Description = description?.Trim()
                });
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), exception: ex);
                return null;
            }
        }


        public static async Task<IEnumerable<Category>> AllAsync(long? idLibrary = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                List<TlibraryCategorie>? modelList = new();

                if (idLibrary == null)
                {
                    modelList = await context.TlibraryCategories.ToListAsync();
                }
                else
                {
                    modelList = await context.TlibraryCategories.Where(w => w.IdLibrary == (long)idLibrary).ToListAsync();
                }

                if (modelList == null || !modelList.Any())
                {
                    return Enumerable.Empty<Category>();
                }

                return modelList.Select(s => ConvertToViewModel(s)).Where(w => w != null)!;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Category), exception: ex);
                return Enumerable.Empty<Category>();
            }
        }

        public static async Task<Category?> SingleAsync(long id, long? idLibrary = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                TlibraryCategorie? record;
                if (idLibrary == null)
                {
                    record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Id == id);
                }
                else
                {
                    record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Id == id && s.IdLibrary == (long)idLibrary);
                }

                if (record == null)
                {
                    throw new NullReferenceException($"La catégorie n'existe pas avec l'id \"{id}\".");
                }

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Category), exception: ex);
                return null;
            }
        }

        public static async Task<Category?> SingleAsync(string name, long? idLibrary = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                if (name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(name),$"Le nom de la categorie ne doit pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                TlibraryCategorie? record;
                if (idLibrary == null)
                {
                    record = await context.TlibraryCategories.FirstOrDefaultAsync(s => s.Name.ToLower() == name.Trim().ToLower());
                }
                else
                {
                    record = await context.TlibraryCategories.FirstOrDefaultAsync(s => s.Name.ToLower() == name.Trim().ToLower() && s.IdLibrary == (long)idLibrary);
                }

                if (record == null)
                {
                    throw new NullReferenceException($"La catégorie n'existe pas avec le nom \"{name}\".");
                }

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Category), exception: ex);
                return null;
            }
        }
        #endregion

        public static Category? ConvertToViewModel(TlibraryCategorie model)
        {
            try
            {
                if (model == null) return null;

                Category viewModel = new()
                {
                    Id = model.Id,
                    IdLibrary = model.IdLibrary,
                    IdParentCategory = model.IdParentCategorie,
                    Description = model.Description,
                    Name = model.Name,
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), nameof(ConvertToViewModel), ex);
                return null;
            }
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
