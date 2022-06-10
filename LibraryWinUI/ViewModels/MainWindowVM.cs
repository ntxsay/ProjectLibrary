using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.ViewModels
{
    public class ItemTagContentVM
    {
        public string Tag { get; set; }
        public string Text { get; set; }
    }

    public class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public MainWindowVM()
        {

        }

        public ItemTagContentVM AboutMenuItem => new()
        {
            Text = "À propos de ...",
            Tag = "About"
        };

        public ItemTagContentVM SettingsMenuItem => new()
        {
            Text = "Paramètres",
            Tag = "settings"
        };

        public ItemTagContentVM NewElementMenuItem => new()
        {
            Text = "Nouvel élément",
            Tag = "AddNewElement"
        };

        public ItemTagContentVM LibraryCollectionMenuItem => new()
        {
            Text = "Bibliothèques",
            Tag = "library-collection"
        };

        public ItemTagContentVM ContactCollectionMenuItem => new()
        {
            Text = "Adhérants",
            Tag = "contact-collection"
        };


        private NavigationViewBackButtonVisible _IsBackArrowVisible;
        public NavigationViewBackButtonVisible IsBackArrowVisible
        {
            get => _IsBackArrowVisible;
            set
            {
                if (_IsBackArrowVisible != value)
                {
                    _IsBackArrowVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _AppBarTitle;
        public object AppBarTitle
        {
            get => _AppBarTitle;
            set
            {
                if (_AppBarTitle != value)
                {
                    _AppBarTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        //public Viewbox MainTitleBar
        //{
        //    get => new Viewbox()
        //    {
        //        HorizontalAlignment = HorizontalAlignment.Left,
        //        Height = 16,
        //        Margin = new Thickness(0, 12, 0, 0),
        //        Child = new LibraryLongLogo(),
        //    };
        //}


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
