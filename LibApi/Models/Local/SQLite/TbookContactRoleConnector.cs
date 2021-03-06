using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TbookContactRoleConnector
    {
        public long Id { get; set; }
        public long IdBook { get; set; }
        public long IdContact { get; set; }
        public long Role { get; set; }

        public virtual Tbook IdBookNavigation { get; set; } = null!;
        public virtual Tcontact IdContactNavigation { get; set; } = null!;
    }
}
