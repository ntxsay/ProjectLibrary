using AppHelpers;
using AppHelpers.Strings;
using LibShared.ViewModels.Books;
using LibShared.ViewModels.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibShared.Services
{
    public static class LibraryHelpersExtensions
    {
        public static IEnumerable<IGrouping<string, T>> GroupItemsBy<T>(this IEnumerable<T> self, GroupBy groupBy = GroupBy.None) where T : class
        {
            try
            {
                LibraryHelpers libraryHelpers = new();
                switch (groupBy)
                {
                    case GroupBy.None:
                        return libraryHelpers.GroupItemsByNone(self);
                    case GroupBy.Letter:
                        return libraryHelpers.GroupItemsByAlphabeticAsync(self);
                    case GroupBy.CreationYear:
                        return libraryHelpers.GroupByCreationYear(self);
                    case GroupBy.ParutionYear:
                        break;
                    default:
                        break;
                }

                return Enumerable.Empty<IGrouping<string, T>>();
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryHelpersExtensions), exception: ex);
                return Enumerable.Empty<IGrouping<string, T>>();
            }
        }
    }

    public class LibraryHelpers
    {
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
                Logs.Log(nameof(LibraryHelpers), exception: ex);
                return Enumerable.Empty<IGrouping<string, T>>();
            }
        }
    }
}
