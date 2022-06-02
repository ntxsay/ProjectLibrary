using AppHelpers;
using AppHelpers.Serialization;
using AppHelpers.Strings;
using LibApi.Helpers;
using LibApi.Models.Local.SQLite;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibApi.ViewModels
{
    public class GenericItemVM : LibraryHelpers, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        
        public long Id { get; protected set; }
        public Guid Guid { get; protected set; } = Guid.NewGuid();

        public DateTime DateAjout { get; protected set; } = DateTime.Now;

        public DateTime? DateEdition { get; protected set; }

        protected string _Name = string.Empty;
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string _Description = string.Empty;
        public string Description
        {
            get => _Description;
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual async Task<bool> CreateAsync()
        {
            return await Task.FromResult<bool>(true);
        }

        public virtual async Task<bool> UpdateAsync()
        {
            return await Task.FromResult<bool>(true);
        }

        public virtual async Task<bool> DeleteAsync()
        {
            return await Task.FromResult<bool>(true);
        }

        public virtual string? GetJsonDataStringAsync()
        {
            return JsonSerialization.SerializeClassToString(this);
        }

        //protected async Task<IEnumerable<T>> OrderAsync<T>(SortBy sortBy = SortBy.Name, OrderBy orderBy = OrderBy.Croissant, long? idLibrary = null) where T : class
        //{
        //    try
        //    {
        //        using LibrarySqLiteDbContext context = new();
        //        if (typeof(T).IsAssignableFrom(typeof(Tbook)) && idLibrary != null)
        //        {
        //            var modelList = await context.Tbooks.Where(c => c.IdLibrary == idLibrary).ToListAsync();
        //            if (modelList == null || !modelList.Any())
        //            {
        //                return Enumerable.Empty<T>();
        //            }

        //            return Order(modelList.Select(s => (T)(object)s), orderBy, sortBy);
        //        }
        //        else if (typeof(T).IsAssignableFrom(typeof(Tlibrary)))
        //        {
        //            var modelList = await context.Tlibraries.ToListAsync();
        //            if (modelList == null || !modelList.Any())
        //            {
        //                return Enumerable.Empty<T>();
        //            }

        //            return Order(modelList.Select(s => (T)(object)s), orderBy, sortBy);
        //        }

        //        return Enumerable.Empty<T>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.Log(nameof(GenericItemVM), nameof(OrderAsync), ex);
        //        return Enumerable.Empty<T>();
        //    }
        //}


        //protected IEnumerable<T> Order<T>(IEnumerable<T> modelList, OrderBy orderBy = OrderBy.Croissant, SortBy sortBy = SortBy.Name) where T : class
        //{
        //    try
        //    {
        //        if (modelList == null || !modelList.Any())
        //        {
        //            return Enumerable.Empty<T>();
        //        }

        //        if (modelList is IEnumerable<Tlibrary> librariesModel)
        //        {
        //            if (sortBy == SortBy.Name)
        //            {
        //                if (orderBy == OrderBy.Croissant)
        //                {
        //                    return librariesModel.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.Name).Select(s => (T)(object)s);
        //                }
        //                else if (orderBy == OrderBy.DCroissant)
        //                {
        //                    return librariesModel.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.Name).Select(s => (T)(object)s);
        //                }
        //            }
        //            else if (sortBy == SortBy.DateCreation)
        //            {
        //                if (orderBy == OrderBy.Croissant)
        //                {
        //                    return librariesModel.OrderBy(o => o.DateAjout).Select(s => (T)(object)s);
        //                }
        //                else if (orderBy == OrderBy.DCroissant)
        //                {
        //                    return librariesModel.OrderByDescending(o => o.DateAjout).Select(s => (T)(object)s);
        //                }
        //            }
        //        }
        //        else if (modelList is IEnumerable<Tbook> booksModel)
        //        {
        //            if (sortBy == SortBy.Name)
        //            {
        //                if (orderBy == OrderBy.Croissant)
        //                {
        //                    return booksModel.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.MainTitle).Select(s => (T)(object)s);
        //                }
        //                else if (orderBy == OrderBy.DCroissant)
        //                {
        //                    return booksModel.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.MainTitle).Select(s => (T)(object)s);
        //                }
        //            }
        //            else if (sortBy == SortBy.DateCreation)
        //            {
        //                if (orderBy == OrderBy.Croissant)
        //                {
        //                    return booksModel.OrderBy(o => o.DateAjout).Select(s => (T)(object)s);
        //                }
        //                else if (orderBy == OrderBy.DCroissant)
        //                {
        //                    return booksModel.OrderByDescending(o => o.DateAjout).Select(s => (T)(object)s);
        //                }
        //            }
        //        }


        //        return Enumerable.Empty<T>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.Log(nameof(GenericItemVM), nameof(Order), ex);
        //        return Enumerable.Empty<T>();
        //    }
        //}

        //public IEnumerable<T> GetPaginatedItems<T>(IEnumerable<T> modelList, int maxItemsPerPage, int goToPage = 1) where T : class
        //{
        //    try
        //    {
        //        IEnumerable<T> itemsPage = Enumerable.Empty<T>();

        //        //Si la séquence contient plus d'items que le nombre max éléments par page
        //        if (modelList.Count() > maxItemsPerPage)
        //        {
        //            //Si la première page (ou moins ^^')
        //            if (goToPage <= 1)
        //            {
        //                itemsPage = modelList.Take(maxItemsPerPage);
        //            }
        //            else //Si plus que la première page
        //            {
        //                var nbItemsToSkip = maxItemsPerPage * (goToPage - 1);
        //                if (modelList.Count() >= nbItemsToSkip)
        //                {
        //                    var getRest = modelList.Skip(nbItemsToSkip);
        //                    //Si reste de la séquence contient plus d'items que le nombre max éléments par page
        //                    if (getRest.Count() > maxItemsPerPage)
        //                    {
        //                        itemsPage = getRest.Take(maxItemsPerPage);
        //                    }
        //                    else
        //                    {
        //                        itemsPage = getRest;
        //                    }
        //                }
        //            }
        //        }
        //        else //Si la séquence contient moins ou le même nombre d'items que le nombre max éléments par page
        //        {
        //            itemsPage = modelList;
        //        }

        //        return itemsPage;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.Log(nameof(GenericItemVM), nameof(GetPaginatedItems), ex);
        //        return Enumerable.Empty<T>();
        //    }
        //}



        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

