using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI.Views.UserControls.Components
{
    public sealed partial class PagingBar : UserControl
    {
        private int _nbPages, _currentPage;
        public PagingBar()
        {
            this.InitializeComponent();
        }

        public int NbPages
        {
            //get { return (LibraryVM)GetValue(OnViewModelChangedProperty); }
            get => _nbPages;
            set { SetValue(OnViewModelChangedProperty, value); }
        }

        public static readonly DependencyProperty OnNbPagesChangedProperty = DependencyProperty.Register(nameof(ViewModel), typeof(LibraryVM),
                                                                typeof(LibraryThumbnailV1), new PropertyMetadata(null, new PropertyChangedCallback(OnViewModelChanged)));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LibraryThumbnailV1 parent && e.NewValue is LibraryVM viewModel)
            {
                parent.UiViewModel.DeepCopy(viewModel);
            }
        }
    }
}
