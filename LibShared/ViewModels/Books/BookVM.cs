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
        public long? IdLibrary { get; set; }
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

        protected ObservableCollection<string> _TitresOeuvre = new ObservableCollection<string>();

        [DisplayName("Autre(s) titre(s)")]
        public ObservableCollection<string> TitresOeuvre
        {
            get => _TitresOeuvre;
            set
            {
                if (_TitresOeuvre != value)
                {
                    _TitresOeuvre = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _TitresOeuvreStringList;
        [JsonIgnore]
        public string? TitresOeuvreStringList
        {
            get => _TitresOeuvreStringList;
            set
            {
                if (_TitresOeuvreStringList != value)
                {
                    _TitresOeuvreStringList = value;
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

        protected string? _AuteursStringList;
        [JsonIgnore]
        public string? AuteursStringList
        {
            get => _AuteursStringList;
            set
            {
                if (_AuteursStringList != value)
                {
                    _AuteursStringList = value;
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

        //private ObservableCollection<CategorieLivreVM> _Categories = new ObservableCollection<CategorieLivreVM>();
        //public ObservableCollection<CategorieLivreVM> Categories
        //{
        //    get => _Categories;
        //    set
        //    {
        //        if (_Categories != value)
        //        {
        //            _Categories = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

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

        #region Publication
        protected string? _DayParution;
        public string? DayParution
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

        protected string? _MonthParution;
        public string? MonthParution
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

        protected string? _YearParution;
        public string? YearParution
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

        protected ObservableCollection<CollectionVM> _Collections = new ObservableCollection<CollectionVM>();
        public ObservableCollection<CollectionVM> Collections
        {
            get => _Collections;
            set
            {
                if (_Collections != value)
                {
                    _Collections = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _CollectionsStringList;
        [JsonIgnore]
        public string? CollectionsStringList
        {
            get => _CollectionsStringList;
            set
            {
                if (_CollectionsStringList != value)
                {
                    _CollectionsStringList = value;
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

        protected string? _EditeursStringList;
        [JsonIgnore]
        public string? EditeursStringList
        {
            get => _EditeursStringList;
            set
            {
                if (_EditeursStringList != value)
                {
                    _EditeursStringList = value;
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


        //public LivreDescriptionVM Description { get; set; } = new LivreDescriptionVM();
        //public LivreClassificationAgeVM ClassificationAge { get; set; } = new LivreClassificationAgeVM();

        //public LivreIdentificationVM Identification { get; set; } = new LivreIdentificationVM();
        //public LivreFormatVM Format { get; set; } = new LivreFormatVM();
        //public LivrePublicationVM Publication { get; set; } = new LivrePublicationVM();
    }
}
