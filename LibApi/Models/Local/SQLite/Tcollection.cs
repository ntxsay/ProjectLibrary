using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class Tcollection
    {
        public Tcollection()
        {
            TbookCollections = new HashSet<TbookCollection>();
        }

        public long Id { get; set; }
        public long IdLibrary { get; set; }
        public long CollectionType { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual Tlibrary IdLibraryNavigation { get; set; } = null!;
        public virtual ICollection<TbookCollection> TbookCollections { get; set; }
    }
}
