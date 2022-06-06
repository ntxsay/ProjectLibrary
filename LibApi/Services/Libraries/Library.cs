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

namespace LibApi.Services.Libraries
{
	public class Library : LibraryVM
	{
        /// <summary>
        /// Obtient une valeur booléenne indiquant si l'objet a déjà été effacé de la base de données
        /// </summary>
        public bool IsDeleted { get; private set; }

        readonly LibraryHelpers libraryHelpers = new();

        //public override Guid Guid { get; protected set; } = Guid.NewGuid();

        //public virtual DateTime DateAjout { get; protected set; } = DateTime.Now;

        //public virtual DateTime? DateEdition { get; protected set; }

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
        public static async Task<IEnumerable<Tlibrary>> GetAllAsync()
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                var modelList = await context.Tlibraries.ToListAsync();
                if (modelList == null || !modelList.Any())
                {
                    return Enumerable.Empty<Tlibrary>();
                }

                return modelList;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(GetAllAsync), ex);
                return Enumerable.Empty<Tlibrary>();
            }
        }

        /// <summary>
        /// Obtient une bibliothèque
        /// </summary>
        /// <param name="idLibrary"></param>
        /// <returns></returns>
        public static async Task<Tlibrary?> GetSingleAsync(long idLibrary)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                var item = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == idLibrary);
                return item;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(GetSingleAsync), ex);
                return null;
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
        /// Ajoute une nouvelle bibliothèque dans la base de données
        /// </summary>
        /// <returns></returns>
        public static async Task<Library?> CreateAsync(string name, string? description = null)
        {
            if (name.IsStringNullOrEmptyOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(name), "Le nom de la bibliothèque ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
            }

            using LibrarySqLiteDbContext context = new();

            Tlibrary? existingItem = await context.Tlibraries.SingleOrDefaultAsync(c => c.Name.ToLower() == name.Trim().ToLower());
            if (existingItem != null)
            {
                Logs.Log(nameof(Library), nameof(CreateAsync), "Cette bibliothèque existe déjà");
                return ConvertToViewModel(existingItem);
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

        /// <summary>
        /// Met à jour la bibliothèque dans la base de données
        /// </summary>
        /// <remarks>Remarque : si aucun paramètre n'est renseigné alors une <see cref="NotSupportedException"/> est levée et annule ainsi l'opération de mise à jour.</remarks>
        /// <param name="newName"></param>
        /// <param name="newDescription">Nouvelle description. Si le paramètre est null alors la modification de ce paramètre est ingoré.</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(string? newName, string? newDescription = null)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new NotSupportedException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (newName.IsStringNullOrEmptyOrWhiteSpace() && newDescription == null)
                {
                    throw new NotSupportedException("Le nouveau nom de la bibliothèque ou sa nouvelle description devait être renseignée.");
                }

                using LibrarySqLiteDbContext context = new();

                Tlibrary? tlibrary = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == Id);
                if (tlibrary == null)
                {
                    throw new ArgumentNullException(nameof(tlibrary), $"La bibliothèque n'existe pas avec l'id \"{Id}\".");
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
                    tlibrary.Description = newDescription.Trim();
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
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(Library), nameof(UpdateAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(Library), nameof(UpdateAsync), ex);
                return false;
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

                using LibrarySqLiteDbContext context = new();

                Tlibrary? tlibrary = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == Id);
                if (tlibrary == null)
                {
                    throw new ArgumentNullException(nameof(Tlibrary), $"La bibliothèque n'existe pas avec l'id \"{Id}\".");
                }

                context.Tlibraries.Remove(tlibrary);
                _ = await context.SaveChangesAsync();

                IsDeleted = true;
                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(Library), nameof(DeleteAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(Library), nameof(DeleteAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(DeleteAsync), ex);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Ajoute une nouvelle collection à la bibliothèque
        /// </summary>
        /// <param name="name">Nom de la bibliothèque</param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<Collection?> AddCollectionAsync(string name, string? description = null)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new NotSupportedException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                if (name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(name), "Le nom de la collection ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();
                Tcollection? existingItem = await context.Tcollections.SingleOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
                if (existingItem != null)
                {
                    Logs.Log(nameof(Library), nameof(AddCollectionAsync), "Cette collection existe déjà");
                    Collection _collection = new(existingItem.Id, Id, existingItem.Name, existingItem.Description);
                    return _collection;
                }

                Tcollection record = new()
                {
                    Name = name.Trim(),
                    IdLibrary = Id,
                    Description = description?.Trim(),
                };

                await context.Tcollections.AddAsync(record);
                await context.SaveChangesAsync();

                Collection collection = new(record.Id, Id, record.Name, record.Description);
                return collection;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(Library), nameof(AddCollectionAsync), ex);
                return null;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(Library), nameof(AddCollectionAsync), ex);
                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(AddCollectionAsync), ex);
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

                using LibrarySqLiteDbContext context = new();
                return await context.Tbooks.CountAsync(w => w.IdLibrary == Id, cancellationToken);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(CountBooksAsync), ex);
                return 0;
            }
        }

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


    }
}

