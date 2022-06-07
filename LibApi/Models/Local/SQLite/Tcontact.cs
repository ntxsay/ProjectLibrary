using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class Tcontact
    {
        public Tcontact()
        {
            TbookContactRoleConnectors = new HashSet<TbookContactRoleConnector>();
            TbookExemplaries = new HashSet<TbookExemplary>();
            TbookPrets = new HashSet<TbookPret>();
        }

        public long Id { get; set; }
        public long ContactType { get; set; }
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
        public string? Nationality { get; set; }
        public string? AdressPostal { get; set; }
        public string? Ville { get; set; }
        public string? CodePostal { get; set; }
        public string? MailAdress { get; set; }
        public string? NoTelephone { get; set; }
        public string? NoMobile { get; set; }
        public string? Observation { get; set; }

        public virtual ICollection<TbookContactRoleConnector> TbookContactRoleConnectors { get; set; }
        public virtual ICollection<TbookExemplary> TbookExemplaries { get; set; }
        public virtual ICollection<TbookPret> TbookPrets { get; set; }
    }
}
