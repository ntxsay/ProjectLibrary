using LibraryWinUI.ViewModels.Libraries;
using LibShared.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.ViewModels.UserControls
{
    internal class ItemCollectionUCVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private int _SelectedPivotIndex;
        public int SelectedPivotIndex
        {
            get => this._SelectedPivotIndex;
            set
            {
                if (_SelectedPivotIndex != value)
                {
                    this._SelectedPivotIndex = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _CurrentPage;
        public int CurrentPage
        {
            get => this._CurrentPage;
            set
            {
                if (_CurrentPage != value)
                {
                    this._CurrentPage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _TotalPages;
        public int TotalPages
        {
            get => this._TotalPages;
            set
            {
                if (_TotalPages != value)
                {
                    this._TotalPages = value;
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
