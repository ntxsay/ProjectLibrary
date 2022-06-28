using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibShared.ViewModels.Books
{
    public class BookClassificationAgeVM : GenericVM
    {
        private ClassificationAge _TypeClassification = ClassificationAge.ToutPublic;
        public ClassificationAge TypeClassification
        {
            get => _TypeClassification;
            set
            {
                if (_TypeClassification != value)
                {
                    _TypeClassification = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte _MinimumAge;
        /// <summary>
        /// A partir de tel age
        /// </summary>
        public byte MinimumAge
        {
            get => _MinimumAge;
            set
            {
                if (_MinimumAge != value)
                {
                    _MinimumAge = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte _MaximumAge;
        /// <summary>
        /// Jusqu'à tel age
        /// </summary>
        public byte MaximumAge
        {
            get => _MaximumAge;
            set
            {
                if (_MaximumAge != value)
                {
                    _MaximumAge = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _StringClassification;
        public string? StringClassification
        {
            get => _StringClassification;
            set
            {
                if (_StringClassification != value)
                {
                    _StringClassification = value;
                    OnPropertyChanged();
                }
            }
        }

        public string GetClassificationAge()
        {
            try
            {
                string result = string.Empty;
                switch (this.TypeClassification)
                {
                    case ClassificationAge.ToutPublic:
                        result = "Tout public";
                        break;
                    case ClassificationAge.ApartirDe:
                        result = $"A partir de {this.MinimumAge} ans";
                        break;
                    case ClassificationAge.Jusqua:
                        result = $"Jusqu'à {this.MaximumAge} ans";
                        break;
                    case ClassificationAge.DeTantATant:
                        if (this.MinimumAge == this.MaximumAge)
                        {
                            result = $"{this.MinimumAge} ans uniquement";
                        }
                        else
                        {
                            result = $"De {this.MinimumAge} à {this.MaximumAge} ans";
                        }
                        break;
                    default:
                        break;
                }

                this.StringClassification = result;
                return result;
            }
            catch (Exception)
            {
                this.StringClassification = string.Empty;
                return string.Empty;
            }
        }
    }
}
