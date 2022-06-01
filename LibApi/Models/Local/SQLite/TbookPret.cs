using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TbookPret
    {
        public long Id { get; set; }
        public long IdBookExemplary { get; set; }
        public long IdContact { get; set; }
        public long IdEtatBefore { get; set; }
        public long? IdEtatAfter { get; set; }
        public string DatePret { get; set; } = null!;
        public string? TimePret { get; set; }
        public string? DateRemise { get; set; }
        public string? TimeRemise { get; set; }
        public string? DateRemiseUser { get; set; }
        public string? Observation { get; set; }

        public virtual TbookExemplary IdBookExemplaryNavigation { get; set; } = null!;
        public virtual Tcontact IdContactNavigation { get; set; } = null!;
        public virtual TbookEtat? IdEtatAfterNavigation { get; set; }
        public virtual TbookEtat IdEtatBeforeNavigation { get; set; } = null!;
    }
}
