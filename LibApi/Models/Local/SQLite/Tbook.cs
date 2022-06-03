using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class Tbook
    {
        public Tbook()
        {
            TbookCollections = new HashSet<TbookCollection>();
            TbookContactRoleConnectors = new HashSet<TbookContactRoleConnector>();
            TbookExemplaries = new HashSet<TbookExemplary>();
            TbookOtherTitles = new HashSet<TbookOtherTitle>();
        }

        public long Id { get; set; }
        public long IdLibrary { get; set; }
        public long? IdCategorie { get; set; }
        public string Guid { get; set; } = null!;
        public string DateAjout { get; set; } = null!;
        public string? DateEdition { get; set; }
        public string MainTitle { get; set; } = null!;
        public long CountOpening { get; set; }
        public string? DateParution { get; set; }
        public string? Resume { get; set; }
        public string? Notes { get; set; }
        public string? Langue { get; set; }
        public string? Pays { get; set; }
        public string? PhysicalLocation { get; set; }

        public virtual TlibraryCategorie? IdCategorieNavigation { get; set; }
        public virtual Tlibrary IdLibraryNavigation { get; set; } = null!;
        public virtual TbookClassification TbookClassification { get; set; } = null!;
        public virtual TbookFormat TbookFormat { get; set; } = null!;
        public virtual TbookIdentification TbookIdentification { get; set; } = null!;
        public virtual TbookReading TbookReading { get; set; } = null!;
        public virtual ICollection<TbookCollection> TbookCollections { get; set; }
        public virtual ICollection<TbookContactRoleConnector> TbookContactRoleConnectors { get; set; }
        public virtual ICollection<TbookExemplary> TbookExemplaries { get; set; }
        public virtual ICollection<TbookOtherTitle> TbookOtherTitles { get; set; }
    }
}
