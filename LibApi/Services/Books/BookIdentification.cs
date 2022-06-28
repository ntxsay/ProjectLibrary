using System;
using LibShared.ViewModels.Books;

namespace LibApi.Services.Books
{
    public class BookIdentification : BookIdentificationVM
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
        public new string? Cotation
        {
            get => _Cotation;
            internal set
            {
                if (_Cotation != value)
                {
                    _Cotation = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// L'ASIN est un numéro international (amazon) normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public new string? ASIN
        {
            get => _ASIN;
            internal set
            {
                if (_ASIN != value)
                {
                    _ASIN = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// L'ISSN est un numéro international  normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public new string? ISSN
        {
            get => _ISSN;
            internal set
            {
                if (_ISSN != value)
                {
                    _ISSN = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// L'ISBN est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public new string? ISBN
        {
            get => _ISBN;
            internal set
            {
                if (_ISBN != value)
                {
                    _ISBN = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// L'ISBN-10 est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public new string? ISBN10
        {
            get => _ISBN10;
            internal set
            {
                if (_ISBN10 != value)
                {
                    _ISBN10 = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// L'ISBN-13 est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public new string? ISBN13
        {
            get => _ISBN13;
            internal set
            {
                if (_ISBN13 != value)
                {
                    _ISBN13 = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// L'ISBN est un numéro international normalisé permettant l'identification d'un livre dans une édition donnée.
        /// </summary>
        public new string? CodeBarre
        {
            get => _CodeBarre;
            internal set
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

