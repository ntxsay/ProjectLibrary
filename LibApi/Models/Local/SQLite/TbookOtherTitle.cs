using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TbookOtherTitle
    {
        public long Id { get; set; }
        public long IdBook { get; set; }
        public string Title { get; set; } = null!;

        public virtual Tbook IdBookNavigation { get; set; } = null!;
    }
}
