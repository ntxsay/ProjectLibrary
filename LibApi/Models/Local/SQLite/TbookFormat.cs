using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TbookFormat
    {
        public long Id { get; set; }
        public string? Format { get; set; }
        public long? NbOfPages { get; set; }
        public double? Largeur { get; set; }
        public double? Hauteur { get; set; }
        public double? Epaisseur { get; set; }
        public double? Weight { get; set; }

        public virtual Tbook IdNavigation { get; set; } = null!;
    }
}
