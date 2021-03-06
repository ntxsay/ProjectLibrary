using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AppHelpers;
using AppHelpers.Dates;
using AppHelpers.Strings;
using LibApi.Helpers;
using LibApi.Models.Local.SQLite;
using LibApi.Services.Categories;
using LibShared;
using LibShared.ViewModels.Books;
using LibShared.ViewModels.Collections;
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
        
        private Book()
        {

        }

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

        public new long Id
        {
            get => _Id;
            private set
            {
                if (_Id != value)
                {
                    _Id = value;
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
        public new int? DayParution
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

        public new int? MonthParution
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

        public new int? YearParution
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

        public new BookIdentification Identification { get; private set; } = new();
        
        public BookClassificationAgeVM ClassificationAge { get; private set; } = new();

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
        public static async Task<Book?> CreateAsync(long idLibrary, string title, string? lang = null, BookFormat? format = null, string? notes = null, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (title.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(title), "Le nom de la catégorie ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();
                var existingId = await GetIdIfExistAsync(title, lang, format == null ? null : LibraryModelList.BookFormatDictionary.GetValueOrDefault((byte)format));
                if (existingId != null)
                {
                    Logs.Log(className: nameof(Book), message: $"Le livre \"{title}\" existe déjà.");
                    if (openIfExist)
                    {
                        var existingItem = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == existingId);
                        if (existingItem != null)
                        {
                            await Book.CompleteBookAsync(context, existingItem);
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

                var record = new Tbook()
                {
                    IdLibrary = idLibrary,
                    Guid = _guid.ToString(),
                    DateAjout = _dateAjout.ToString(),
                    DateEdition = null,
                    MainTitle = title.Trim(),
                    CountOpening = 0,
                    Resume = description,
                    Notes = notes?.Trim(),
                    //Pays = viewModel.Publication?.Pays,
                    Langue = lang?.Trim(),
                };

                await context.Tbooks.AddAsync(record);
                await context.SaveChangesAsync();

                InputOutput inputOutput = new ();
                inputOutput.GetOrCreateDefaultFolderItem(_guid, DefaultFolders.Books);

                await CompleteBookAsync(context, record);
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
        /// <param name="dateParution"></param>
        /// <param name="notes"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(string? title, string? lang = null, string? notes = null, string? description = null)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (title.IsStringNullOrEmptyOrWhiteSpace() && lang == null && notes == null && description == null)
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
                    long? existingId = await GetIdIfExistAsync(title, lang, Format, true, Id)!;
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

                DateTime dateEdition = DateTime.UtcNow;
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
        /// <returns>True si la suppression s'est correctement effectuée sinon False</returns>
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

                bool isDeleted = await DeleteAsync(tValues: new Tbook[] { record }, null);
                if (!isDeleted)
                {
                    throw new Exception("Le livre n'a pas pû être supprimé.");
                }

                IsDeleted = true;

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }

        /// <summary>
        /// Efface tous les enregistrement ou seulement ceux d'une bibliothèque spécifié.
        /// </summary>
        /// <param name="idLibrary">Identifiant de la bibliothèque</param>
        /// <returns>True si la suppression s'est correctement effectuée sinon False</returns>
        internal static async Task<bool> DeleteAsync(long? idLibrary = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                List<Tbook> tValues = idLibrary == null ? await context.Tbooks.ToListAsync() : await context.Tbooks.Where(s => s.IdLibrary == idLibrary).ToListAsync();
                if (tValues == null || !tValues.Any())
                {
                    return true;
                }

                return await DeleteAsync(tValues, null);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }


        internal static async Task<bool> DeleteAsync(IEnumerable<Tbook> tValues, long? idLibrary = null)
        {
            try
            {
                if (tValues == null || !tValues.Any())
                {
                    throw new ArgumentNullException(nameof(tValues), "La liste des modèles ne doit pas être null.");
                }

                List<Tbook> _tValues = idLibrary == null ? tValues.ToList() : tValues.Where(s => s.IdLibrary == idLibrary).ToList();
                if (_tValues == null || !_tValues.Any())
                {
                    return true;
                }

                using LibrarySqLiteDbContext context = new();

                List<TbookCollection>? tbookCollectionsToDelete = new();
                List<TbookOtherTitle>? tbookOtherTitlesToDelete = new();
                List<TbookIdentification>? tbookIdentificationsToDelete = new();
                List<TbookClassification>? tbookClassificationsToDelete = new();
                List<TbookFormat>? tbookFormatsToDelete = new();
                List<TbookContactRoleConnector>? tbookContactRoleConnectorsToDelete = new();
                List<TbookExemplary>? tbookExemplariesToDelete = new();
                List<TbookPret>? tbookPretsToDelete = new();
                List<TbookEtat>? tbookEtatsToDelete = new();

                foreach (var tValue in _tValues)
                {
                    //Titles
                    List<TbookOtherTitle>? recordTitles = await context.TbookOtherTitles.Where(a => a.IdBook == tValue.Id).ToListAsync();
                    if (recordTitles != null && recordTitles.Any())
                    {
                        tbookOtherTitlesToDelete.AddRange(recordTitles);
                    }

                    //Identification
                    TbookIdentification? recordIdentification = await context.TbookIdentifications.SingleOrDefaultAsync(a => a.Id == tValue.Id);
                    if (recordIdentification != null)
                    {
                        tbookIdentificationsToDelete.Add(recordIdentification);
                    }

                    //Classification
                    TbookClassification? recordClassification = await context.TbookClassifications.SingleOrDefaultAsync(a => a.Id == tValue.Id);
                    if (recordClassification != null)
                    {
                        tbookClassificationsToDelete.Add(recordClassification);
                    }

                    //Format
                    TbookFormat? recordFormat = await context.TbookFormats.SingleOrDefaultAsync(a => a.Id == tValue.Id);
                    if (recordFormat != null)
                    {
                        tbookFormatsToDelete.Add(recordFormat);
                    }

                    //Collection connector
                    List<TbookCollection> _tbookCollections = await context.TbookCollections.Where(a => a.IdBook == tValue.Id).ToListAsync();
                    if (_tbookCollections != null && _tbookCollections.Any())
                    {
                        tbookCollectionsToDelete.AddRange(_tbookCollections);
                    }

                    //Contact role connector
                    List<TbookContactRoleConnector>? tbookContactRoleConnectors = await context.TbookContactRoleConnectors.Where(a => a.IdBook == tValue.Id).ToListAsync();
                    if (tbookContactRoleConnectors != null && tbookContactRoleConnectors.Any())
                    {
                        tbookContactRoleConnectorsToDelete.AddRange(tbookContactRoleConnectors);
                    }

                    //Exemplaries
                    List<TbookExemplary>? recordExemplary = await context.TbookExemplaries.Where(a => a.IdBook == tValue.Id).ToListAsync();
                    if (recordExemplary != null && recordExemplary.Any())
                    {
                        foreach (var exemplary in recordExemplary)
                        {
                            //Pret
                            List<TbookPret>? recordPrets = await context.TbookPrets.Where(a => a.IdBookExemplary == exemplary.Id).ToListAsync();
                            if (recordPrets != null && recordPrets.Any())
                            {
                                tbookPretsToDelete.AddRange(recordPrets);
                            }

                            //Etats
                            List<TbookEtat>? recordEtats = await context.TbookEtats.Where(a => a.IdBookExemplary == exemplary.Id).ToListAsync();
                            if (recordEtats != null && recordEtats.Any())
                            {
                                tbookEtatsToDelete.AddRange(recordEtats);
                            }
                        }

                        tbookExemplariesToDelete.AddRange(recordExemplary);
                    }
                }

                //Titles
                if (tbookOtherTitlesToDelete != null && tbookOtherTitlesToDelete.Any())
                {
                    context.TbookOtherTitles.RemoveRange(tbookOtherTitlesToDelete);
                }

                //Identification
                if (tbookIdentificationsToDelete != null && tbookIdentificationsToDelete.Any())
                {
                    context.TbookIdentifications.RemoveRange(tbookIdentificationsToDelete);
                }

                //Classification
                if (tbookClassificationsToDelete != null && tbookClassificationsToDelete.Any())
                {
                    context.TbookClassifications.RemoveRange(tbookClassificationsToDelete);
                }

                //Format
                if (tbookFormatsToDelete != null && tbookFormatsToDelete.Any())
                {
                    context.TbookFormats.RemoveRange(tbookFormatsToDelete);
                }

                //Collection Connector
                if (tbookCollectionsToDelete != null && tbookCollectionsToDelete.Any())
                {
                    context.TbookCollections.RemoveRange(tbookCollectionsToDelete);
                }

                //Contact role connector
                if (tbookContactRoleConnectorsToDelete != null && tbookContactRoleConnectorsToDelete.Any())
                {
                    context.TbookContactRoleConnectors.RemoveRange(tbookContactRoleConnectorsToDelete);
                }

                //Book Etat
                if (tbookEtatsToDelete != null && tbookEtatsToDelete.Any())
                {
                    context.TbookEtats.RemoveRange(tbookEtatsToDelete);
                }

                //Book Pret
                if (tbookPretsToDelete != null && tbookPretsToDelete.Any())
                {
                    context.TbookPrets.RemoveRange(tbookPretsToDelete);
                }

                //Book Exemplary
                if (tbookExemplariesToDelete != null && tbookExemplariesToDelete.Any())
                {
                    context.TbookExemplaries.RemoveRange(tbookExemplariesToDelete);
                }

                context.Tbooks.RemoveRange(_tValues);
                await context.SaveChangesAsync();

                foreach (var tValue in _tValues)
                {
                    if (Guid.TryParse(tValue.Guid, out Guid guid))
                    {
                        InputOutput inputOutput = new();
                        inputOutput.GetOrCreateDefaultFolderItem(guid, DefaultFolders.Books);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Ajoute et/ou met à jour les autres titres.
        /// </summary>
        /// <param name="values"></param>
        /// <remarks>Si vous souhaitez ajouter un ou plusieurs titres tout en conservant l'existant, alors rajoutez l'existant en plus du nouveau en paramètre</remarks>
        /// <returns></returns>
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
                await UpdateDateEditionAsync(record);

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
                        Id = record.Id,
                        Format = format == null ? null : LibraryModelList.BookFormatDictionary.GetValueOrDefault((byte)format),
                        NbOfPages = nbPages is null or < 0 ? null : nbPages,
                        Largeur = largeur is null or < 0 ? null : largeur,
                        Hauteur = hauteur is null or < 0 ? null : hauteur,
                        Epaisseur = epaisseur is null or < 0 ? null : epaisseur,
                        Weight = weight is null or < 0 ? null : weight,
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

                    if (nbPages != null)
                    {
                        tbookFormat.NbOfPages = nbPages < 0 ? null : nbPages;
                    }

                    if (largeur != null)
                    {
                        tbookFormat.Largeur = largeur < 0 ? null : largeur;
                    }

                    if (hauteur != null)
                    {
                        tbookFormat.Hauteur = hauteur < 0 ? null : hauteur;
                    }

                    if (epaisseur != null)
                    {
                        tbookFormat.Epaisseur = epaisseur < 0 ? null : epaisseur;
                    }

                    if (weight != null)
                    {
                        tbookFormat.Weight = weight < 0 ? null : weight;
                    }

                    context.TbookFormats.Update(tbookFormat);
                    await context.SaveChangesAsync();
                }

                if (format != null)
                {
                    Format = tbookFormat.Format;
                }

                if (nbPages != null)
                {
                    NbOfPages = Convert.ToInt16(tbookFormat.NbOfPages);
                }

                if (largeur != null)
                {
                    Largeur = tbookFormat.Largeur;
                }

                if (hauteur != null)
                {
                    Hauteur = tbookFormat.Hauteur;
                }

                if (epaisseur != null)
                {
                    Epaisseur = tbookFormat.Epaisseur;
                }

                if (weight != null)
                {
                    Poids = tbookFormat.Weight;
                }

                await UpdateDateEditionAsync(record);

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }

        public async Task<bool> AddOrUpdateIdentificationAsync(string? isbn = null, string? isbn10 = null, string? isbn13 = null, string? issn = null, string? asin = null, string? cotation = null, string? codeBarre = null)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimé.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (isbn == null && isbn10 == null && isbn13 == null && issn == null && asin == null && cotation == null && codeBarre == null)
                {
                    throw new InvalidOperationException("Au moins un paramètre doit être renseigné.");
                }

                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec l'id \"{Id}\".");
                }

                TbookIdentification? tbookIdentification = await context.TbookIdentifications.SingleOrDefaultAsync(s => s.Id == record.Id);
                if (tbookIdentification == null)
                {
                    tbookIdentification = new TbookIdentification()
                    {
                        Id = record.Id,
                        Isbn = isbn ?? null,
                        Isbn10 = isbn10 ?? null,
                        Isbn13 = isbn13 ?? null,
                        Issn = issn ?? null,
                        Asin = asin ?? null,
                        Cotation = cotation ?? null,
                        CodeBarre = codeBarre ?? null,
                    };

                    await context.TbookIdentifications.AddAsync(tbookIdentification);
                    await context.SaveChangesAsync();
                }
                else
                {
                    if (isbn != null)
                    {
                        tbookIdentification.Isbn = isbn;
                    }

                    if (isbn10 != null)
                    {
                        tbookIdentification.Isbn10 = isbn10;
                    }

                    if (isbn13 != null)
                    {
                        tbookIdentification.Isbn13 = isbn13;
                    }

                    if (issn != null)
                    {
                        tbookIdentification.Issn = issn;
                    }

                    if (asin != null)
                    {
                        tbookIdentification.Asin = asin;
                    }

                    if (cotation != null)
                    {
                        tbookIdentification.Cotation = cotation;
                    }

                    if (codeBarre != null)
                    {
                        tbookIdentification.CodeBarre = codeBarre;
                    }

                    context.TbookIdentifications.Update(tbookIdentification);
                    await context.SaveChangesAsync();
                }

                if (isbn != null)
                {
                    Identification.ISBN = tbookIdentification.Isbn;
                }

                if (isbn10 != null)
                {
                    Identification.ISBN10 = tbookIdentification.Isbn10;
                }

                if (isbn13 != null)
                {
                    Identification.ISBN13 = tbookIdentification.Isbn13;
                }

                if (issn != null)
                {
                    Identification.ISSN = tbookIdentification.Issn;
                }

                if (asin != null)
                {
                    Identification.ASIN = tbookIdentification.Asin;
                }

                if (cotation != null)
                {
                    Identification.Cotation = tbookIdentification.Cotation;
                }

                if (codeBarre != null)
                {
                    Identification.CodeBarre = tbookIdentification.CodeBarre;
                }

                await UpdateDateEditionAsync(record);

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }

        public async Task<bool> AddOrUpdateDateParution(int? day, int? month, int? year)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (day == null && month == null && year == null)
                {
                    throw new InvalidOperationException("Au moins un paramètre doit être renseigné.");
                }

                if (day is not null and > 0 && (month is not null and <= 0 || MonthParution is null or <= 0))
                {
                    throw new InvalidOperationException($"Le mois doit être spécifié avant de valider le jour.");
                }

                if (month is not null and > 0 && (year is not null and <= 0 || YearParution is null or <= 0))
                {
                    throw new InvalidOperationException($"L'année doit être spécifiée avant de valider le mois.");
                }

                string _date;
                if (day != null && month != null && year != null)
                {
                    var isDateCorrect = DateTime.TryParseExact($"{day:00}/{month:00}/{year:0000}", "dd/MM/yyyy", new CultureInfo("fr-FR"), DateTimeStyles.AssumeLocal, out DateTime date);
                    if (!isDateCorrect)
                    {
                        throw new Exception($"La date de parution n'est pas valide..");
                    }
                    else
                    {
                        _date = date.ToString("dd/MM/yyyy");
                    }
                }
                else
                {
                    string date = $"{(day is not null and > 0 ? day.ToString() : (DayParution is not null and > 0 ? DayParution.ToString() : "--"))}/{(month is not null and > 0 ? month.ToString() : (MonthParution is not null and > 0 ? MonthParution.ToString() : "--"))}/{(year is not null and > 0 ? year.ToString() : (YearParution is not null and > 0 ? YearParution.ToString() : "--"))}";
                    _date = date;
                }

                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec l'id \"{Id}\".");
                }

                DateTime dateEdition = DateTime.UtcNow;
                record.DateEdition = dateEdition.ToString();
                record.DateParution = _date;
                context.Tbooks.Update(record);
                await context.SaveChangesAsync();

                if (day != null)
                {
                    DayParution = day <= 0 ? null : day;
                }

                if (month != null)
                {
                    MonthParution = month <= 0 ? null : month;
                }

                if (year != null)
                {
                    YearParution = year <= 0 ? null : year;
                }

                DateParution = _date;
                DateEdition = dateEdition;
                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }

        public async Task<bool> AddOrUpdateClassificationAgeAsync(ClassificationAge type, byte? minAge, byte? maxAge)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimé.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (minAge == null && maxAge == null)
                {
                    throw new InvalidOperationException("Au moins un paramètre doit être renseigné.");
                }

                if (ClassificationAge.TypeClassification == LibShared.ClassificationAge.ToutPublic)
                {
                    minAge = 0;
                    maxAge = 0;
                }
                else if (ClassificationAge.TypeClassification == LibShared.ClassificationAge.ApartirDe)
                {
                    if (minAge is null or < 0)
                    {
                        throw new InvalidOperationException("L'âge minimal ne doit pas être inférieur à 0.");
                    }
                    maxAge = 0;
                }
                else if (ClassificationAge.TypeClassification == LibShared.ClassificationAge.Jusqua)
                {
                    if (maxAge is null or < 1)
                    {
                        throw new InvalidOperationException("L'âge ne doit pas être inférieur à 1.");
                    }
                    minAge = 0;
                }
                else if (ClassificationAge.TypeClassification == LibShared.ClassificationAge.DeTantATant)
                {
                    if (maxAge is null or < 1)
                    {
                        throw new InvalidOperationException("L'âge ne doit pas être inférieur à 1.");
                    }

                    if (minAge is null or < 0)
                    {
                        throw new InvalidOperationException("L'âge minimal ne doit pas être inférieur à 0.");
                    }
                    if (minAge > maxAge)
                    {
                        throw new InvalidOperationException("L'âge minimal ne peut pas être supérieur à l'âge maximal.");
                    }
                }

                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(a => a.Id == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec l'id \"{Id}\".");
                }

                TbookClassification? tbookClassificationAge = await context.TbookClassifications.SingleOrDefaultAsync(s => s.Id == record.Id);
                if (tbookClassificationAge == null)
                {
                    tbookClassificationAge = new TbookClassification()
                    {
                        Id = record.Id,
                        TypeClassification = (byte)type,
                        DeTelAge = minAge != null ? (byte)minAge : 0,//Remplacer par minimumAge
                        AtelAge = maxAge != null ? (byte)maxAge : 0,
                    };

                    await context.TbookClassifications.AddAsync(tbookClassificationAge);
                    await context.SaveChangesAsync();
                }
                else
                {
                    tbookClassificationAge.TypeClassification = (byte)type;
                    tbookClassificationAge.DeTelAge = minAge != null ? (byte)minAge : 0;//Remplacer par minimumAge
                    tbookClassificationAge.AtelAge = maxAge != null ? (byte)maxAge : 0;
                    
                    context.TbookClassifications.Update(tbookClassificationAge);
                    await context.SaveChangesAsync();
                }

                ClassificationAge.TypeClassification = (LibShared.ClassificationAge)tbookClassificationAge.TypeClassification;
                ClassificationAge.MinimumAge = Convert.ToByte(tbookClassificationAge.DeTelAge);
                ClassificationAge.MaximumAge = Convert.ToByte(tbookClassificationAge.AtelAge);

                await UpdateDateEditionAsync(record);

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }


        #region Collections
        public async Task<IEnumerable<Collections.Collection>> GetCollectionsAsync()
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimée.");
                }

                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec l'id \"{Id}\".");
                }

                List<TbookCollection>? values = await context.TbookCollections.Where(a => a.IdBook == Id).ToListAsync();
                if (values != null && values.Any())
                {
                    List<Tcollection> collections = new();
                    List<long> idCollections = values.Select(a => a.Id).ToList();
                    foreach (var idCollection in idCollections)
                    {
                        var value = await context.Tcollections.SingleOrDefaultAsync(a => a.Id == idCollection);
                        if (value != null) collections.Add(value);
                    }

                    if (collections.Any())
                    {
                        return collections.Select(s => Collections.Collection.ConvertToViewModel(s)).Where(w => w != null);
                    }
                }
                return Enumerable.Empty<Collections.Collection>();
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return Enumerable.Empty<Collections.Collection>();
            }
        }

        public async Task<bool> AddCollectionsAsync(CollectionVM[] values)
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

                foreach (var v in values)
                {
                    long? idCollection = await Collections.Collection.GetIdIfExistAsync(IdLibrary, v);
                    if (idCollection == null)
                    {
                        Collections.Collection? nCollection = await Collections.Collection.CreateAsync(IdLibrary, v);
                        idCollection = nCollection?.Id;
                    }

                    if (idCollection != null)
                    {
                        if (!await context.TbookCollections.AnyAsync(a => a.IdCollection == (long)idCollection && a.IdBook == Id))
                        {
                            TbookCollection tbookCollection = new()
                            {
                                IdBook = Id,
                                IdCollection = (long)idCollection,
                            };

                            await context.TbookCollections.AddAsync(tbookCollection);
                            await context.SaveChangesAsync();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }

        public async Task<bool> RemoveCollectionsAsync(CollectionVM[] values)
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

                return await RemoveCollectionsAsync(values.Select(s => s.Id).ToArray());
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }

        public async Task<bool> RemoveCollectionsAsync(long[] idCollections)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (idCollections == null)
                {
                    throw new ArgumentNullException(nameof(idCollections), "le paramètre ne doit pas être null.");
                }

                foreach (var id in idCollections)
                {
                    List<TbookCollection>? values = await context.TbookCollections.Where(a => a.IdCollection == id && a.IdBook == Id).ToListAsync();
                    if (values != null && values.Any())
                    {
                        context.TbookCollections.RemoveRange(values);
                        await context.SaveChangesAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }
        #endregion

        #region Jaquette
        public async Task<bool> SaveJaquetteAsync(byte[] fileBytes, string fileName)
        {
            try
            {
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    throw new ArgumentNullException(nameof(fileBytes), "Ce parmètre ne peut pas être null.");
                }

                if (fileName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(fileName), "Ce parmètre ne peut pas être null ou ne contenir que des espaces blancs.");
                }

                InputOutput inputOutput = new();
                return await inputOutput.CopyJaquetteFileAsync(Guid, DefaultFolders.Books, fileBytes, fileName);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Book), exception: ex);
                return false;
            }
        }

        public async Task<byte[]> GetJaquetteBytesAsync()
        {
            try
            {
                InputOutput inputOutput = new();
                return await inputOutput.GetJaquetteFileAsync(Guid, DefaultFolders.Books);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Book), exception: ex);
                return Array.Empty<byte>();
            }
        }
        #endregion

        public async Task<bool> AddToCategoryAsync(Category category)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimée.");
                }

                if (category == null)
                {
                    throw new ArgumentNullException(nameof(category), $"La catégorie ne doit pas être null.");
                }

                if (category.IdLibrary != IdLibrary)
                {
                    throw new InvalidOperationException($"Impossible d'ajouter ce livre à une catégorie provenant d'une autre bibliothèque.");
                }

                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(w => w.Id == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec l'id \"{Id}\".");
                }

                DateTime dateEdition = DateTime.UtcNow;
                record.DateEdition = dateEdition.ToString();
                record.IdCategorie = category.Id;
                
                context.Tbooks.Update(record);
                _ = await context.SaveChangesAsync();
                DateEdition = dateEdition;
                IdCategorie = category.Id;
                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className:nameof(Book), exception:ex);
                return false;
            }
        }

        public async Task<bool> RemoveCategory()
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"Le livre {MainTitle} a déjà été supprimée.");
                }

                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(w => w.Id == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec l'id \"{Id}\".");
                }

                if (record.IdCategorie != null)
                {
                    DateTime dateEdition = DateTime.UtcNow;
                    record.DateEdition = dateEdition.ToString();
                    record.IdCategorie = null;

                    context.Tbooks.Update(record);
                    _ = await context.SaveChangesAsync();
                    DateEdition = dateEdition;
                    IdCategorie = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return false;
            }
        }

        public async Task<Category?> GetCategorieAsync()
        {
            try
            {
                TlibraryCategorie? tlibraryCategorie = await context.TlibraryCategories.SingleOrDefaultAsync(w => w.Id == IdCategorie && w.IdLibrary == IdLibrary);
                if (tlibraryCategorie != null)
                {
                    return Category.ConvertToViewModel(tlibraryCategorie);
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return null;
            }
        }

        #region Static methods
        public static async Task<IEnumerable<Book>> AllAsync(long? idLibrary = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                List<Tbook>? modelList = new();

                if (idLibrary == null)
                {
                    modelList = await context.Tbooks.ToListAsync();
                }
                else
                {
                    modelList = await context.Tbooks.Where(w => w.IdLibrary == (long)idLibrary).ToListAsync();
                }

                if (modelList == null || !modelList.Any())
                {
                    return Enumerable.Empty<Book>();
                }

                modelList.ForEach(async fe => await CompleteBookAsync(context, fe));
                return modelList.Select(s => ConvertToViewModel(s)).Where(w => w != null)!;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return Enumerable.Empty<Book>();
            }
        }

        public static async Task<Book?> SingleAsync(long id)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                Tbook? record = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec l'id \"{id}\".");
                }

                await CompleteBookAsync(context, record);
                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return null;
            }
        }

        public static async Task<Book?> SingleAsync(string titleName, string? lang = null, BookFormat? format = null, long? idLibrary = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                if (titleName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(titleName), $"Le nom du livre ne doit pas être null, vide ou ne contenir que des espaces blancs.");
                }

                Tbook? record = await GetDbModelIfExistAsync(mainTitle: titleName, lang: lang, format: format == null ? null : LibraryModelList.BookFormatDictionary.GetValueOrDefault((byte)format), isEdit: false, modelId: null, idLibrary: idLibrary);
                if (record == null)
                {
                    throw new NullReferenceException($"Le livre n'existe pas avec le nom \"{titleName}\".");
                }

                await CompleteBookAsync(context, record);
                return ConvertToViewModel(record);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return null;
            }
        }

        public static async Task<IEnumerable<Book>> MultiplesAsync(long[] idArray, long? idLibrary = null)
        {
            try
            {
                if (idArray == null || !idArray.Any())
                {
                    throw new ArgumentNullException(nameof(idArray), $"Le tableau d'id ne doit pas être null et doit contenir au moins un élément.");
                }

                using LibrarySqLiteDbContext context = new();

                List<Tbook> tmodelList = new();
                foreach (long id in idArray)
                {
                    Tbook? record = null;

                    if (idLibrary == null)
                    {
                        record = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == id);
                    }
                    else
                    {
                        record = await context.Tbooks.SingleOrDefaultAsync(s => s.Id == id && s.IdLibrary == idLibrary);
                    }

                    if (record == null)
                    {
                        Logs.Log(className: nameof(Book), message: $"Le livre n'existe pas avec l'id \"{id}\".");
                        continue;
                    }

                    tmodelList.Add(record);
                }

                return tmodelList.Select(s => ConvertToViewModel(s))!;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Book), exception: ex);
                return Enumerable.Empty<Book>();
            }
        }


        public static async Task<long?> GetIdIfExistAsync(string mainTitle, string? lang, string? format, bool isEdit = false, long? modelId = null, long? idLibrary = null)
        {
            try
            {
                var item = await GetDbModelIfExistAsync(mainTitle: mainTitle, lang: lang, format: format, isEdit: isEdit, modelId: modelId, idLibrary: idLibrary);
                return item?.Id;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return null;
            }
        }

        internal static async Task<Tbook?> GetDbModelIfExistAsync(string mainTitle, string? lang, string? format, bool isEdit = false, long? modelId = null, long? idLibrary = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();
                List<Tbook> existingItemList = new();

                if (mainTitle.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                mainTitle = mainTitle.Trim().ToLower();

                if (!isEdit)
                {
                    if (idLibrary == null)
                    {
                        existingItemList = await context.Tbooks.Where(c => c.MainTitle.ToLower() == mainTitle).ToListAsync();
                    }
                    else
                    {
                        existingItemList = await context.Tbooks.Where(c => c.MainTitle.ToLower() == mainTitle && c.IdLibrary == (long)idLibrary).ToListAsync();
                    }
                }
                else if (isEdit && modelId != null)
                {
                    if (idLibrary == null)
                    {
                        existingItemList = await context.Tbooks.Where(c => c.Id != (long)modelId && c.MainTitle.ToLower() == mainTitle).ToListAsync();
                    }
                    else
                    {
                        existingItemList = await context.Tbooks.Where(c => c.Id != (long)modelId && c.MainTitle.ToLower() == mainTitle && c.IdLibrary == (long)idLibrary).ToListAsync();
                    }
                }

                if (existingItemList != null && existingItemList.Any())
                {
                    foreach (var item in existingItemList)
                    {
                        item.TbookFormat = await context.TbookFormats.SingleOrDefaultAsync(c => c.Id == item.Id);
                        if (item.TbookFormat?.Format?.ToLower() == format && item.Langue?.ToLower() == lang)
                        {
                            return item;
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

        internal static async Task CompleteBookAsync(LibrarySqLiteDbContext context, Tbook model)
        {
            try
            {
                if (context == null || model == null)
                {
                    return;
                }

                model.TbookClassification = await context.TbookClassifications.SingleOrDefaultAsync(s => s.Id == model.Id)!;
                model.TbookFormat = await context.TbookFormats.SingleOrDefaultAsync(s => s.Id == model.Id)!;
                model.TbookIdentification = await context.TbookIdentifications.SingleOrDefaultAsync(s => s.Id == model.Id)!;
                //Facultatif
                //model.TbookCollections = await context.TbookCollections.Where(w => w.IdBook == model.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return;
            }
        }

        public static Book? ConvertToViewModel(Tbook model)
        {
            try
            {
                if (model == null) return null;

                var isGuidCorrect = Guid.TryParse(model.Guid, out Guid guid);
                if (isGuidCorrect == false) return null;

                Book viewModel = new()
                {
                    Id = model.Id,
                    IdLibrary = model.IdLibrary,
                    IdCategorie = model.IdCategorie,
                    Guid = isGuidCorrect ? guid : Guid.Empty,
                    DateAjout = DateHelpers.Converter.GetDateFromString(model.DateAjout).ToLocalTime(),
                    DateEdition = DateHelpers.Converter.GetNullableDateFromString(model.DateEdition)?.ToLocalTime(),
                    MainTitle = model.MainTitle,
                    CountOpening = model.CountOpening,
                    Langue = model.Langue,
                    Notes = model.Notes,
                    Resume = model.Resume,

                };

                if (model.TbookIdentification != null)
                {
                    viewModel.Identification.ISBN = model.TbookIdentification.Isbn;
                    viewModel.Identification.ISBN10 = model.TbookIdentification.Isbn10;
                    viewModel.Identification.ISBN13 = model.TbookIdentification.Isbn13;
                    viewModel.Identification.ISSN = model.TbookIdentification.Issn;
                    viewModel.Identification.ASIN = model.TbookIdentification.Asin;
                    viewModel.Identification.CodeBarre = model.TbookIdentification.CodeBarre;
                    viewModel.Identification.Cotation = model.TbookIdentification.Cotation;
                }

                if (model.TbookFormat != null)
                {
                    viewModel.Format = model.TbookFormat.Format;
                    viewModel.NbOfPages = model.TbookFormat.NbOfPages is null or < short.MaxValue ? null : (short)model.TbookFormat.NbOfPages;
                    viewModel.Hauteur = model.TbookFormat.Hauteur ?? 0;
                    viewModel.Epaisseur = model.TbookFormat?.Epaisseur ?? 0;
                    viewModel.Largeur = model.TbookFormat?.Largeur ?? 0;
                    viewModel.Poids = model.TbookFormat?.Weight ?? 0;
                }

                var dateParution = DateHelpers.Converter.StringDateToStringDate(model.DateParution, '/', out int? dayParution, out int? monthParution, out int? yearParution);
                viewModel.DateParution = dateParution;
                viewModel.DayParution = dayParution;
                viewModel.MonthParution = monthParution;
                viewModel.YearParution = yearParution;

                return viewModel;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return null;
            }
        } 
        #endregion

        private async Task UpdateDateEditionAsync(Tbook record)
        {
            try
            {
                if (record != null)
                {
                    DateTime dateEdition = DateTime.UtcNow;
                    record.DateEdition = dateEdition.ToString();

                    context.Tbooks.Update(record);
                    _ = await context.SaveChangesAsync();

                    DateEdition = dateEdition;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(Book), exception: ex);
                return;
            }
        }

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }

        public async Task DisposeAsync()
        {
            if (context != null)
            {
                await context.DisposeAsync();
            }
        }
    }
}
