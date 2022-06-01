using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TbookCollection
    {
        public long Id { get; set; }
        public long IdBook { get; set; }
        public long IdCollection { get; set; }

        public virtual Tbook IdBookNavigation { get; set; } = null!;
        public virtual Tcollection IdCollectionNavigation { get; set; } = null!;
    }
}
