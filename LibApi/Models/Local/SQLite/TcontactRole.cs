﻿using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TcontactRole
    {
        public TcontactRole()
        {
            TbookContactRoleConnectors = new HashSet<TbookContactRoleConnector>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<TbookContactRoleConnector> TbookContactRoleConnectors { get; set; }
    }
}
