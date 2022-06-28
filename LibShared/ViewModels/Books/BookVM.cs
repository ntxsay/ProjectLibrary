using LibShared.ViewModels.Collections;
using LibShared.ViewModels.Contacts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibShared.ViewModels.Books
{
    public abstract class BookVM : GenericVM
    {
        [JsonIgnore]
        public long IdLibrary { get; protected set; }
        public long? IdCategorie { get; protected set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public long CountOpening { get; set; }

        protected string _MainTitle = "";

        [DisplayName("Titre du livre")]
        public string MainTitle
        {
            get => _MainTitle;
            set
            {
                if (_MainTitle != value)
                {
                    _MainTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        protected ObservableCollection<string> _OtherTitles = new ObservableCollection<string>();

        [DisplayName("Autre(s) titre(s)")]
        public ObservableCollection<string> OtherTitles
        {
            get => _OtherTitles;
            set
            {
                if (_OtherTitles != value)
                {
                    _OtherTitles = value;
                    OnPropertyChanged();
                }
            }
        }

        protected ObservableCollection<ContactVM> _Auteurs = new ObservableCollection<ContactVM>();
        public ObservableCollection<ContactVM> Auteurs
        {
            get => _Auteurs;
            set
            {
                if (_Auteurs != value)
                {
                    _Auteurs = value;
                    OnPropertyChanged();
                }
            }
        }

        protected DateTime _DateAjout = DateTime.UtcNow;
        public DateTime DateAjout
        {
            get => _DateAjout;
            set
            {
                if (_DateAjout != value)
                {
                    _DateAjout = value;
                    OnPropertyChanged();
                }
            }
        }

        protected DateTime? _DateEdition;
        public DateTime? DateEdition
        {
            get => _DateEdition;
            set
            {
                if (_DateEdition != value)
                {
                    _DateEdition = value;
                    OnPropertyChanged();
                }
            }
        }

        //private ObservableCollection<LivreEtatVM> _EtatLivre = new ObservableCollection<LivreEtatVM>();
        //public ObservableCollection<LivreEtatVM> EtatLivre
        //{
        //    get => _EtatLivre;
        //    set
        //    {
        //        if (_EtatLivre != value)
        //        {
        //            _EtatLivre = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        protected string _JaquettePath = String.Empty;

        [JsonIgnore]
        public string JaquettePath
        {
            get => this._JaquettePath;
            set
            {
                if (this._JaquettePath != value)
                {
                    this._JaquettePath = value;
                    this.OnPropertyChanged();
                }
            }
        }

        protected long _NbExemplaires;
        public long NbExemplaires
        {
            get => _NbExemplaires;
            set
            {
                if (_NbExemplaires != value)
                {
                    _NbExemplaires = value;
                    OnPropertyChanged();
                }
            }
        }

        protected long _NbPrets;
        public long NbPrets
        {
            get => _NbPrets;
            set
            {
                if (_NbPrets != value)
                {
                    _NbPrets = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Format
        protected string? _Format;
        public string? Format
        {
            get => _Format;
            set
            {
                if (_Format != value)
                {
                    _Format = value;
                    OnPropertyChanged();
                }
            }
        }

        protected short? _NbOfPages = 0;
        public short? NbOfPages
        {
            get => _NbOfPages;
            set
            {
                if (_NbOfPages != value)
                {
                    _NbOfPages = value;
                    OnPropertyChanged();
                }
            }
        }

        protected double? _Hauteur;
        public double? Hauteur
        {
            get => _Hauteur;
            set
            {
                if (_Hauteur != value)
                {
                    _Hauteur = value;
                    OnPropertyChanged();
                }
            }
        }

        protected double? _Largeur;
        public double? Largeur
        {
            get => _Largeur;
            set
            {
                if (_Largeur != value)
                {
                    _Largeur = value;
                    OnPropertyChanged();
                }
            }
        }

        protected double? _Epaisseur;
        public double? Epaisseur
        {
            get => _Epaisseur;
            set
            {
                if (_Epaisseur != value)
                {
                    _Epaisseur = value;
                    OnPropertyChanged();
                }
            }
        }

        protected double? _Poids;
        public double? Poids
        {
            get => _Poids;
            set
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
        protected int? _DayParution;
        public int? DayParution
        {
            get => this._DayParution;
            set
            {
                if (this._DayParution != value)
                {
                    this._DayParution = value;
                    this.OnPropertyChanged();
                }
            }
        }

        protected int? _MonthParution;
        public int? MonthParution
        {
            get => this._MonthParution;
            set
            {
                if (this._MonthParution != value)
                {
                    this._MonthParution = value;
                    this.OnPropertyChanged();
                }
            }
        }

        protected int? _YearParution;
        public int? YearParution
        {
            get => this._YearParution;
            set
            {
                if (this._YearParution != value)
                {
                    this._YearParution = value;
                    this.OnPropertyChanged();
                }
            }
        }

        protected string? _DateParution;
        public string? DateParution
        {
            get => _DateParution;
            set
            {
                if (_DateParution != value)
                {
                    _DateParution = value;
                    OnPropertyChanged();
                }
            }
        }

        protected ObservableCollection<ContactVM> _Editeurs = new ObservableCollection<ContactVM>();
        public ObservableCollection<ContactVM> Editeurs
        {
            get => _Editeurs;
            set
            {
                if (_Editeurs != value)
                {
                    _Editeurs = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _Pays;
        public string? Pays
        {
            get => _Pays;
            set
            {
                if (_Pays != value)
                {
                    _Pays = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _Langue;
        public string? Langue
        {
            get => _Langue;
            set
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
        protected string? _Resume;

        [DisplayName("Résumé")]
        public string? Resume
        {
            get => _Resume;
            set
            {
                if (_Resume != value)
                {
                    _Resume = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _Notes;
        public string? Notes
        {
            get => _Notes;
            set
            {
                if (_Notes != value)
                {
                    _Notes = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public string Dimensions => $"{Hauteur?.ToString() ?? "?"} cm × {Largeur?.ToString() ?? "?"} cm × {Epaisseur?.ToString() ?? "?"} cm";

        //public LivreDescriptionVM Description { get; set; } = new LivreDescriptionVM();

        protected BookIdentificationVM _Identification = new ();
        //public BookIdentificationVM Identification { get; set; } = new LivreIdentificationVM();
        public BookIdentificationVM Identification
        {
            get => _Identification;
            set
            {
                if (_Identification != value)
                {
                    _Identification = value;
                    OnPropertyChanged();
                }
            }
        }
        //public LivreFormatVM Format { get; set; } = new LivreFormatVM();
        //public LivrePublicationVM Publication { get; set; } = new LivrePublicationVM();
    }
}
