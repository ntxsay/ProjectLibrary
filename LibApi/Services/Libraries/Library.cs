using System;
using System.Diagnostics;
using AppHelpers;
using AppHelpers.Serialization;
using AppHelpers.Strings;
using LibApi.Helpers;
using LibApi.Models.Local.SQLite;
using LibShared.ViewModels.Libraries;
using Microsoft.EntityFrameworkCore;

namespace LibApi.Services.Libraries
{
	public class Library : LibraryVM
	{
        readonly LibraryHelpers libraryHelpers = new ();

        /// <summary>
        /// Obtient toutes bibliothèques trié par nom et par ordre croissant
        /// </summary>
        /// <returns></returns>
        public static async Task<Tlibrary[]> OrderByAscendingNameAsync() => (await new LibraryHelpers().OrderAsync<Tlibrary>(SortBy.Name, OrderBy.Croissant))?.ToArray() ?? Array.Empty<Tlibrary>();

        /// <summary>
        /// Obtient toutes bibliothèques trié par nom et par ordre décroissant
        /// </summary>
        /// <returns></returns>
        public static async Task<Tlibrary[]> OrderByDescendingNameAsync() => (await new LibraryHelpers().OrderAsync<Tlibrary>(SortBy.Name, OrderBy.DCroissant))?.ToArray() ?? Array.Empty<Tlibrary>();

        /// <summary>
        /// Obtient toutes bibliothèques trié par date d'ajout et par ordre croissant
        /// </summary>
        /// <returns></returns>
        public static async Task<Tlibrary[]> OrderByAscendingDateCreationAsync() => (await new LibraryHelpers().OrderAsync<Tlibrary>(SortBy.DateCreation, OrderBy.Croissant))?.ToArray() ?? Array.Empty<Tlibrary>();

        /// <summary>
        /// Obtient toutes bibliothèques trié par date d'ajout et par ordre décroissant
        /// </summary>
        /// <returns></returns>
        public static async Task<Tlibrary[]> OrderByDescendingDateCreationAsync() => (await new LibraryHelpers().OrderAsync<Tlibrary>(SortBy.DateCreation, OrderBy.DCroissant))?.ToArray() ?? Array.Empty<Tlibrary>();


        /// <summary>
        /// Obtient tous les livres de la bibliothèque trié par nom et par ordre croissant
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Tbook>> OrderBooksByAscendingNameAsync() => await libraryHelpers.OrderAsync<Tbook>(SortBy.Name, OrderBy.Croissant);

        /// <summary>
        /// Obtient tous les livres de la bibliothèque trié par nom et par ordre décroissant
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Tbook>> OrderBooksByDescendingNameAsync() => await libraryHelpers.OrderAsync<Tbook>(SortBy.Name, OrderBy.DCroissant);

        public Library()
		{
		}

        #region CRUD
        /// <summary>
        /// Ajoute la bibliothèque dans la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateAsync()
        {
            try
            {
                if (Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(Name), "Le nom de la bibliothèque ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();

                bool isExist = await context.Tlibraries.AnyAsync(c => c.Name.ToLower() == Name.ToLower());
                if (isExist)
                {
                    Logs.Log(nameof(Library), nameof(CreateAsync), "Cette bibliothèque existe déjà");
                    return true;
                }

                Tlibrary tlibrary = new()
                {
                    DateAjout = DateAjout.ToString(),
                    Guid = Guid.ToString(),
                    Name = Name,
                    Description = Description,
                };

                await context.Tlibraries.AddAsync(tlibrary);
                await context.SaveChangesAsync();

                Id = tlibrary.Id;

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(Library), nameof(CreateAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(Library), nameof(CreateAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(CreateAsync), ex);
                return false;
            }
        }

        /// <summary>
        /// Met à jour la bibliothèque dans la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateAsync()
        {
            try
            {
                if (Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(Name), "Le nom de la bibliothèque ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();

                Tlibrary? tlibrary = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == Id);
                if (tlibrary == null)
                {
                    throw new ArgumentNullException(nameof(Tlibrary), $"La bibliothèque n'existe pas avec l'id \"{Id}\".");
                }

                bool isExist = await context.Tlibraries.AnyAsync(c => c.Id != Id && c.Name.ToLower() == Name.ToLower());
                if (isExist)
                {
                    Logs.Log(nameof(Library), nameof(UpdateAsync), "Cette bibliothèque existe déjà");
                    return false;
                }

                DateTime dateEdition = DateTime.Now;

                tlibrary.Name = this.Name;
                tlibrary.Description = this.Description;
                tlibrary.DateEdition = dateEdition.ToString();

                context.Tlibraries.Update(tlibrary);
                _ = await context.SaveChangesAsync();

                DateEdition = dateEdition;

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
                using LibrarySqLiteDbContext context = new();

                Tlibrary? tlibrary = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == Id);
                if (tlibrary == null)
                {
                    throw new ArgumentNullException(nameof(Tlibrary), $"La bibliothèque n'existe pas avec l'id \"{Id}\".");
                }

                context.Tlibraries.Remove(tlibrary);
                _ = await context.SaveChangesAsync();

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

        #endregion


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

        /// <summary>
        /// Compte le nombre de livres dans cette bibliothèque
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> CountBooksAsync(CancellationToken cancellationToken = default)
        {
            try
            {
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
        /// <param name="model"></param>
        /// <returns></returns>
        public static Library? ConvertToViewModel(Tlibrary model)
        {
            try
            {
                if (model == null) return null;

                var isGuidCorrect = Guid.TryParse(model.Guid, out Guid guid);
                if (isGuidCorrect == false) return null;

                var viewModel = new Library()
                {
                    Id = model.Id,
                    //DateAjout = DatesHelpers.Converter.GetDateFromString(model.DateAjout),
                    //DateEdition = DatesHelpers.Converter.GetNullableDateFromString(model.DateEdition),
                    Description = model.Description ?? "",
                    Name = model.Name,
                    Guid = isGuidCorrect ? guid : Guid.Empty,
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Library), nameof(CountBooksAsync), ex);
                return null;
            }
        }


    }
}

