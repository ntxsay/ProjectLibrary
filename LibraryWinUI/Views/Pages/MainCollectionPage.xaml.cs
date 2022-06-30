using AppHelpers;
using LibraryWinUI.ViewModels;
using LibraryWinUI.ViewModels.Pages;
using LibraryWinUI.Views.UserControls;
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

namespace LibraryWinUI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainCollectionPage : Page
    {
        readonly MainCollectionPageVM ViewModelPage = new ();
        public MainCollectionPage()
        {
            this.InitializeComponent();
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
        #endregion
    }
}
