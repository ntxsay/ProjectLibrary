﻿using LibShared.ViewModels.Contacts;
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
    public class BookVM : GenericVM
    {
        [JsonIgnore]
        public long? IdLibrary { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public long CountOpening { get; set; }

        private string _MainTitle;

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

        private ObservableCollection<string> _TitresOeuvre = new ObservableCollection<string>();

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

        private string _TitresOeuvreStringList;
        [JsonIgnore]
        public string TitresOeuvreStringList
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

        private ObservableCollection<ContactVM> _Auteurs = new ObservableCollection<ContactVM>();
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

        private string _AuteursStringList;
        [JsonIgnore]
        public string AuteursStringList
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

        private DateTime _DateAjout = DateTime.UtcNow;
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

        private DateTime? _DateEdition;
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

        private string _JaquettePath = String.Empty;

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

        private long _NbExemplaires;
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

        private long _NbPrets;
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

        //public LivreDescriptionVM Description { get; set; } = new LivreDescriptionVM();
        //public LivreClassificationAgeVM ClassificationAge { get; set; } = new LivreClassificationAgeVM();

        //public LivreIdentificationVM Identification { get; set; } = new LivreIdentificationVM();
        //public LivreFormatVM Format { get; set; } = new LivreFormatVM();
        //public LivrePublicationVM Publication { get; set; } = new LivrePublicationVM();
    }
}
