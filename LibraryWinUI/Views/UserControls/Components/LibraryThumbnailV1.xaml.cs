﻿using LibShared.ViewModels.Libraries;
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
    public sealed partial class LibraryThumbnailV1 : Grid
    {
        private LibraryVM UiViewModel { get; set; }
        public LibraryThumbnailV1()
        {
            this.InitializeComponent();
        }

        public LibraryVM ViewModel
        {
            get { return (LibraryVM)GetValue(OnViewModelChangedProperty); }
            set { SetValue(OnViewModelChangedProperty, value); }
        }

        public static readonly DependencyProperty OnViewModelChangedProperty = DependencyProperty.Register(nameof(ViewModel), typeof(LibraryVM),
                                                                typeof(LibraryThumbnailV1), new PropertyMetadata(null, new PropertyChangedCallback(OnViewModelChanged)));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LibraryThumbnailV1 parent && e.NewValue is LibraryVM viewModel)
            {
                parent.UiViewModel = viewModel;
            }
        }

        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MFI_Change_Jaquette_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await esAppBaseApi.ReplaceJaquetteAsync<LivreVM>(viewModel.Guid);
                if (!result.IsSuccess)
                {
                    return;
                }

                UiViewModel.JaquettePath = result.Result?.ToString();// ?? EsGeneral.BookDefaultJaquette;
                var image = uiServices.GetSelectedThumbnailImage<LivreVM>(viewModel.Id, PivotItems, "GridViewItems");
                if (image != null)
                {
                    var bitmapImage = await Files.BitmapImageFromFileAsync(viewModel.JaquettePath);
                    image.Source = bitmapImage;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void MFI_Export_Item_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MFI_Edit_Item_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MFI_Delete_Item_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
