﻿using LibraryWinUI.Code;
using LibraryWinUI.ViewModels.UserControls;
using LibraryWinUI.Views.Pages;
using LibShared.ViewModels;
using LibShared.ViewModels.Libraries;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI.Views.UserControls
{
    public sealed partial class ItemCollectionUC : UserControl
    {
        internal ItemCollectionUCVM UiViewModel { get; set; } = new();
        internal MainCollectionPage MainCollectionPage { get; init; }
        internal DataViewMode DataViewMode { get; set; }
        public ItemCollectionUC()
        {
            this.InitializeComponent();
        }

        public ItemCollectionUC(MainCollectionPage mainCollectionPage)
        {
            this.InitializeComponent();
            this.MainCollectionPage = mainCollectionPage;
        }

        public void InitializeCollection<T>(IEnumerable<IGrouping<string, T>> dataList, DataViewMode dataViewMode = DataViewMode.GridView) where T : class
        {
            DataViewMode = dataViewMode;

            if (typeof(T) == typeof(LibraryVM))
            {
                switch (dataViewMode)
                {
                    case DataViewMode.DataGridView:
                        break;
                    case DataViewMode.GridView:
                        this.PivotItems.ItemTemplate = (DataTemplate)this.Resources["GridViewLibraryTemplate"];
                        break;
                    case DataViewMode.ListView:
                        this.PivotItems.ItemTemplate = (DataTemplate)this.Resources["ListViewLibraryTemplate"];
                        break;
                    default:
                        this.PivotItems.ItemTemplate = (DataTemplate)this.Resources["GridViewLibraryTemplate"];
                        break;
                }
            }
            this.DataContext = new ObservableCollection<IGrouping<string, T>>(dataList);
        }

        private void PivotItems_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {

        }

        private void GridViewItems_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void GridViewItems_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void GridViewItems_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }
    }
}
