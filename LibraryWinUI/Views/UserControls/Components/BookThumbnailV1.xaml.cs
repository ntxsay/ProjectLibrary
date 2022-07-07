using AppHelpers;
using AppHelpers.Extensions;
using LibraryWinUI.Code.Helpers;
using LibShared.ViewModels.Books;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI.Views.UserControls.Components
{
    public sealed partial class BookThumbnailV1 : Grid
    {
        private BookVM UiViewModel { get; set; } = new();
        public delegate void EditItemEventHandler(BookThumbnailV1 sender, BookVM viewModel);
        public event EditItemEventHandler EditItemRequested;

        public BookThumbnailV1()
        {
            this.InitializeComponent();
        }

        public BookVM ViewModel
        {
            //get { return (LibraryVM)GetValue(OnViewModelChangedProperty); }
            get => UiViewModel;
            set { SetValue(OnViewModelChangedProperty, value); }
        }

        public static readonly DependencyProperty OnViewModelChangedProperty = DependencyProperty.Register(nameof(ViewModel), typeof(BookVM),
                                                                typeof(BookThumbnailV1), new PropertyMetadata(null, new PropertyChangedCallback(OnViewModelChanged)));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BookThumbnailV1 parent && e.NewValue is BookVM viewModel)
            {
                parent.UiViewModel.DeepCopy(viewModel);
            }
        }

        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private async void Image_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Image imageCtrl)
                {
                    //LibraryWebApi libApi = new();
                    //BitmapImage bitmapImage = await libApi.GetJaquetteBitmap(UiViewModel.Id);
                    //if (bitmapImage != null)
                    //{
                    //    ImageThumbnail.Source = bitmapImage;
                    //}
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(BookThumbnailV1), exception: ex);
                return;
            }
        }

        private async void MFI_Change_Jaquette_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputOutputHelpers inputOutputHelpers = new();
                StorageFile file = await inputOutputHelpers.OpenFileAsync(AppHelpers.ES.FilesHelpers.Extensions.ImageExtensions);
                if (file == null)
                {
                    return;
                }

                //LibraryWebApi libApi = new();
                //bool isSuccess = await libApi.UpdloadJaquette(UiViewModel.Id, file.Path);
                //if (isSuccess)
                //{
                //    BitmapImage bitmapImage = await inputOutputHelpers.BitmapImageFromFileAsync(file);
                //    ImageThumbnail.Source = bitmapImage;
                //}
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(BookThumbnailV1), exception: ex);
                return;
            }
        }

        private async void MFI_Delete_Jaquette_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputOutputHelpers inputOutputHelpers = new();
                StorageFile file = await inputOutputHelpers.OpenFileAsync(AppHelpers.ES.FilesHelpers.Extensions.ImageExtensions);
                if (file == null)
                {
                    return;
                }

                //LibraryWebApi libApi = new();
                //bool isSuccess = await libApi.UpdloadJaquette(UiViewModel.Id, file.Path);
                //if (isSuccess)
                //{
                //    BitmapImage bitmapImage = await inputOutputHelpers.BitmapImageFromFileAsync(file);
                //    ImageThumbnail.Source = bitmapImage;
                //}
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(BookThumbnailV1), exception: ex);
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
