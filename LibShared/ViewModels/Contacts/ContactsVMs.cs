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
        [JsonIgnore]
        [Obsolete($"La propriété {nameof(Guid)} n'est pas supportée dans {nameof(ContactRoleVM)} et lèvera une {nameof(NotSupportedException)} s'il y a usage.", true)]
        public override Guid Guid
        {
            get => throw new NotSupportedException($"La propriété {nameof(Guid)} n'est pas supportée dans {nameof(ContactRoleVM)}.");
            protected set => throw new NotSupportedException($"La propriété {nameof(Guid)} n'est pas supportée dans {nameof(ContactRoleVM)}.");
        }

        [JsonIgnore]
        [Obsolete($"La propriété {nameof(DateAjout)} n'est pas supportée dans {nameof(ContactRoleVM)} et lèvera une {nameof(NotSupportedException)} s'il y a usage.", true)]
        public override DateTime DateAjout
        {
            get => throw new NotSupportedException($"La propriété {nameof(DateAjout)} n'est pas supportée dans {nameof(ContactRoleVM)}.");
            protected set => throw new NotSupportedException($"La propriété {nameof(DateAjout)} n'est pas supportée dans {nameof(ContactRoleVM)}.");
        }

        [JsonIgnore]
        [Obsolete($"La propriété {nameof(DateEdition)} n'est pas supportée dans {nameof(ContactRoleVM)} et lèvera une {nameof(NotSupportedException)} s'il y a usage.", true)]
        public override DateTime? DateEdition
        {
            get => throw new NotSupportedException($"La propriété {nameof(DateEdition)} n'est pas supportée dans {nameof(ContactRoleVM)}.");
            protected set => throw new NotSupportedException($"La propriété {nameof(DateEdition)} n'est pas supportée dans {nameof(ContactRoleVM)}.");
        }

    }

    public class ContactTypeVM : GenericVM
    {
        [JsonIgnore]
        [Obsolete($"La propriété {nameof(Guid)} n'est pas supportée dans {nameof(ContactTypeVM)} et lèvera une {nameof(NotSupportedException)} s'il y a usage.", true)]
        public override Guid Guid
        {
            get => throw new NotSupportedException($"La propriété {nameof(Guid)} n'est pas supportée dans {nameof(ContactTypeVM)}.");
            protected set => throw new NotSupportedException($"La propriété {nameof(Guid)} n'est pas supportée dans {nameof(ContactTypeVM)}.");
        }

        [JsonIgnore]
        [Obsolete($"La propriété {nameof(DateAjout)} n'est pas supportée dans {nameof(ContactTypeVM)} et lèvera une {nameof(NotSupportedException)} s'il y a usage.", true)]
        public override DateTime DateAjout
        {
            get => throw new NotSupportedException($"La propriété {nameof(DateAjout)} n'est pas supportée dans {nameof(ContactTypeVM)}.");
            protected set => throw new NotSupportedException($"La propriété {nameof(DateAjout)} n'est pas supportée dans {nameof(ContactTypeVM)}.");
        }

        [JsonIgnore]
        [Obsolete($"La propriété {nameof(DateEdition)} n'est pas supportée dans {nameof(ContactTypeVM)} et lèvera une {nameof(NotSupportedException)} s'il y a usage.", true)]
        public override DateTime? DateEdition
        {
            get => throw new NotSupportedException($"La propriété {nameof(DateEdition)} n'est pas supportée dans {nameof(ContactTypeVM)}.");
            protected set => throw new NotSupportedException($"La propriété {nameof(DateEdition)} n'est pas supportée dans {nameof(ContactTypeVM)}.");
        }

    }

    public class ContactVM : GenericVM
    {

        private ContactType _ContactType = ContactType.Human;
        public ContactType ContactType
        {
            get => _ContactType;
            protected set
            {
                if (_ContactType != value)
                {
                    _ContactType = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ContactRoleVM> _ContactRoles = new ObservableCollection<ContactRoleVM>();
        public ObservableCollection<ContactRoleVM> ContactRoles
        {
            get => _ContactRoles;
            protected set
            {
                if (_ContactRoles != value)
                {
                    _ContactRoles = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _TitreCivilite;
        public string? TitreCivilite
        {
            get => _TitreCivilite;
            protected set
            {
                if (_TitreCivilite != value)
                {
                    _TitreCivilite = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _NomNaissance;
        public string? NomNaissance
        {
            get => _NomNaissance;
            protected set
            {
                if (_NomNaissance != value)
                {
                    _NomNaissance = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _NomUsage = String.Empty;
        public string? NomUsage
        {
            get => _NomUsage;
            protected set
            {
                if (_NomUsage != value)
                {
                    _NomUsage = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _Prenom;
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

        private string? _AutresPrenoms = String.Empty;
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

        private string? _AdressePostal;
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

        private string? _Ville;
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

        private string? _CodePostal;
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

        private string? _AdresseMail;
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

        private string? _NoTelephone;
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

        private string? _NoMobile;
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

        private string? _Observation;
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

        private string? _SocietyName;
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

        private string? _Nationality;
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

        private string? _JaquettePath;
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
        
    }
}
