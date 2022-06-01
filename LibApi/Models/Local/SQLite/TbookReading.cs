using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TbookReading
    {
        public long Id { get; set; }
        public long? Status { get; set; }
        public long? LastPageReaded { get; set; }
        public string? LastDateReaded { get; set; }
        public double? Note10 { get; set; }

        public virtual Tbook IdNavigation { get; set; } = null!;
    }
}
