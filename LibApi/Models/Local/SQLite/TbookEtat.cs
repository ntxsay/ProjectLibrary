using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TbookEtat
    {
        public TbookEtat()
        {
            TbookPretIdEtatAfterNavigations = new HashSet<TbookPret>();
            TbookPretIdEtatBeforeNavigations = new HashSet<TbookPret>();
        }

        public long Id { get; set; }
        public long IdBookExemplary { get; set; }
        public string DateAjout { get; set; } = null!;
        public long TypeVerification { get; set; }
        public string Etat { get; set; } = null!;
        public string? Observations { get; set; }

        public virtual TbookExemplary IdBookExemplaryNavigation { get; set; } = null!;
        public virtual ICollection<TbookPret> TbookPretIdEtatAfterNavigations { get; set; }
        public virtual ICollection<TbookPret> TbookPretIdEtatBeforeNavigations { get; set; }
    }
}
