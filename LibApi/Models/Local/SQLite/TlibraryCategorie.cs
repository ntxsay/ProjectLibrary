using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TlibraryCategorie
    {
        public TlibraryCategorie()
        {
            InverseIdParentCategorieNavigation = new HashSet<TlibraryCategorie>();
            Tbooks = new HashSet<Tbook>();
        }

        public long Id { get; set; }
        public long IdLibrary { get; set; }
        public long? IdParentCategorie { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual Tlibrary IdLibraryNavigation { get; set; } = null!;
        public virtual TlibraryCategorie? IdParentCategorieNavigation { get; set; }
        public virtual ICollection<TlibraryCategorie> InverseIdParentCategorieNavigation { get; set; }
        public virtual ICollection<Tbook> Tbooks { get; set; }
    }
}
