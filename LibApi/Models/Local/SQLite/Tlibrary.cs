using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class Tlibrary
    {
        public Tlibrary()
        {
            Tbooks = new HashSet<Tbook>();
            Tcollections = new HashSet<Tcollection>();
            TlibraryCategories = new HashSet<TlibraryCategorie>();
        }

        public long Id { get; set; }
        public string Guid { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string DateAjout { get; set; } = null!;
        public string? DateEdition { get; set; }

        public virtual ICollection<Tbook> Tbooks { get; set; }
        public virtual ICollection<Tcollection> Tcollections { get; set; }
        public virtual ICollection<TlibraryCategorie> TlibraryCategories { get; set; }
    }
}
