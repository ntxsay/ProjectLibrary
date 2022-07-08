using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.ViewModels.Pages
{

    public class MainCollectionPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public MainCollectionPageVM()
        {

        }

        private ObservableCollection<SideBarItemHeaderVM> _ItemsSideBarHeader = new ();
        public ObservableCollection<SideBarItemHeaderVM> ItemsSideBarHeader
        {
            get => this._ItemsSideBarHeader;
            set
            {
                if (_ItemsSideBarHeader != value)
                {
                    this._ItemsSideBarHeader = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #region SplitView
        private bool _IsSplitViewOpen = false;
        public bool IsSplitViewOpen
        {
            get => this._IsSplitViewOpen;
            set
            {
                if (_IsSplitViewOpen != value)
                {
                    this._IsSplitViewOpen = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public const double MinSplitViewWidth = 400;

        private double _SplitViewWidth = MinSplitViewWidth;
        public double SplitViewWidth
        {
            get => this._SplitViewWidth;
            set
            {
                if (_SplitViewWidth != value)
                {
                    this._SplitViewWidth = value;
                    this.OnPropertyChanged();
                }
            }
        } 
        #endregion

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
