using System;
using System.Diagnostics;
using AppHelpers;
using AppHelpers.Serialization;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using Microsoft.EntityFrameworkCore;

namespace LibApi.ViewModels.Libraries
{
	public class LibraryVM : GenericItemVM
	{
        private int _MaxItemsPerPage = 100;
        public int MaxItemsPerPage
        {
            get => this._MaxItemsPerPage;
            set
            {
                if (_MaxItemsPerPage != value)
                {
                    this._MaxItemsPerPage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public LibraryVM()
		{
		}

        //public override string? GetJsonDataStringAsync()
        //{
        //	return JsonSerialization.SerializeClassToString(this);
        //}

        #region CRUD
        /// <summary>
        /// Ajoute la bibliothèque dans la base de données puis crée 
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> CreateAsync()
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
                    Logs.Log(nameof(LibraryVM), nameof(CreateAsync), "Cette bibliothèque existe déjà");
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
                Logs.Log(nameof(LibraryVM), nameof(CreateAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(LibraryVM), nameof(CreateAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryVM), nameof(CreateAsync), ex);
                return false;
            }
        }

        public override async Task<bool> UpdateAsync()
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
                    Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), "Cette bibliothèque existe déjà");
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
                Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
                return false;
            }
        }

        public override async Task<bool> DeleteAsync()
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
                Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
                return false;
            }
        }

        #endregion

        public async Task<IEnumerable<Tlibrary>> OrderByNameAsync() => await OrderAsync<Tlibrary>(SortBy.Name, OrderBy.Croissant);


        public async Task<int> CountBooksAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                return await context.Tbooks.CountAsync(w => w.IdLibrary == Id, cancellationToken);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryVM), nameof(CountBooksAsync), ex);
                return 0;
            }
        }

        public IEnumerable<LibraryVM> GetPaginatedItemsVm(IEnumerable<Tlibrary> modelList, int goToPage = 1)
        {
            try
            {
                var selectedItems = GetPaginatedItems(modelList, MaxItemsPerPage, goToPage);
                List<LibraryVM>? viewModelList = selectedItems?.Select(s => Convert(s))?.Where(w => w != null)?.ToList();
                return viewModelList ?? Enumerable.Empty<LibraryVM>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryVM), nameof(GetPaginatedItemsVm), ex);
                return Enumerable.Empty<LibraryVM>();
            }
        }

        public static LibraryVM? Convert(Tlibrary model)
        {
            try
            {
                if (model == null) return null;

                var isGuidCorrect = Guid.TryParse(model.Guid, out Guid guid);
                if (isGuidCorrect == false) return null;

                var viewModel = new LibraryVM()
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
                Logs.Log(nameof(LibraryVM), nameof(CountBooksAsync), ex);
                return null;
            }
        }


    }
}

