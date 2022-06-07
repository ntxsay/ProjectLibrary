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
    public class Collection : CollectionVM
    {
        /// <summary>
        /// Obtient une valeur booléenne indiquant si l'objet a déjà été effacé de la base de données
        /// </summary>
        public bool IsDeleted { get; private set; }
        private Collection()
        {

        }

        public new long Id
        {
            get => _Id;
            protected set
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
            protected set
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
            protected set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged();
                }
            }
        }

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

                using LibrarySqLiteDbContext context = new();

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
        /// Supprime la bibliothèque de la base de données
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

                using LibrarySqLiteDbContext context = new();

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

        public static Collection? ViewModelConverter(Tcollection model)
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
                Logs.Log(nameof(Collection), nameof(DeleteAsync), ex);
                return null;
            }
        }

    }
}
