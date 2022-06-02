using AppHelpers;
using LibApi.Helpers;
using LibApi.Models.Local.SQLite;
using LibApi.ViewModels.Books;
using LibApi.ViewModels.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApi.Extensions
{
    public static class LibraryExtensions
    {
        public static IEnumerable<T> DisplayPage<T>( this IEnumerable<T> modelList, int maxItemsPerPage = 20, int goToPage = 1) where T : class
        {
            try
            {
                LibraryHelpers libraryHelpers = new();
                return libraryHelpers.GetPaginatedItems(modelList, maxItemsPerPage, goToPage);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryExtensions), nameof(DisplayPage), ex);
                return Enumerable.Empty<T>();
            }
        }

        public static int CountPages<T>(this IEnumerable<T> modelList, int maxItemsPerPage = 20) where T : class
        {
            try
            {
                LibraryHelpers libraryHelpers = new();
                return libraryHelpers.CountPages(modelList, maxItemsPerPage);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryExtensions), nameof(DisplayPage), ex);
                return 0;
            }
        }

        public static IEnumerable<LibraryVM> ConvertToViewModel(this IEnumerable<Tlibrary> modelList)
        {
            try
            {
                return modelList?.Select(s => LibraryVM.ConvertToViewModel(s))?.Where(w => w != null) ?? Enumerable.Empty<LibraryVM>();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(LibraryExtensions), nameof(ConvertToViewModel), ex);
                return Enumerable.Empty<LibraryVM>();
            }
        }

        public static IEnumerable<Book> ConvertToViewModel(this IEnumerable<Tbook> modelList)
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
