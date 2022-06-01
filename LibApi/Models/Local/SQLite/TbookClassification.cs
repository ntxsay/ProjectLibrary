using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TbookClassification
    {
        public long Id { get; set; }
        public long TypeClassification { get; set; }
        public long ApartirDe { get; set; }
        public long Jusqua { get; set; }
        public long DeTelAge { get; set; }
        public long AtelAge { get; set; }

        public virtual Tbook IdNavigation { get; set; } = null!;
    }
}
