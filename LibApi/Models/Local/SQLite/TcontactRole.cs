using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TcontactRole
    {
        public long Id { get; set; }
        public long IdContact { get; set; }
        public long Role { get; set; }

        public virtual Tcontact IdContactNavigation { get; set; } = null!;
    }
}
