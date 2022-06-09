using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibShared.ViewModels.Libraries
{
    public class LibraryVM : GenericVM
    {

        public virtual Guid Guid { get; protected set; } = Guid.NewGuid();

        public virtual DateTime DateAjout { get; protected set; } = DateTime.Now;

        public virtual DateTime? DateEdition { get; protected set; }

    }
}
