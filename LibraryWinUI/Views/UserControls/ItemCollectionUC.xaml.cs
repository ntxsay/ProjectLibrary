using AppHelpers;
using LibraryWinUI.Code;
using LibraryWinUI.Code.Helpers;
using LibraryWinUI.ViewModels.UserControls;
using LibraryWinUI.Views.Pages;
using LibShared.ViewModels;
using LibShared.ViewModels.Books;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI.Views.UserControls
{
    public sealed partial class ItemCollectionUC : UserControl
    {
        internal ItemCollectionUCVM UiViewModel { get; set; } = new();
        internal MainCollectionPage MainCollectionPage { get; private set; }
        internal DataViewMode DataViewMode { get; set; }
        readonly DatatemplateHelpers datatemplateHelpers = new ();
        internal Type TypeOfData { get; private set; }

        public ItemCollectionUC()
        {
            this.InitializeComponent();
        }

        public ItemCollectionUC(MainCollectionPage mainCollectionPage)
        {
            this.InitializeComponent();
            this.MainCollectionPage = mainCollectionPage;
        }

        public void InitializeCollection<T>(IEnumerable<IGrouping<string, T>> dataList, int nbPages, int currentPage, DataViewMode dataViewMode = DataViewMode.GridView) where T : class
        {
            DataViewMode = dataViewMode;
            TypeOfData = typeof(T);

            switch (dataViewMode)
            {
                case DataViewMode.DataGridView:
                    break;
                case DataViewMode.GridView:                    
                    this.PivotItems.ItemTemplate = (DataTemplate)this.Resources["ViewModeGridViewTemplate"];
                    break;
                case DataViewMode.ListView:
                    this.PivotItems.ItemTemplate = (DataTemplate)this.Resources["ListViewLibraryTemplate"];
                    break;
                default:
                    this.PivotItems.ItemTemplate = (DataTemplate)this.Resources["ViewModeGridViewTemplate"];
                    break;
            }
            this.DataContext = new ObservableCollection<IGrouping<string, T>>(dataList);
        }


        private void PivotItems_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void PivotItems_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {

        }

        private void GridViewItems_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is GridView gridView)
            {
                if (TypeOfData == typeof(LibraryVM))
                {
                    gridView.ItemTemplate = (DataTemplate)this.Resources["GridViewItemLibraryTemplate"];
                }
                else if (TypeOfData == typeof(BookVM))
                {
                    gridView.ItemTemplate = (DataTemplate)this.Resources["GridViewItemBookTemplate"];
                }
            }

        }

        private void GridViewItems_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void GridViewItems_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        

        //private void Image_Loaded(object sender, RoutedEventArgs e)
        //{

        //}

        //private void ViewboxSimpleThumnailDatatemplate_PointerPressed(object sender, PointerRoutedEventArgs e)
        //{

        //}

        private async void LibraryThumbnailV1_EditItemRequested(Components.LibraryThumbnailV1 sender, LibraryVM viewModel)
        {
            try
            {
                await this.MainCollectionPage.LibraryNewEditAsync(sender, LibShared.EditMode.Edit);
            }
            catch (Exception ex)
            {
                Logs.Log(className:nameof(ItemCollectionUC), exception:ex);
                return;
            }
        }

        private void LibraryThumbnailV1_OpenItemRequested(Components.LibraryThumbnailV1 sender, LibraryVM viewModel)
        {
            InputOutputHelpers inputOutputHelpers = new();
            inputOutputHelpers.window.MainCollectionNavigation(viewModel.Id, typeof(BookVM));
        }

        private void LibraryThumbnailV1_EditItemRequested_1(Components.LibraryThumbnailV1 sender, LibraryVM viewModel)
        {

        }

        private void BookThumbnailV1_EditItemRequested(Components.BookThumbnailV1 sender, LibShared.ViewModels.Books.BookVM viewModel)
        {

        }

        
    }
}
