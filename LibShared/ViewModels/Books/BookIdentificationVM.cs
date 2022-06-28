using System;
namespace LibShared.ViewModels.Books
{
    public class BookIdentificationVM : GenericVM
    {
        private new long Id
        {
            get => _Id;
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Identification
        protected string? _Cotation;
        public string? Cotation
        {
            get => _Cotation;
            set
            {
                if (_Cotation != value)
                {
                    _Cotation = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _ASIN;
        /// <summary>
        /// L'ASIN est un numéro international (amazon) normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string? ASIN
        {
            get => _ASIN;
            set
            {
                if (_ASIN != value)
                {
                    _ASIN = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _ISSN;
        /// <summary>
        /// L'ISSN est un numéro international  normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string? ISSN
        {
            get => _ISSN;
            set
            {
                if (_ISSN != value)
                {
                    _ISSN = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _ISBN;
        /// <summary>
        /// L'ISBN est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string? ISBN
        {
            get => _ISBN;
            set
            {
                if (_ISBN != value)
                {
                    _ISBN = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _ISBN10;
        /// <summary>
        /// L'ISBN-10 est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string? ISBN10
        {
            get => _ISBN10;
            set
            {
                if (_ISBN10 != value)
                {
                    _ISBN10 = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _ISBN13;
        /// <summary>
        /// L'ISBN-13 est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string? ISBN13
        {
            get => _ISBN13;
            set
            {
                if (_ISBN13 != value)
                {
                    _ISBN13 = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _CodeBarre;
        /// <summary>
        /// L'ISBN est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public string? CodeBarre
        {
            get => _CodeBarre;
            set
            {
                if (_CodeBarre != value)
                {
                    _CodeBarre = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

    }
}

