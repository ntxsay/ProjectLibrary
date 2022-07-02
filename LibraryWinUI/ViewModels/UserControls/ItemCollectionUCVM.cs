using LibraryWinUI.ViewModels.Libraries;
using LibShared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.ViewModels.UserControls
{
    internal class ItemCollectionUCVM  : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private IEnumerable<LibraryVM> _DataList;
        public IEnumerable<LibraryVM> DataList
        {
            get => this._DataList;
            set
            {
                if (this._DataList != value)
                {
                    this._DataList = value;
                    this.OnPropertyChanged();
                }
            }
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



}
