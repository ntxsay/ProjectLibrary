using AppHelpers;
using LibApi.Helpers;
using LibApi.Models.Local.SQLite;
using LibApi.Services.Libraries;
using LibApi.ViewModels.Books;
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
                Logs.Log(nameof(LibraryExtensions), nameof(OrderByName), ex);
                return Enumerable.Empty<T>();
            }
        }

        public static IEnumerable<T> OrderByName<T>(this IEnumerable<T> self, OrderBy orderBy = OrderBy.Ascending) where T : class
        {
            try
            {
                LibraryHelpers libraryHelpers = new();
                return libraryHelpers.Order(self, orderBy, SortBy.Name) ?? Enumerable.Empty<T>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryExtensions), nameof(OrderByName), ex);
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
                Logs.Log(nameof(LibraryExtensions), nameof(DisplayPage), ex);
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
                Logs.Log(nameof(LibraryExtensions), nameof(DisplayPage), ex);
                return 0;
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
                Logs.Log(nameof(LibraryExtensions), nameof(ConvertToViewModel), ex);
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
