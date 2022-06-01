using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class Tbook
    {
        public Tbook()
        {
            TbookAuthorConnectors = new HashSet<TbookAuthorConnector>();
            TbookCollections = new HashSet<TbookCollection>();
            TbookEditeurConnectors = new HashSet<TbookEditeurConnector>();
            TbookExemplaries = new HashSet<TbookExemplary>();
            TbookIllustratorConnectors = new HashSet<TbookIllustratorConnector>();
            TbookOtherTitles = new HashSet<TbookOtherTitle>();
            TbookTranslatorConnectors = new HashSet<TbookTranslatorConnector>();
        }

        public long Id { get; set; }
        public long IdLibrary { get; set; }
        public long? IdCategorie { get; set; }
        public long? IdSubCategorie { get; set; }
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
        public virtual TlibrarySubCategorie? IdSubCategorieNavigation { get; set; }
        public virtual TbookClassification TbookClassification { get; set; } = null!;
        public virtual TbookFormat TbookFormat { get; set; } = null!;
        public virtual TbookIdentification TbookIdentification { get; set; } = null!;
        public virtual TbookReading TbookReading { get; set; } = null!;
        public virtual ICollection<TbookAuthorConnector> TbookAuthorConnectors { get; set; }
        public virtual ICollection<TbookCollection> TbookCollections { get; set; }
        public virtual ICollection<TbookEditeurConnector> TbookEditeurConnectors { get; set; }
        public virtual ICollection<TbookExemplary> TbookExemplaries { get; set; }
        public virtual ICollection<TbookIllustratorConnector> TbookIllustratorConnectors { get; set; }
        public virtual ICollection<TbookOtherTitle> TbookOtherTitles { get; set; }
        public virtual ICollection<TbookTranslatorConnector> TbookTranslatorConnectors { get; set; }
    }
}
