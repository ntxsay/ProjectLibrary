using System;
using System.Collections.Generic;

namespace LibApi.Models.Local.SQLite
{
    public partial class TbookIdentification
    {
        public long Id { get; set; }
        public string? Isbn { get; set; }
        public string? Isbn10 { get; set; }
        public string? Isbn13 { get; set; }
        public string? Issn { get; set; }
        public string? Asin { get; set; }
        public string? Cotation { get; set; }
        public string? CodeBarre { get; set; }

        public virtual Tbook IdNavigation { get; set; } = null!;
    }
}
