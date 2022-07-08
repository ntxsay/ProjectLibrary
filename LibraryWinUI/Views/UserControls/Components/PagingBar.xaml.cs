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
        private int NbPages { get; set; }
        private int ActualPage { get; set; }
        public PagingBar()
        {
            this.InitializeComponent();
        }

        public int TotalPages
        {
            get => NbPages;
            set { SetValue(OnNbPagesChangedProperty, value); }
        }

        public static readonly DependencyProperty OnNbPagesChangedProperty = DependencyProperty.Register(nameof(TotalPages), typeof(int),
                                                                typeof(PagingBar), new PropertyMetadata(null, new PropertyChangedCallback(OnNbPagesChanged)));

        private static void OnNbPagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PagingBar parent && e.NewValue is int value)
            {
                parent.NbPages = value;
            }
        }

        public int CurrentPage
        {
            get => ActualPage;
            set { SetValue(OnCurrentPageChangedProperty, value); }
        }

        public static readonly DependencyProperty OnCurrentPageChangedProperty = DependencyProperty.Register(nameof(CurrentPage), typeof(int),
                                                                typeof(PagingBar), new PropertyMetadata(null, new PropertyChangedCallback(OnCurrentPageChanged)));

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PagingBar parent && e.NewValue is int value)
            {
                parent.ActualPage = value;
            }
        }
    }
}
