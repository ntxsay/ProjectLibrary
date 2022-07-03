using AppHelpers;
using AppHelpers.Serialization;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using LibShared;
using LibShared.ViewModels.Books;
using LibShared.ViewModels.Contacts;
using LibShared.ViewModels.Libraries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApi.Helpers
{
    public class LibraryHelpers
    {
        internal async Task<IEnumerable<T>> OrderAsync<T>(SortBy sortBy = SortBy.Name, OrderBy orderBy = OrderBy.Ascending, long? idLibrary = null) where T : class
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                if (typeof(T).IsAssignableFrom(typeof(Tbook)) && idLibrary != null)
                {
                    var modelList = await context.Tbooks.Where(c => c.IdLibrary == idLibrary).ToListAsync();
                    if (modelList == null || !modelList.Any())
                    {
                        return Enumerable.Empty<T>();
                    }

                    return Order(modelList.Select(s => (T)(object)s), orderBy, sortBy);
                }
                else if (typeof(T).IsAssignableFrom(typeof(Tlibrary)))
                {
                    var modelList = await context.Tlibraries.ToListAsync();
                    if (modelList == null || !modelList.Any())
                    {
                        return Enumerable.Empty<T>();
                    }

                    return Order(modelList.Select(s => (T)(object)s), orderBy, sortBy);
                }

                throw new NotSupportedException($"Le type {typeof(T)} n'est pas supporté dans la méthode {nameof(OrderAsync)}.");
                //return Enumerable.Empty<T>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryHelpers), nameof(OrderAsync), ex);
                return Enumerable.Empty<T>();
            }
        }

        public IEnumerable<T> Order<T>(IEnumerable<T> modelList, OrderBy orderBy = OrderBy.Ascending, SortBy sortBy = SortBy.Name) where T : class
        {
            try
            {
                if (modelList == null || !modelList.Any())
                {
                    return Enumerable.Empty<T>();
                }

                if (modelList is IEnumerable<Tlibrary> librariesModel)
                {
                    if (sortBy == SortBy.Name)
                    {
                        if (orderBy == OrderBy.Ascending)
                        {
                            return librariesModel.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.Name).Select(s => (T)(object)s);
                        }
                        else if (orderBy == OrderBy.Descending)
                        {
                            return librariesModel.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.Name).Select(s => (T)(object)s);
                        }
                    }
                    else if (sortBy == SortBy.DateCreation)
                    {
                        if (orderBy == OrderBy.Ascending)
                        {
                            return librariesModel.OrderBy(o => o.DateAjout).Select(s => (T)(object)s);
                        }
                        else if (orderBy == OrderBy.Descending)
                        {
                            return librariesModel.OrderByDescending(o => o.DateAjout).Select(s => (T)(object)s);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException($"{sortBy} n'est pas supporté dans le type {typeof(T)}.");
                    }
                }
                else if (modelList is IEnumerable<LibraryVM> librariesViewModel)
                {
                    if (sortBy == SortBy.Name)
                    {
                        if (orderBy == OrderBy.Ascending)
                        {
                            return librariesViewModel.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.Name).Select(s => (T)(object)s);
                        }
                        else if (orderBy == OrderBy.Descending)
                        {
                            return librariesViewModel.Where(w => w != null && !w.Name.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.Name).Select(s => (T)(object)s);
                        }
                    }
                    else if (sortBy == SortBy.DateCreation)
                    {
                        if (orderBy == OrderBy.Ascending)
                        {
                            return librariesViewModel.OrderBy(o => o.DateAjout).Select(s => (T)(object)s);
                        }
                        else if (orderBy == OrderBy.Descending)
                        {
                            return librariesViewModel.OrderByDescending(o => o.DateAjout).Select(s => (T)(object)s);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException($"{sortBy} n'est pas supporté dans le type {typeof(T)}.");
                    }
                }
                else if (modelList is IEnumerable<Tbook> booksModel)
                {
                    if (sortBy == SortBy.Name)
                    {
                        if (orderBy == OrderBy.Ascending)
                        {
                            return booksModel.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderBy(o => o.MainTitle).Select(s => (T)(object)s);
                        }
                        else if (orderBy == OrderBy.Descending)
                        {
                            return booksModel.Where(w => w != null && !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace()).OrderByDescending(o => o.MainTitle).Select(s => (T)(object)s);
                        }
                    }
                    else if (sortBy == SortBy.DateCreation)
                    {
                        if (orderBy == OrderBy.Ascending)
                        {
                            return booksModel.OrderBy(o => o.DateAjout).Select(s => (T)(object)s);
                        }
                        else if (orderBy == OrderBy.Descending)
                        {
                            return booksModel.OrderByDescending(o => o.DateAjout).Select(s => (T)(object)s);
                        }
                    }
                }

                throw new NotSupportedException($"Le type {typeof(T)} n'est pas supporté dans la méthode {nameof(Order)}.");
                //return Enumerable.Empty<T>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryHelpers), nameof(Order), ex);
                return Enumerable.Empty<T>();
            }
        }

        public IEnumerable<T> GetPaginatedItems<T>(IEnumerable<T> modelList, int maxItemsPerPage, int goToPage = 1) where T : class
        {
            try
            {
                IEnumerable<T> itemsPage = Enumerable.Empty<T>();

                //Si la séquence contient plus d'items que le nombre max éléments par page
                if (modelList.Count() > maxItemsPerPage)
                {
                    //Si la première page (ou moins ^^')
                    if (goToPage <= 1)
                    {
                        itemsPage = modelList.Take(maxItemsPerPage);
                    }
                    else //Si plus que la première page
                    {
                        var nbItemsToSkip = maxItemsPerPage * (goToPage - 1);
                        if (modelList.Count() >= nbItemsToSkip)
                        {
                            var getRest = modelList.Skip(nbItemsToSkip);
                            //Si reste de la séquence contient plus d'items que le nombre max éléments par page
                            if (getRest.Count() > maxItemsPerPage)
                            {
                                itemsPage = getRest.Take(maxItemsPerPage);
                            }
                            else
                            {
                                itemsPage = getRest;
                            }
                        }
                    }
                }
                else //Si la séquence contient moins ou le même nombre d'items que le nombre max éléments par page
                {
                    itemsPage = modelList;
                }

                return itemsPage;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryHelpers), nameof(GetPaginatedItems), ex);
                return Enumerable.Empty<T>();
            }
        }

        public int CountPages<T>(IEnumerable<T> modelList, int maxItemsPerPage) where T : class
        {
            try
            {
                int countBook = modelList?.Count() ?? 0;
                if (countBook > 0)
                {
                    int nbPageDefault = countBook / maxItemsPerPage;
                    double nbPageExact = countBook / Convert.ToDouble(maxItemsPerPage);
                    int nbPageRounded = nbPageExact > nbPageDefault ? nbPageDefault + 1 : nbPageDefault;
                    return nbPageRounded;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryHelpers), exception: ex);
                return 0;
            }
        }

        public IEnumerable<IGrouping<string, T>> GroupItemsByNone<T>(IEnumerable<T> modelList) where T : class
        {
            try
            {
                if (modelList == null || !modelList.Any())
                {
                    throw new ArgumentNullException(nameof(modelList), "La liste ne peut pas être null ou ne contenir aucune données");
                }

                IEnumerable<IGrouping<string, T>>? groupingItems = null;

                if (typeof(T).IsAssignableFrom(typeof(LibraryVM)))
                {
                    groupingItems = modelList.Select(q => (LibraryVM)(object)q).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos bibliothèques").OrderBy(o => o.Key).Select(s => (IGrouping<string, T>)(object)s);
                }
                else if (typeof(T).IsAssignableFrom(typeof(BookVM)))
                {
                    groupingItems = modelList.Select(q => (BookVM)(object)q).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => "Vos livres").OrderBy(o => o.Key).Select(s => (IGrouping<string, T>)(object)s);
                }
                else
                {
                    throw new NotSupportedException($"Le type : \"{typeof(T)}\" n'est pas supporté.");
                }

                return groupingItems ?? Enumerable.Empty<IGrouping<string, T>>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryHelpers), exception: ex);
                return Enumerable.Empty<IGrouping<string, T>>();
            }
        }

        public IEnumerable<IGrouping<string, T>> GroupItemsByAlphabeticAsync<T>(IEnumerable<T> modelList) where T : class
        {
            try
            {
                if (modelList == null || !modelList.Any())
                {
                    throw new ArgumentNullException(nameof(modelList), "La liste ne peut pas être null ou ne contenir aucune données");
                }

                IEnumerable<IGrouping<string, T>>? groupingItems = null;

                if (typeof(T).IsAssignableFrom(typeof(LibraryVM)))
                {
                    groupingItems = modelList.Select(q => (LibraryVM)(object)q).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => g.Name?.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => (IGrouping<string, T>)(object)s);
                }
                else if (typeof(T).IsAssignableFrom(typeof(BookVM)))
                {
                    groupingItems = modelList.Select(q => (BookVM)(object)q).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => g.MainTitle?.FirstOrDefault().ToString().ToUpper()).OrderBy(o => o.Key).Select(s => (IGrouping<string, T>)(object)s);
                }
                else
                {
                    throw new NotSupportedException($"Le type : \"{typeof(T)}\" n'est pas supporté.");
                }

                return groupingItems ?? Enumerable.Empty<IGrouping<string, T>>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryHelpers), exception: ex);
                return Enumerable.Empty<IGrouping<string, T>>();
            }
        }

        public IEnumerable<IGrouping<string, T>> GroupByCreationYear<T>(IEnumerable<T> modelList) where T : class
        {
            try
            {
                if (modelList == null || !modelList.Any())
                {
                    throw new ArgumentNullException(nameof(modelList), "La liste ne peut pas être null ou ne contenir aucune données");
                }

                IEnumerable<IGrouping<string, T>>? groupingItems = null;

                if (typeof(T).IsAssignableFrom(typeof(LibraryVM)))
                {
                    groupingItems = modelList.Select(q => (LibraryVM)(object)q).Where(w => !w.Name.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => g.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => (IGrouping<string, T>)(object)s);
                }
                else if (typeof(T).IsAssignableFrom(typeof(BookVM)))
                {
                    groupingItems = modelList.Select(q => (BookVM)(object)q).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => g.DateAjout.Year.ToString() ?? "Année de création inconnue").OrderBy(o => o.Key).Select(s => (IGrouping<string, T>)(object)s);
                }
                else
                {
                    throw new NotSupportedException($"Le type : \"{typeof(T)}\" n'est pas supporté.");
                }

                return groupingItems ?? Enumerable.Empty<IGrouping<string, T>>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryHelpers), exception: ex);
                return Enumerable.Empty<IGrouping<string, T>>();
            }
        }

        public IEnumerable<IGrouping<string, T>> GroupByParutionYear<T>(IEnumerable<T> modelList) where T : class
        {
            try
            {
                if (modelList == null || !modelList.Any())
                {
                    throw new ArgumentNullException(nameof(modelList), "La liste ne peut pas être null ou ne contenir aucune données");
                }

                IEnumerable<IGrouping<string, T>>? groupingItems = null;

                if (typeof(T).IsAssignableFrom(typeof(BookVM)))
                {
                    groupingItems = modelList.Select(q => (BookVM)(object)q).Where(w => !w.MainTitle.IsStringNullOrEmptyOrWhiteSpace())?.GroupBy(g => g.YearParution?.ToString() ?? "Année de parution inconnue").OrderBy(o => o.Key).Select(s => (IGrouping<string, T>)(object)s);
                }
                else
                {
                    throw new NotSupportedException($"Le type : \"{typeof(T)}\" n'est pas supporté.");
                }

                return groupingItems ?? Enumerable.Empty<IGrouping<string, T>>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryHelpers), exception:ex);
                return Enumerable.Empty<IGrouping<string, T>>();
            }
        }

    }
}
