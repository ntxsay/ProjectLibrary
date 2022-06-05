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
        private short _MaxItemsPerPage = 100;

        [Obsolete]
        public short MaxItemsPerPage
        {
            get => this._MaxItemsPerPage;
            set
            {
                if (_MaxItemsPerPage != value)
                {
                    this._MaxItemsPerPage = value;
                    this.OnPropertyChanged();
                }
            }
        }
    }
}
