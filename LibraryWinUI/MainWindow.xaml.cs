using AppHelpers;
using AppHelpers.Strings;
using LibraryWinUI.ViewModels;
using LibraryWinUI.Views.Pages;
using LibShared.ViewModels.Libraries;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using Windows.ApplicationModel.Resources;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        //readonly ResourceLoader langResource = ResourceLoader.GetForCurrentView("MainWindowRessources");
        ResourceLoader langResource = ResourceLoader.GetForViewIndependentUse("MainWindowRessources");
        public MainWindowVM ViewModelPage { get; set; } = new MainWindowVM();
        public MainWindow()
        {
            this.InitializeComponent();
            m_AppWindow = GetAppWindowForCurrentWindow();
            m_AppWindow.Closing += M_AppWindow_Closing;

            this.InitializeAppTitleBar();

            TrySetAcrylicBackdrop();

            DispatcherQueue.GetForCurrentThread().TryEnqueue(DispatcherQueuePriority.Low, new DispatcherQueueHandler(() =>
            {
                this.InitializeWindow();
            }));
        }

        private async void M_AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            try
            {
                args.Cancel = true;
                if (MainFrameContainer.Content is MainCollectionPage mainCollectionPage)
                {
                    bool isCheckedBeforeClose = await mainCollectionPage.CheckAndCloseSidebarsAsync();
                    if (isCheckedBeforeClose)
                    {
                        App.Current.Exit();
                    }
                }

            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainWindow), exception: ex);
                return;
            }
        }


        private void InitializeWindow()
        {
            try
            {
                MainCollectionNavigation(typeof(LibraryVM));
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainWindow), nameof(InitializeWindow), ex);
                return;
            }
        }

        #region TitleBar
        
        #endregion

        public NavigationViewItem _lastItemMUCX;
        private void PrincipalNaviguation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            try
            {
                if (args.InvokedItemContainer is not NavigationViewItem item || item == _lastItemMUCX)
                {
                    //sender.Content = null;
                    return;
                }

                if (item.Tag != null)
                {
                    NavigateToView(item.Tag.ToString(), item);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainWindow), nameof(PrincipalNaviguation_ItemInvoked), ex);
                return;
            }
        }

        private void NavigateToView(string itemTag, NavigationViewItem item = null)
        {
            try
            {
                if (itemTag.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return;
                }

                if (itemTag == langResource.GetString("LibraryNavViewMenuItem/Tag"))
                {
                    MainCollectionNavigation(typeof(LibraryVM));
                }

                item.IsSelected = true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainWindow), nameof(NavigateToView), ex);
                return;
            }
        }

        private void PrincipalNaviguation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            this.GoToBack();
        }

        internal bool MainCollectionNavigation(Type type)
        {
            try
            {
                //this.ChangeAppTitle(ViewModelPage.MainTitleBar);
                if (MainFrameContainer.Content is not MainCollectionPage)
                {
                    return NavigateToView(typeof(MainCollectionPage), type);
                }

                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainWindow), nameof(CheckBackArrowVisibility), ex);
                return false;
            }
        }

        internal bool NavigateToView(Type pageType, object parameters = null)
        {
            try
            {
                bool isSuccess = this.MainFrameContainer.Navigate(pageType, parameters, new EntranceNavigationTransitionInfo());
                CheckBackArrowVisibility();
                return isSuccess;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainWindow), nameof(CheckBackArrowVisibility), ex);
                return false;
            }
        }

        internal void CheckBackArrowVisibility()
        {
            try
            {
                this.ViewModelPage.IsBackArrowVisible = this.MainFrameContainer.CanGoBack
                    ? NavigationViewBackButtonVisible.Visible
                    : NavigationViewBackButtonVisible.Collapsed;

                if (!this.MainFrameContainer.CanGoBack)
                {
                    AppTitleBar.Margin = new Thickness(40, 0, 0, 0);
                }
                else
                {
                    AppTitleBar.Margin = new Thickness(80, 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainWindow), nameof(CheckBackArrowVisibility), ex);
                return;
            }
        }

        internal void GoToBack()
        {
            try
            {
                if (this.MainFrameContainer.CanGoBack)
                {
                    this.MainFrameContainer.GoBack();
                }

                CheckBackArrowVisibility();
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainWindow), nameof(GoToBack), ex);
                return;
            }
        }

        
    }
}
