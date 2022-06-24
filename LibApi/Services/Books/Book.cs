using System;
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
using LibShared;
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
        public new ObservableCollection<string> OtherTitles
        {
            get => _OtherTitles;
            private set
            {
                if (_OtherTitles != value)
                {
                    _OtherTitles = value;
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

        #region Format
        public new string? Format
        {
            get => _Format;
            private set
            {
                if (_Format != value)
                {
                    _Format = value;
                    OnPropertyChanged();
                }
            }
        }

        public new short? NbOfPages
        {
            get => _NbOfPages;
            private set
            {
                if (_NbOfPages != value)
                {
                    _NbOfPages = value;
                    OnPropertyChanged();
                }
            }
        }

        public new double? Hauteur
        {
            get => _Hauteur;
            private set
            {
                if (_Hauteur != value)
                {
                    _Hauteur = value;
                    OnPropertyChanged();
                }
            }
        }

        public new double? Largeur
        {
            get => _Largeur;
            private set
            {
                if (_Largeur != value)
                {
                    _Largeur = value;
                    OnPropertyChanged();
                }
            }
        }

        public new double? Epaisseur
        {
            get => _Epaisseur;
            private set
            {
                if (_Epaisseur != value)
                {
                    _Epaisseur = value;
                    OnPropertyChanged();
                }
            }
        }

        public new double? Poids
        {
            get => _Poids;
            private set
            {
                if (_Poids != value)
                {
                    _Poids = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion


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
        /// Ajoute un nouveau livre dans une bibliothèque.
        /// </summary>
        /// <param name="name">Nom de la collection</param>
        /// <param name="description">Description de la collection</param>
        /// <remarks>Si la collection existe, la collection existante sera retournée.</remarks>
        /// <returns></returns>
        public static async Task<Book?> CreateAsync(long idLibrary, string title, string? lang = null, string? format = null, string? dateParution = null, string? notes = null, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (title.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(title), "Le nom de la catégorie ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();
                var existingId = await Book.IsBookExistAsync(title, lang, format);
                if (existingId != null)
                {
                    Logs.Log(className: nameof(Book), message: $"Le livre \"{title}\" existe déjà.");
                    if (openIfExist)
                    {
                        var existingItem = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == existingId);
                        if (existingItem != null)
                        {
                            return ConvertToViewModel(existingItem);
                        }

                        throw new Exception($"Impossible de récupérer le livre existant.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Le livre \"{title}\" existe déjà.");
                    }
                }

                var _dateAjout = DateTime.UtcNow;
                var _guid = System.Guid.NewGuid();
                var _dateParution = DateHelpers.Converter.StringDateToStringDate(dateParution, '/', out _, out _, out _, false);

                var record = new Tbook()
                {
                    IdLibrary = idLibrary,
                    Guid = _guid.ToString(),
                    DateAjout = _dateAjout.ToString(),
                    DateEdition = null,
                    DateParution = _dateParution,
                    MainTitle = title.Trim(),
                    CountOpening = 0,
                    Resume = description,
                    Notes = notes?.Trim(),
                    //Pays = viewModel.Publication?.Pays,
                    Langue = lang?.Trim(),
                };

                await context.Tbooks.AddAsync(record);
                await context.SaveChangesAsync();

                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return null;
            }
        }

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

        public async Task<bool> AddOrUpdateOtherTitles(string[] values)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (values == null)
                {
                    throw new ArgumentNullException(nameof(values), "le paramètre ne doit pas être null.");
                }

                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec l'id \"{Id}\".");
                }

                var recordTitles = await context.TbookOtherTitles.Where(a => a.IdBook == record.Id).ToListAsync();
                if (recordTitles.Any())
                {
                    context.TbookOtherTitles.RemoveRange(recordTitles);
                }

                if (values.Any())
                {
                    var valueRange = values.Where(w => w != null).Select(s => new TbookOtherTitle()
                    {
                        IdBook = record.Id,
                        Title = s
                    }).ToList();
                    await context.TbookOtherTitles.AddRangeAsync(valueRange);
                }

                await context.SaveChangesAsync();
                OtherTitles = new ObservableCollection<string>(values.Where(w => w != null));

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }

        public async Task<bool> AddOrUpdateFormatAsync(BookFormat? format = null, short? nbPages = null, double? largeur = null, double? hauteur = null, double? epaisseur = null, double? weight = null)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimé.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (format == null && nbPages == null && largeur == null && hauteur == null && epaisseur == null && weight == null)
                {
                    throw new InvalidOperationException("Au moins un paramètre doit être renseigné.");
                }

                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec l'id \"{Id}\".");
                }

                TbookFormat? tbookFormat = await context.TbookFormats.SingleOrDefaultAsync(s => s.Id == Id);
                if (tbookFormat == null)
                {
                    tbookFormat = new TbookFormat()
                    {
                        Id = Id,
                        Format = format == null ? null : LibraryModelList.BookFormatDictionary.GetValueOrDefault((byte)format),
                        NbOfPages = nbPages == null || nbPages < 0 ? null : nbPages,
                        Largeur = largeur == null || largeur < 0 ? null : largeur,
                        Hauteur = hauteur == null || hauteur < 0 ? null : hauteur,
                        Epaisseur = epaisseur == null || epaisseur < 0 ? null : epaisseur,
                        Weight = weight == null || weight < 0 ? null : weight,
                    };

                    await context.TbookFormats.AddAsync(tbookFormat);
                    await context.SaveChangesAsync();
                }
                else
                {
                    if (format != null)
                    {
                        tbookFormat.Format = LibraryModelList.BookFormatDictionary.GetValueOrDefault((byte)format);
                    }

                    if (nbPages != null && nbPages <= 0)
                    {
                        tbookFormat.NbOfPages = nbPages;
                    }

                    if (largeur != null && largeur <= 0)
                    {
                        tbookFormat.Largeur = largeur;
                    }

                    if (hauteur != null && hauteur <= 0)
                    {
                        tbookFormat.Hauteur = hauteur;
                    }

                    if (epaisseur != null && epaisseur <= 0)
                    {
                        tbookFormat.Epaisseur = epaisseur;
                    }

                    if (weight != null && weight <= 0)
                    {
                        tbookFormat.Weight = weight;
                    }

                    context.TbookFormats.Update(tbookFormat);
                    await context.SaveChangesAsync();
                }

                if (tbookFormat != null)
                {
                    if (format != null)
                    {
                        Format = tbookFormat.Format;
                    }

                    if (nbPages != null && nbPages <= 0)
                    {
                        NbOfPages = Convert.ToInt16(tbookFormat.NbOfPages);
                    }

                    if (largeur != null && largeur <= 0)
                    {
                        Largeur = tbookFormat.Largeur;
                    }

                    if (hauteur != null && hauteur <= 0)
                    {
                        Hauteur = tbookFormat.Hauteur;
                    }

                    if (epaisseur != null && epaisseur <= 0)
                    {
                        Epaisseur = tbookFormat.Epaisseur;
                    }

                    if (weight != null && weight <= 0)
                    {
                        Poids = tbookFormat.Weight;
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
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
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimée.");
                }

                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(a => a.Id == Id);
                if (record == null)
                {
                    throw new ArgumentNullException(nameof(Tlibrary), $"Le livre n'existe pas avec l'id \"{Id}\".");
                }

                //Titles
                var recordTitles = await context.TbookOtherTitles.Where(a => a.IdBook == record.Id).ToListAsync();
                if (recordTitles.Any())
                {
                    context.TbookOtherTitles.RemoveRange(recordTitles);
                }

                //Identification
                TbookIdentification? recordIdentification = await context.TbookIdentifications.SingleOrDefaultAsync(a => a.Id == record.Id);
                if (recordIdentification != null)
                {
                    context.TbookIdentifications.Remove(recordIdentification);
                }

                //Classification
                TbookClassification? recordClassification = await context.TbookClassifications.SingleOrDefaultAsync(a => a.Id == record.Id);
                if (recordClassification != null)
                {
                    context.TbookClassifications.Remove(recordClassification);
                }

                //Format
                TbookFormat? recordFormat = await context.TbookFormats.SingleOrDefaultAsync(a => a.Id == record.Id);
                if (recordFormat != null)
                {
                    context.TbookFormats.Remove(recordFormat);
                }

                //Collection connecto
                var recordCollection = await context.TbookCollections.Where(a => a.IdBook == record.Id).ToListAsync();
                if (recordCollection.Any())
                {
                    context.TbookCollections.RemoveRange(recordCollection);
                }

                //Exemplaries
                var recordExemplary = await context.TbookExemplaries.Where(a => a.IdBook == record.Id).ToListAsync();
                if (recordExemplary.Any())
                {
                    foreach (var exemplary in recordExemplary)
                    {
                        //Pret
                        var recordPrets = await context.TbookPrets.Where(a => a.IdBookExemplary == exemplary.Id).ToListAsync();
                        if (recordPrets != null)
                        {
                            context.TbookPrets.RemoveRange(recordPrets);
                        }

                        //Etats
                        var recordEtats = await context.TbookEtats.Where(a => a.IdBookExemplary == exemplary.Id).ToListAsync();
                        if (recordEtats != null)
                        {
                            context.TbookEtats.RemoveRange(recordEtats);
                        }
                    }
                    context.TbookExemplaries.RemoveRange(recordExemplary);
                }

                context.Tbooks.Remove(record);
                await context.SaveChangesAsync();

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
