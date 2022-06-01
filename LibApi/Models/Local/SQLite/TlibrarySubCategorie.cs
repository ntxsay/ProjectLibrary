using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TlibrarySubCategorie
    {
        public TlibrarySubCategorie()
        {
            Tbooks = new HashSet<Tbook>();
        }

        public long Id { get; set; }
        public long IdCategorie { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual TlibraryCategorie IdCategorieNavigation { get; set; } = null!;
        public virtual ICollection<Tbook> Tbooks { get; set; }
    }
}
