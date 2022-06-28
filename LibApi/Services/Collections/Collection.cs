using AppHelpers;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using LibShared.ViewModels.Collections;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApi.Services.Collections
{
    public sealed class Collection : CollectionVM, IDisposable
    {
        /// <summary>
        /// Obtient une valeur booléenne indiquant si l'objet a déjà été effacé de la base de données
        /// </summary>
        public bool IsDeleted { get; private set; }
        readonly LibrarySqLiteDbContext context = new();

        private Collection()
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
        #endregion

        #region CRUD
        /// <summary>
        /// Met à jour la collection dans la base de données
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
                    throw new InvalidOperationException($"La collection {Name} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (newName.IsStringNullOrEmptyOrWhiteSpace() && newDescription == null)
                {
                    throw new InvalidOperationException("Le nouveau nom de la collection ou sa nouvelle description devait être renseignée.");
                }

                Tcollection? record = await context.Tcollections.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new ArgumentException(nameof(record), $"La collection n'existe pas avec l'id \"{Id}\".");
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    bool isExist = await context.Tcollections.AnyAsync(c => c.Id != Id && c.Name.ToLower() == newName.Trim().ToLower())!;
                    if (isExist)
                    {
                        throw new ArgumentException($"Cette collection existe déjà.");
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

                context.Tcollections.Update(record);
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
                Logs.Log(nameof(Collection), nameof(UpdateAsync), ex);
                return false;
            }
        }

        /// <summary>
        /// Supprime la collection de la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            try
            {
                if (IsDeleted)
                {
                    throw new NotSupportedException($"La collection {Name} a déjà été supprimée.");
                }

                Tcollection? record = await context.Tcollections.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new ArgumentNullException(nameof(Tcollection), $"La collection n'existe pas avec l'id \"{Id}\".");
                }

                context.Tcollections.Remove(record);
                _ = await context.SaveChangesAsync();
                
                IsDeleted = true;

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(Collection), nameof(DeleteAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(Collection), nameof(DeleteAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Collection), nameof(DeleteAsync), ex);
                return false;
            }
        }

        #endregion

        public static async Task<IEnumerable<Collection>> AllAsync(long? idLibrary = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                List<Tcollection>? modelList = new();

                if (idLibrary == null)
                {
                    modelList = await context.Tcollections.ToListAsync();
                }
                else
                {
                    modelList = await context.Tcollections.Where(w => w.IdLibrary == (long)idLibrary).ToListAsync();
                }

                if (modelList == null || !modelList.Any())
                {
                    return Enumerable.Empty<Collection>();
                }

                return modelList.Select(s => ConvertToViewModel(s)).Where(w => w != null)!;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Collection), exception: ex);
                return Enumerable.Empty<Collection>();
            }
        }

        public static async Task<Collection?> SingleAsync(long id, long? idLibrary = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                Tcollection? record;
                if (idLibrary == null)
                {
                    record = await context.Tcollections.SingleOrDefaultAsync(s => s.Id == id);
                }
                else
                {
                    record = await context.Tcollections.SingleOrDefaultAsync(s => s.Id == id && s.IdLibrary == (long)idLibrary);
                }

                if (record == null)
                {
                    throw new NullReferenceException($"La collection n'existe pas avec l'id \"{id}\".");
                }

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Collection), exception: ex);
                return null;
            }
        }

        public static async Task<Collection?> SingleAsync(string name, long? idLibrary = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                if (name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(name), $"Le nom de la collection ne doit pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                Tcollection? record;
                if (idLibrary == null)
                {
                    record = await context.Tcollections.FirstOrDefaultAsync(s => s.Name.ToLower() == name.Trim().ToLower());
                }
                else
                {
                    record = await context.Tcollections.FirstOrDefaultAsync(s => s.Name.ToLower() == name.Trim().ToLower() && s.IdLibrary == (long)idLibrary);
                }

                if (record == null)
                {
                    throw new NullReferenceException($"La collection n'existe pas avec le nom \"{name}\".");
                }

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Collection), exception: ex);
                return null;
            }
        }


        public static async Task<Collection?> CreateAsync(long idLibrary, CollectionVM viewModel, bool openIfExist = false)
        {
            try
            {
                if (viewModel == null)
                {
                    throw new ArgumentNullException(nameof(viewModel), "Le modèle de vue ne peut pas être null.");
                }

                if (viewModel.Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(viewModel.Name), "Le nom de la collection ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();

                Tcollection? existingItem = await context.Tcollections.SingleOrDefaultAsync(c => c.Name.ToLower() == viewModel.Name.Trim().ToLower() && c.IdLibrary == idLibrary);
                if (existingItem != null)
                {
                    Logs.Log(nameof(Collection), message:"Cette collection existe déjà");
                    if (openIfExist)
                    {
                        return ConvertToViewModel(existingItem);
                    }
                    else
                    {
                        throw new InvalidOperationException($"La collection {viewModel.Name} existe déjà.");
                    }
                }

                Tcollection record = new()
                {
                    Name = viewModel.Name.Trim(),
                    IdLibrary = idLibrary,
                    Description = viewModel.Description?.Trim(),
                };

                await context.Tcollections.AddAsync(record);
                await context.SaveChangesAsync();

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Collection), exception: ex);
                return null;
            }
        }

        public static async Task<Collection?> CreateAsync(long idLibrary, string name, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(name), "Le nom de la collection ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                return await CreateAsync(idLibrary, new CollectionVM()
                {
                    Name = name.Trim(),
                    Description = description?.Trim()
                });
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Collection), exception: ex);
                return null;
            }
        }

        public static Collection? ConvertToViewModel(Tcollection model)
        {
            try
            {
                if (model == null) return null;

                Collection viewModel = new()
                {
                    Id = model.Id,
                    IdLibrary = model.IdLibrary,
                    Description = model.Description,
                    Name = model.Name,
                    //BooksId = (await GetBooksIdInCollectionAsync(model.Id)).ToList()
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Collection), nameof(ConvertToViewModel), ex);
                return null;
            }
        }

        public static async Task<long?> GetIdIfExistAsync(long idLibrary, CollectionVM viewModel, bool isEdit = false, long? modelId = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                string? name = viewModel.Name?.Trim()?.ToLower();

                if (!isEdit)
                {
                    return (await context.Tcollections.FirstOrDefaultAsync(c => c.IdLibrary == idLibrary && c.Name.ToLower() == name))?.Id ?? null;
                }
                else if (isEdit && modelId != null)
                {
                    return (await context.Tcollections.FirstOrDefaultAsync(c => c.IdLibrary == idLibrary && c.Id != (long)modelId && c.Name.ToLower() == name))?.Id ?? null;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Collection), exception: ex);
                return null;
            }
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
