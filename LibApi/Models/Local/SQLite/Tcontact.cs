using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class Tcontact
    {
        public Tcontact()
        {
            TbookAuthorConnectors = new HashSet<TbookAuthorConnector>();
            TbookEditeurConnectors = new HashSet<TbookEditeurConnector>();
            TbookExemplaries = new HashSet<TbookExemplary>();
            TbookIllustratorConnectors = new HashSet<TbookIllustratorConnector>();
            TbookPrets = new HashSet<TbookPret>();
            TbookTranslatorConnectors = new HashSet<TbookTranslatorConnector>();
            TcontactRoles = new HashSet<TcontactRole>();
        }

        public long Id { get; set; }
        public long Type { get; set; }
        public string Guid { get; set; } = null!;
        public string DateAjout { get; set; } = null!;
        public string? DateEdition { get; set; }
        public string? SocietyName { get; set; }
        public string? TitreCivilite { get; set; }
        public string? NomNaissance { get; set; }
        public string? NomUsage { get; set; }
        public string? Prenom { get; set; }
        public string? AutresPrenoms { get; set; }
        public string? DateNaissance { get; set; }
        public string? DateDeces { get; set; }
        public string? LieuNaissance { get; set; }
        public string? LieuDeces { get; set; }
        public string? Nationality { get; set; }
        public string? AdressPostal { get; set; }
        public string? Ville { get; set; }
        public string? CodePostal { get; set; }
        public string? MailAdress { get; set; }
        public string? NoTelephone { get; set; }
        public string? NoMobile { get; set; }
        public string? Observation { get; set; }
        public string? Biographie { get; set; }

        public virtual ICollection<TbookAuthorConnector> TbookAuthorConnectors { get; set; }
        public virtual ICollection<TbookEditeurConnector> TbookEditeurConnectors { get; set; }
        public virtual ICollection<TbookExemplary> TbookExemplaries { get; set; }
        public virtual ICollection<TbookIllustratorConnector> TbookIllustratorConnectors { get; set; }
        public virtual ICollection<TbookPret> TbookPrets { get; set; }
        public virtual ICollection<TbookTranslatorConnector> TbookTranslatorConnectors { get; set; }
        public virtual ICollection<TcontactRole> TcontactRoles { get; set; }
    }
}
