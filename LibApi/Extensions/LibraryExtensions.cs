using AppHelpers;
using LibApi.Helpers;
using LibApi.Models.Local.SQLite;
using LibApi.Services.Libraries;
using LibApi.Services.Books;
using LibShared;
using LibShared.ViewModels.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApi.Extensions
{
    public static class LibraryExtensions
    {
        /// <summary>
        /// Retourne la liste d'objet ordonnée et triée
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="orderBy"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrderItemsBy<T>(this IEnumerable<T> self, OrderBy orderBy = OrderBy.Ascending, SortBy sortBy = SortBy.Name) where T : class
        {
            try
            {
                LibraryHelpers libraryHelpers = new();
                return libraryHelpers.Order(self, orderBy, sortBy) ?? Enumerable.Empty<T>();
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryExtensions), exception:ex);
                return Enumerable.Empty<T>();
            }
        }

        public static IEnumerable<T> DisplayPage<T>( this IEnumerable<T> self, int maxItemsPerPage = 20, int goToPage = 1) where T : class
        {
            try
            {
                LibraryHelpers libraryHelpers = new();
                return libraryHelpers.GetPaginatedItems(self, maxItemsPerPage, goToPage);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryExtensions), exception: ex);
                return Enumerable.Empty<T>();
            }
        }

        /// <summary>
        /// Retourne le nombre de page que pourrait comp
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="maxItemsPerPage"></param>
        /// <returns></returns>
        public static int CountPages<T>(this IEnumerable<T> self, int maxItemsPerPage = 20) where T : class
        {
            try
            {
                LibraryHelpers libraryHelpers = new();
                return libraryHelpers.CountPages(self, maxItemsPerPage);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryExtensions), exception: ex);
                return 0;
            }
        }

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
                Logs.Log(className: nameof(LibraryExtensions), exception: ex);
                return Enumerable.Empty<IGrouping<string, T>>();
            }
        }

        /// <summary>
        /// Convertit un tableau de modèle en tableau de modèle de vue.
        /// </summary>
        /// <param name="self">Tableau de modèle</param>
        /// <returns></returns>
        public static IEnumerable<LibraryVM> ConvertToViewModel(this IEnumerable<Tlibrary> self)
        {
            try
            {
                return self?.Select(s => Library.ConvertToViewModel(s))?.Where(w => w != null) ?? Enumerable.Empty<Library>();
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryExtensions), exception: ex);
                return Enumerable.Empty<LibraryVM>();
            }
        }

        /// <summary>
        /// Convertit un tableau de modèle en tableau de modèle de vue.
        /// </summary>
        /// <param name="self">Tableau de modèle</param>
        /// <returns></returns>
        public static IEnumerable<Library> ConvertToObject(this IEnumerable<Tlibrary> self)
        {
            try
            {
                return self?.Select(s => Library.ConvertToViewModel(s))?.Where(w => w != null) ?? Enumerable.Empty<Library>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryExtensions), nameof(ConvertToObject), ex);
                return Enumerable.Empty<Library>();
            }
        }

        // <summary>
        /// Convertit un modèle en modèle de vue.
        /// </summary>
        /// <param name="self">Tableau de modèle</param>
        /// <returns></returns>
        public static LibraryVM? ConvertToViewModel(this Tlibrary self) => Library.ConvertToViewModel(self);
        public static Library? ConvertToObject(this Tlibrary self) => Library.ConvertToViewModel(self);

        /// <summary>
        /// Convertit un tableau de modèle en tableau de modèle de vue.
        /// </summary>
        /// <param name="self">Tableau de modèle</param>
        /// <returns></returns>
        public static IEnumerable<Book> ConvertToViewModel(this IEnumerable<Tbook> self)
        {
            try
            {
                return Enumerable.Empty<Book>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryExtensions), nameof(ConvertToViewModel), ex);
                return Enumerable.Empty<Book>();
            }
        }
    }
}
