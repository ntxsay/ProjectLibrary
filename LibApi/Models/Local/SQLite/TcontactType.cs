using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TcontactType
    {
        public TcontactType()
        {
            Tcontacts = new HashSet<Tcontact>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<Tcontact> Tcontacts { get; set; }
    }
}
