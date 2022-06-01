using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TlibraryCategorie
    {
        public TlibraryCategorie()
        {
            Tbooks = new HashSet<Tbook>();
            TlibrarySubCategories = new HashSet<TlibrarySubCategorie>();
        }

        public long Id { get; set; }
        public long IdLibrary { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual Tlibrary IdLibraryNavigation { get; set; } = null!;
        public virtual ICollection<Tbook> Tbooks { get; set; }
        public virtual ICollection<TlibrarySubCategorie> TlibrarySubCategories { get; set; }
    }
}
