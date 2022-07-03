using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibShared.ViewModels.Libraries
{
    public class LibraryRequestVM
    {
        public IEnumerable<LibraryVM>? List { get; set; }
        public int CurrentPage { get; set; }
        public int NbPages { get; set; }
    }
}
