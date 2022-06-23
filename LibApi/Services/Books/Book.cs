using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AppHelpers;
using AppHelpers.Dates;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using LibShared.ViewModels.Books;
using Microsoft.EntityFrameworkCore;

namespace LibApi.Services.Books
{
    public sealed class Book : BookVM, IDisposable
    {
        readonly LibrarySqLiteDbContext context = new();

        internal static async Task<long?> IsBookExistAsync(string mainTitle, string lang, string format, bool isEdit = false, long? modelId = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                List<Tbook> existingItemList = new ();

                if (mainTitle.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                mainTitle = mainTitle.Trim().ToLower();

                if (!isEdit)
                {
                    existingItemList = await context.Tbooks.Where(c => c.MainTitle.ToLower() == mainTitle).ToListAsync();
                }
                else if (isEdit && modelId != null)
                {
                    existingItemList = await context.Tbooks.Where(c => c.Id != (long)modelId && c.MainTitle.ToLower() == mainTitle).ToListAsync();
                }

                if (existingItemList != null && existingItemList.Any())
                {
                    foreach (var item in existingItemList)
                    {
                        item.TbookFormat = await context.TbookFormats.SingleOrDefaultAsync(c => c.Id == item.Id);
                        if (item.TbookFormat?.Format?.ToLower() == format && item.Langue?.ToLower() == lang)
                        {
                            return item.Id;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return null;
            }
        }

        public static Book? ConvertToViewModel(Tbook model)
        {
            try
            {
                if (model == null) return null;


                var isGuidCorrect = Guid.TryParse(model.Guid, out Guid guid);
                if (isGuidCorrect == false) return null;

                Book viewModel = new ()
                {
                    Id = model.Id,
                    IdLibrary = model.IdLibrary,
                    Guid = isGuidCorrect ? guid : Guid.Empty,
                    DateAjout = DateHelpers.Converter.GetDateFromString(model.DateAjout).ToLocalTime(),
                    DateEdition = DateHelpers.Converter.GetNullableDateFromString(model.DateEdition)?.ToLocalTime(),
                    MainTitle = model.MainTitle,
                    CountOpening = model.CountOpening,
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                Logs.Log(className:nameof(Book), exception:ex);
                return null;
            }
        }
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
