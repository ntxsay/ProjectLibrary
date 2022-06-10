using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
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


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
