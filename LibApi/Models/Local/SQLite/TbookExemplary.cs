using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TbookExemplary
    {
        public TbookExemplary()
        {
            TbookEtats = new HashSet<TbookEtat>();
            TbookPrets = new HashSet<TbookPret>();
        }

        public long Id { get; set; }
        public long IdBook { get; set; }
        public long? IdContactSource { get; set; }
        public string? NoGroup { get; set; }
        public long NoExemplary { get; set; }
        public string DateAjout { get; set; } = null!;
        public string? DateEdition { get; set; }
        public string TypeAcquisition { get; set; } = null!;
        public double? Price { get; set; }
        public string? DeviceName { get; set; }
        public string? DateAcquisition { get; set; }
        public string? DateRemise { get; set; }
        public long IsVisible { get; set; }
        public string? Observations { get; set; }

        public virtual Tbook IdBookNavigation { get; set; } = null!;
        public virtual Tcontact? IdContactSourceNavigation { get; set; }
        public virtual ICollection<TbookEtat> TbookEtats { get; set; }
        public virtual ICollection<TbookPret> TbookPrets { get; set; }
    }
}
