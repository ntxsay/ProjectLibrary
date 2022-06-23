﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using LibShared.ViewModels.Contacts;
using Microsoft.EntityFrameworkCore;

namespace LibApi.Services.Books
{
    public sealed class Book : BookVM, IDisposable
    {
        /// <summary>
        /// Obtient une valeur booléenne indiquant si l'objet a déjà été effacé de la base de données
        /// </summary>
        public bool IsDeleted { get; private set; }

        #region Properties
        [DisplayName("Titre du livre")]
        public new string MainTitle
        {
            get => _MainTitle;
            private set
            {
                if (_MainTitle != value)
                {
                    _MainTitle = value;
                    OnPropertyChanged();
                }
            }
        }


        [DisplayName("Autre(s) titre(s)")]
        public new ObservableCollection<string> TitresOeuvre
        {
            get => _TitresOeuvre;
            private set
            {
                if (_TitresOeuvre != value)
                {
                    _TitresOeuvre = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string? TitresOeuvreStringList
        {
            get => _TitresOeuvreStringList;
            private set
            {
                if (_TitresOeuvreStringList != value)
                {
                    _TitresOeuvreStringList = value;
                    OnPropertyChanged();
                }
            }
        }

        public new ObservableCollection<ContactVM> Auteurs
        {
            get => _Auteurs;
            private set
            {
                if (_Auteurs != value)
                {
                    _Auteurs = value;
                    OnPropertyChanged();
                }
            }
        }

        public new  string? AuteursStringList
        {
            get => _AuteursStringList;
            private set
            {
                if (_AuteursStringList != value)
                {
                    _AuteursStringList = value;
                    OnPropertyChanged();
                }
            }
        }

        public new DateTime DateAjout
        {
            get => _DateAjout;
            private set
            {
                if (_DateAjout != value)
                {
                    _DateAjout = value;
                    OnPropertyChanged();
                }
            }
        }

        public new DateTime? DateEdition
        {
            get => _DateEdition;
            private set
            {
                if (_DateEdition != value)
                {
                    _DateEdition = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Publication
        public new string? DayParution
        {
            get => this._DayParution;
            private set
            {
                if (this._DayParution != value)
                {
                    this._DayParution = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public new string? MonthParution
        {
            get => this._MonthParution;
            private set
            {
                if (this._MonthParution != value)
                {
                    this._MonthParution = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public new string? YearParution
        {
            get => this._YearParution;
            private set
            {
                if (this._YearParution != value)
                {
                    this._YearParution = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public new string? DateParution
        {
            get => _DateParution;
            private set
            {
                if (_DateParution != value)
                {
                    _DateParution = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string? CollectionsStringList
        {
            get => _CollectionsStringList;
            private set
            {
                if (_CollectionsStringList != value)
                {
                    _CollectionsStringList = value;
                    OnPropertyChanged();
                }
            }
        }

        public new ObservableCollection<ContactVM> Editeurs
        {
            get => _Editeurs;
            private set
            {
                if (_Editeurs != value)
                {
                    _Editeurs = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string? EditeursStringList
        {
            get => _EditeursStringList;
            private set
            {
                if (_EditeursStringList != value)
                {
                    _EditeursStringList = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string? Pays
        {
            get => _Pays;
            private set
            {
                if (_Pays != value)
                {
                    _Pays = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string? Langue
        {
            get => _Langue;
            private set
            {
                if (_Langue != value)
                {
                    _Langue = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Description
        [DisplayName("Résumé")]
        public new string? Resume
        {
            get => _Resume;
            private set
            {
                if (_Resume != value)
                {
                    _Resume = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string? Notes
        {
            get => _Notes;
            private set
            {
                if (_Notes != value)
                {
                    _Notes = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #endregion

        readonly LibrarySqLiteDbContext context = new();

        #region CRUD
        /// <summary>
        /// Met à jour le livre dans la base de données
        /// </summary>
        /// <param name="title"></param>
        /// <param name="lang"></param>
        /// <param name="format"></param>
        /// <param name="dateParution"></param>
        /// <param name="notes"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(string? title = null, string? lang = null, string? format = null, string? dateParution = null, string? notes = null, string? description = null)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (title.IsStringNullOrEmptyOrWhiteSpace() && lang == null && format == null && dateParution == null && notes == null && description == null)
                {
                    throw new InvalidOperationException("Au moins un paramètre doit être renseigné.");
                }

                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec l'id \"{Id}\".");
                }

                if (!title.IsStringNullOrEmptyOrWhiteSpace())
                {
                    long? existingId = await Book.IsBookExistAsync(title, lang, format, true, Id)!;
                    if (existingId != null)
                    {
                        Logs.Log(className:nameof(Book), message: "Ce livre existe déjà");
                        return false;
                    }

                    record.MainTitle = title.Trim();
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (lang != null)
                {
                    record.Langue = lang?.Trim();
                }

                ////La mise à jour de la description n'est pas ignorée si elle est différent de null.
                //if (format != null)
                //{
                //    record.Resume = format?.Trim();
                //}

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (dateParution != null)
                {
                    record.DateParution = dateParution?.Trim();
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (notes != null)
                {
                    record.Notes = notes?.Trim();
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (description != null)
                {
                    record.Resume = description?.Trim();
                }

                DateTime dateEdition = DateTime.Now;
                record.DateEdition = dateEdition.ToString();

                context.Tbooks.Update(record);
                _ = await context.SaveChangesAsync();

                DateEdition = dateEdition;

                if (!title.IsStringNullOrEmptyOrWhiteSpace())
                {
                    MainTitle = record.MainTitle;
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (lang != null)
                {
                    Langue = record.Langue;
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (dateParution != null)
                {
                    DateParution = record.DateParution;
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (notes != null)
                {
                    Notes = record.Notes;
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (description != null)
                {
                    Resume = record.Resume;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className:nameof(Book), exception:ex);
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

                Tlibrary? tlibrary = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == Id);
                if (tlibrary == null)
                {
                    throw new ArgumentNullException(nameof(Tlibrary), $"La bibliothèque n'existe pas avec l'id \"{Id}\".");
                }

                List<Tcollection>? tcollections = await context.Tcollections.Where(s => s.IdLibrary == Id).ToListAsync();
                if (tcollections.Any())
                {
                    context.Tcollections.RemoveRange(tcollections);
                }

                context.Tlibraries.Remove(tlibrary);
                _ = await context.SaveChangesAsync();

                IsDeleted = true;
                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }
        #endregion

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
