using AppHelpers;
using LibraryWinUI.Code.WebApi;
using LibraryWinUI.ViewModels;
using LibraryWinUI.ViewModels.Pages;
using LibraryWinUI.Views.SideBar;
using LibraryWinUI.Views.UserControls;
using LibraryWinUI.Views.UserControls.Components;
using LibShared;
using LibShared.Services;
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainCollectionPage : Page
    {
        readonly MainCollectionPageVM ViewModelPage = new ();
        internal Type TypeOfMainCollection { get; private set; }
        public MainCollectionPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Type type)
            {
                TypeOfMainCollection = type;

                if (type == typeof(LibraryVM))
                {
#warning Juste à des fins de tests
                    await TestGetLibrariesAsync();
                }
            }
            else if (e.Parameter is Tuple<long, Type> param1)
            {
                TypeOfMainCollection = param1.Item2;

                if (param1.Item2 == typeof(BookVM))
                {
                    await TestGetBooksAsync(param1.Item1);
                }
            }
        }

        private async Task TestGetLibrariesAsync()
        {
            try
            {
                LibraryWebApi libApi = new();
                LibraryRequestVM resquestResult = await libApi.GetLibrariesAsync(orderBy: OrderBy.Ascending, sortBy: SortBy.Name, maxItemsPerPage: 20, gotoPage: 1);
                if (resquestResult != null)
                {
                    ItemCollectionUC itemCollectionUC = new (this);
                    itemCollectionUC.InitializeCollection(resquestResult.List.GroupItemsBy(GroupBy.None), resquestResult.NbPages, resquestResult.CurrentPage);
                    FrameContainer.Content = itemCollectionUC;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainCollectionPage), exception:ex);
                return;
            }
        }

        private async Task TestGetBooksAsync(long idLibrary)
        {
            try
            {
                BookWebApi bookApi = new();
                BookRequestVM resquestResult = await bookApi.GetBooksAsync(idLibrary: idLibrary, orderBy: OrderBy.Ascending, sortBy: SortBy.Name, maxItemsPerPage: 20, gotoPage: 1);
                if (resquestResult != null)
                {
                    ItemCollectionUC itemCollectionUC = new(this);
                    itemCollectionUC.InitializeCollection(resquestResult.List.GroupItemsBy(GroupBy.None), resquestResult.NbPages, resquestResult.CurrentPage);
                    FrameContainer.Content = itemCollectionUC;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainCollectionPage), exception: ex);
                return;
            }
        }


        private void ASB_SearchItem_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //try
            //{
            //    if (sender.Text.IsStringNullOrEmptyOrWhiteSpace())
            //    {
            //        MyTeachingTip.Target = sender;
            //        MyTeachingTip.Title = sender.PlaceholderText;
            //        MyTeachingTip.Subtitle = "Vous devez d'abord entrer votre mot-clé avant de lancer la recherche.";
            //        MyTeachingTip.IsOpen = true;
            //        return;
            //    }

            //    if (MyTeachingTip.IsOpen)
            //    {
            //        MyTeachingTip.IsOpen = false;
            //    }

            //    ResearchItemVM researchItemVM = new ResearchItemVM()
            //    {
            //        Term = sender.Text?.Trim(),
            //        TermParameter = Code.Search.Terms.Contains,
            //        SearchInMainTitle = true,
            //    };

            //    if (IsContainsBookCollection(out _))
            //    {
            //        researchItemVM.SearchInAuthors = true;
            //        researchItemVM.SearchInEditors = true;
            //        researchItemVM.SearchInOtherTitles = true;
            //        researchItemVM.SearchInCollections = true;
            //    }

            //    LaunchSearch(new List<ResearchItemVM>() { researchItemVM }, true);
            //}
            //catch (Exception ex)
            //{
            //    MethodBase m = MethodBase.GetCurrentMethod();
            //    Logs.Log(ex, m);
            //    return;
            //}
        }

        #region CommandBar Initialize
        private void InitializeAddsCmdBarItemsForLibraryCollection()
        {
            try
            {
                MenuFlyoutCommandAdds.Items.Clear();

                MenuFlyoutItem TMFIAddNewItem = new MenuFlyoutItem()
                {
                    Text = "Nouvelle bibliothèque",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE710",
                    }
                };
                TMFIAddNewItem.Click += TMFIAddNewItem_Click;

                MenuFlyoutItem TMFIAddFromFile = new MenuFlyoutItem()
                {
                    Text = "Ouvrir un fichier",
                    IsEnabled = true,
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE8B5",
                    }
                };
                TMFIAddFromFile.Click += TMFIAddFromFile_Click;

                MenuFlyoutItem TMFIAddNewHuman = new MenuFlyoutItem()
                {
                    Text = "Ajouter une personne",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE77b",
                    }
                };
                TMFIAddNewHuman.Click += TMFIAddNewHuman_Click;

                MenuFlyoutItem TMFIAddNewSociety = new MenuFlyoutItem()
                {
                    Text = "Ajouter une société",
                    Icon = new FontIcon
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        Glyph = "\uE731",
                    }
                };
                TMFIAddNewSociety.Click += MFI_NewSociety_Click;

                MenuFlyoutCommandAdds.Items.Add(TMFIAddNewItem);
                MenuFlyoutCommandAdds.Items.Add(TMFIAddFromFile);
                MenuFlyoutCommandAdds.Items.Add(new MenuFlyoutSeparator());
                MenuFlyoutCommandAdds.Items.Add(TMFIAddNewHuman);
                MenuFlyoutCommandAdds.Items.Add(TMFIAddNewSociety);

            }
            catch (Exception ex)
            {
                Logs.Log(ex, m);
                return;
            }
        }

        private async void TMFIAddNewItem_Click(object sender, RoutedEventArgs e)
        {
            if (TypeOfMainCollection == typeof(LibraryVM))
            {
                await LibraryNewEditAsync(new LibraryVM(), EditMode.Create);
            }
            else if (TypeOfMainCollection == typeof(BookVM))
            {
                
            }
        }

        
        #endregion

        #region CommandBar Events
        #region Add Commands
        private void MenuFlyoutCommandAdds_Opened(object sender, object e)
        {

        }

        private void TMFIAddFromWebsite_Click(object sender, RoutedEventArgs e)
        {


        }

        private void TMFIAddFromFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TMFIAddFromExcelFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TMFIAddNewCollection_Click(object sender, RoutedEventArgs e)
        {
            //NewEditCollection(null, EditMode.Create);
        }

        private void TMFIAddNewHuman_Click(object sender, RoutedEventArgs e)
        {
            //this.NewEditContact(EditMode.Create, ContactType.Human, null, null);
        }

        private void MFI_NewSociety_Click(object sender, RoutedEventArgs e)
        {
            //this.NewEditContact(EditMode.Create, ContactType.Society, null, null);
        }
        #endregion
        #endregion

        #region Actions

        #region Library
        internal async Task LibraryNewEditAsync(LibraryThumbnailV1 element, EditMode editMode = EditMode.Create)
        {
            try
            {

                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is LibraryNewEditSideBar item && item.UiViewModel.EditMode == editMode) is LibraryNewEditSideBar checkedItem)
                {
                    var isModificationStateChecked = await checkedItem.CheckModificationsStateAsync();
                    if (isModificationStateChecked)
                    {
                        checkedItem.InitializeSideBar(this, element, editMode);
                        this.SelectItemSideBar(checkedItem);
                    }
                }
                else
                {
                    LibraryNewEditSideBar userControl = new();
                    userControl.InitializeSideBar(this, element, editMode);

                    userControl.CancelModificationRequested += LibraryNewEditSideBar_CancelModificationRequested; ;
                    userControl.ExecuteTaskRequested += LibraryNewEditSideBar_ExecuteTaskRequested; ;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.UiViewModel.Glyph,
                        Title = userControl.UiViewModel.Header,
                        IdItem = userControl.ItemGuid,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(MainCollectionPage), exception: ex);
                return;
            }
        }

        internal async Task LibraryNewEditAsync(LibraryVM viewModel, EditMode editMode = EditMode.Create)
        {
            try
            {

                if (this.PivotRightSideBar.Items.FirstOrDefault(f => f is LibraryNewEditSideBar item && item.UiViewModel.EditMode == editMode) is LibraryNewEditSideBar checkedItem)
                {
                    var isModificationStateChecked = await checkedItem.CheckModificationsStateAsync();
                    if (isModificationStateChecked)
                    {
                        checkedItem.InitializeSideBar(this, viewModel, editMode);
                        this.SelectItemSideBar(checkedItem);
                    }
                }
                else
                {
                    LibraryNewEditSideBar userControl = new();
                    userControl.InitializeSideBar(this, viewModel, editMode);

                    userControl.CancelModificationRequested += LibraryNewEditSideBar_CancelModificationRequested; ;
                    userControl.ExecuteTaskRequested += LibraryNewEditSideBar_ExecuteTaskRequested; ;

                    this.AddItemToSideBar(userControl, new SideBarItemHeaderVM()
                    {
                        Glyph = userControl.UiViewModel.Glyph,
                        Title = userControl.UiViewModel.Header,
                        IdItem = userControl.ItemGuid,
                    });
                }
                this.ViewModelPage.IsSplitViewOpen = true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(MainCollectionPage), exception: ex);
                return;
            }
        }


        private void LibraryNewEditSideBar_ExecuteTaskRequested(LibraryNewEditSideBar sender, LibraryVM originalViewModel, LibraryVM editedViewModel)
        {
            if (editedViewModel != null)
            {
                switch (sender.UiViewModel.EditMode)
                {
                    case EditMode.Create:
                        break;
                    case EditMode.Edit:
                        if (sender.ThumbnailV1 != null)
                        {
                            sender.ThumbnailV1.ViewModel = editedViewModel;
                        }
                        break;
                    default:
                        break;
                }
                //await this.RefreshItemsGrouping(this.GetSelectedPage, true);
                this.RemoveItemToSideBar(sender);
            }
        }

        private void LibraryNewEditSideBar_CancelModificationRequested(LibraryNewEditSideBar sender, ExecuteRequestedEventArgs e)
        {
            this.RemoveItemToSideBar(sender);
        } 
        #endregion

        #endregion

        #region SideBar
        private void CmbxSideBarItemTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is ComboBox cmbx && cmbx.SelectedItem is SideBarItemHeaderVM headerVM)
                {
                    foreach (var item in this.PivotRightSideBar.Items)
                    {
                        if (item is PivotItem pivotItem && pivotItem.Header is Grid grid && grid.Children[0] is SideBarItemHeader itemHeader)
                        {
                            if (itemHeader.Guid == headerVM.IdItem)
                            {
                                this.PivotRightSideBar.SelectedItem = item;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(MainCollectionPage), exception: ex);
                return;
            }
        }

        private void AddItemToSideBar(PivotItem item, SideBarItemHeaderVM sideBarItem)
        {
            try
            {
                this.PivotRightSideBar.Items.Add(item);
                this.PivotRightSideBar.SelectedItem = item;
                ViewModelPage.ItemsSideBarHeader.Add(sideBarItem);
                this.CmbxSideBarItemTitle.SelectedItem = sideBarItem;
                if (PivotRightSideBar.Items.Count >= 2)
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Visible;
                }
                else
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(MainCollectionPage), exception: ex);
                return;
            }
        }

        private void RemoveAllItemToSideBar()
        {
            try
            {
                if (this.CmbxSideBarItemTitle.Items.Count > 0)
                {
                    for (int i = 0; i < this.CmbxSideBarItemTitle.Items.Count; i++)
                    {
                        if (this.CmbxSideBarItemTitle.Items[i] is SideBarItemHeaderVM headerVM)
                        {
                            ViewModelPage.ItemsSideBarHeader.Remove(headerVM);
                            i = 0;
                            continue;
                        }
                    }
                }

                if (this.PivotRightSideBar.Items.Count > 0)
                {
                    for (int i = 0; i < this.PivotRightSideBar.Items.Count; i++)
                    {
                        this.PivotRightSideBar.Items.RemoveAt(i);
                        i = 0;
                        continue;
                    }
                }

                this.ViewModelPage.IsSplitViewOpen = false;
                this.CmbxSideBarItemTitle.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(MainCollectionPage), exception: ex);
                return;
            }
        }


        private void RemoveItemToSideBar(PivotItem item)
        {
            try
            {
                if (this.PivotRightSideBar.Items.Count == 1)
                {
                    this.ViewModelPage.IsSplitViewOpen = false;
                }

                if (this.CmbxSideBarItemTitle.Items.Count > 0)
                {
                    if (item.Header is Grid grid && grid.Children[0] is SideBarItemHeader itemHeader)
                    {
                        foreach (var cmbxItem in this.CmbxSideBarItemTitle.Items)
                        {
                            if (cmbxItem is SideBarItemHeaderVM headerVM)
                            {
                                if (itemHeader.Guid == headerVM.IdItem)
                                {
                                    ViewModelPage.ItemsSideBarHeader.Remove(headerVM);
                                    break;
                                }
                            }
                        }

                    }
                }

                this.PivotRightSideBar.Items.Remove(item);
                if (PivotRightSideBar.Items.Count < 2)
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.CmbxSideBarItemTitle.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(MainCollectionPage), exception: ex);
                return;
            }
        }

        private void SelectItemSideBar(PivotItem item)
        {
            try
            {
                if (!this.PivotRightSideBar.Items.Contains(item))
                {
                    return;
                }

                this.PivotRightSideBar.SelectedItem = item;

                if (this.CmbxSideBarItemTitle.Items.Count > 0)
                {
                    if (item.Header is Grid grid && grid.Children[0] is SideBarItemHeader itemHeader)
                    {
                        foreach (var cmbxItem in this.CmbxSideBarItemTitle.Items)
                        {
                            if (cmbxItem is SideBarItemHeaderVM headerVM)
                            {
                                if (itemHeader.Guid == headerVM.IdItem)
                                {
                                    this.CmbxSideBarItemTitle.SelectedItem = headerVM;
                                    return;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(MainCollectionPage), exception: ex);
                return;
            }
        }

        internal async Task<bool> CheckAndCloseSidebarsAsync()
        {
            try
            {
                for (int i = 0; i < this.PivotRightSideBar.Items.Count; i++)
                {
                    if (this.PivotRightSideBar.Items[i] is LibraryNewEditSideBar libraryNewEditSideBar)
                    {
                        bool isModificationStateChecked = await libraryNewEditSideBar.CheckModificationsStateAsync();
                        if (isModificationStateChecked)
                        {
                            this.RemoveItemToSideBar(libraryNewEditSideBar);
                            i = -1;
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(MainCollectionPage), exception: ex);
                return false;
            }
        }
        #endregion
    }
}
