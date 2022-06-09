using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibShared.ViewModels.Contacts
{
    public class ContactRoleVM : GenericVM
    {
        

    }

    public class ContactTypeVM : GenericVM
    {
        

    }

    public class ContactVM : GenericVM
    {
        public virtual Guid Guid { get; protected set; } = Guid.NewGuid();

        public virtual DateTime DateAjout { get; protected set; } = DateTime.Now;

        public virtual DateTime? DateEdition { get; protected set; }

        protected ContactType _ContactType = ContactType.Human;
        public ContactType ContactType
        {
            get => _ContactType;
            set
            {
                if (_ContactType != value)
                {
                    _ContactType = value;
                    OnPropertyChanged();
                }
            }
        }

        protected ObservableCollection<ContactRoleVM> _ContactRoles = new ObservableCollection<ContactRoleVM>();
        public ObservableCollection<ContactRoleVM> ContactRoles
        {
            get => _ContactRoles;
            set
            {
                if (_ContactRoles != value)
                {
                    _ContactRoles = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _TitreCivilite;
        public string? TitreCivilite
        {
            get => _TitreCivilite;
            set
            {
                if (_TitreCivilite != value)
                {
                    _TitreCivilite = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _NomNaissance;
        public string? NomNaissance
        {
            get => _NomNaissance;
            set
            {
                if (_NomNaissance != value)
                {
                    _NomNaissance = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _NomUsage = String.Empty;
        public string? NomUsage
        {
            get => _NomUsage;
            set
            {
                if (_NomUsage != value)
                {
                    _NomUsage = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _Prenom;
        public string? Prenom
        {
            get => _Prenom;
            set
            {
                if (_Prenom != value)
                {
                    _Prenom = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _AutresPrenoms = String.Empty;
        public string? AutresPrenoms
        {
            get => _AutresPrenoms;
            set
            {
                if (_AutresPrenoms != value)
                {
                    _AutresPrenoms = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _AdressePostal;
        public string? AdressePostal
        {
            get => _AdressePostal;
            set
            {
                if (_AdressePostal != value)
                {
                    _AdressePostal = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _Ville;
        public string? Ville
        {
            get => _Ville;
            set
            {
                if (_Ville != value)
                {
                    _Ville = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _CodePostal;
        public string? CodePostal
        {
            get => _CodePostal;
            set
            {
                if (_CodePostal != value)
                {
                    _CodePostal = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _AdresseMail;
        public string? AdresseMail
        {
            get => _AdresseMail;
            set
            {
                if (_AdresseMail != value)
                {
                    _AdresseMail = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _NoTelephone;
        public string? NoTelephone
        {
            get => _NoTelephone;
            set
            {
                if (_NoTelephone != value)
                {
                    _NoTelephone = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _NoMobile;
        public string? NoMobile
        {
            get => _NoMobile;
            set
            {
                if (_NoMobile != value)
                {
                    _NoMobile = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _Observation;
        public string? Observation
        {
            get => _Observation;
            set
            {
                if (_Observation != value)
                {
                    _Observation = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _SocietyName;
        public string? SocietyName
        {
            get => _SocietyName;
            set
            {
                if (_SocietyName != value)
                {
                    _SocietyName = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _Nationality;
        public string? Nationality
        {
            get => _Nationality;
            set
            {
                if (_Nationality != value)
                {
                    _Nationality = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTimeOffset? _DateNaissance;
        public DateTimeOffset? DateNaissance
        {
            get => _DateNaissance;
            set
            {
                if (_DateNaissance != value)
                {
                    _DateNaissance = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _JaquettePath;
        [JsonIgnore]
        public string? JaquettePath
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

        
    }
}
