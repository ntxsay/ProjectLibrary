using LibraryWinUI.Code.Helpers;
using LibShared.ViewModels.Libraries;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using AppHelpers.ES;
using AppHelpers;
using AppHelpers.Extensions;
using LibraryWinUI.Code.WebApi;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI.Views.UserControls.Components
{
    public sealed partial class LibraryThumbnailV1 : Grid
    {
        private LibraryVM UiViewModel { get; set; } = new();
        public delegate void OpenItemEventHandler(LibraryThumbnailV1 sender, LibraryVM viewModel);
        public event OpenItemEventHandler OpenItemRequested;

        public delegate void EditItemEventHandler(LibraryThumbnailV1 sender, LibraryVM viewModel);
        public event EditItemEventHandler EditItemRequested;

        public LibraryThumbnailV1()
        {
            this.InitializeComponent();
        }

        public LibraryVM ViewModel
        {
            //get { return (LibraryVM)GetValue(OnViewModelChangedProperty); }
            get => UiViewModel;
            set { SetValue(OnViewModelChangedProperty, value); }
        }

        public static readonly DependencyProperty OnViewModelChangedProperty = DependencyProperty.Register(nameof(ViewModel), typeof(LibraryVM),
                                                                typeof(LibraryThumbnailV1), new PropertyMetadata(null, new PropertyChangedCallback(OnViewModelChanged)));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LibraryThumbnailV1 parent && e.NewValue is LibraryVM viewModel)
            {
                parent.UiViewModel.DeepCopy(viewModel);
            }
        }

        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            OpenItemRequested?.Invoke(this, UiViewModel);
        }

        private async void Image_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Image imageCtrl)
                {
                    LibraryWebApi libApi = new();
                    BitmapImage bitmapImage = await libApi.GetJaquetteBitmap(UiViewModel.Id);
                    if (bitmapImage != null)
                    {
                        ImageThumbnail.Source = bitmapImage;
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryThumbnailV1), exception: ex);
                return;
            }
        }

        private async void MFI_Change_Jaquette_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputOutputHelpers inputOutputHelpers = new ();
                StorageFile file = await inputOutputHelpers.OpenFileAsync(FilesHelpers.Extensions.ImageExtensions);
                if (file == null)
                {
                    return;
                }

                LibraryWebApi libApi = new();
                bool isSuccess = await libApi.UpdloadJaquette(UiViewModel.Id, file.Path);
                if (isSuccess)
                {
                    BitmapImage bitmapImage = await inputOutputHelpers.BitmapImageFromFileAsync(file);
                    ImageThumbnail.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryThumbnailV1), exception: ex);
                return;
            }
        }

        private void MFI_Export_Item_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MFI_Edit_Item_Click(object sender, RoutedEventArgs e)
        {
            EditItemRequested?.Invoke(this, UiViewModel);
        }

        private void MFI_Delete_Item_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
