using AppHelpers;
using AppHelpers.Strings;
using LibraryWinUI.ViewModels;
using LibraryWinUI.ViewModels.Pages;
using LibShared.ViewModels.Libraries;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using Microsoft.UI.Dispatching;
using LibraryWinUI.Views.Pages;
using Windows.ApplicationModel.Resources;
using LibraryWinUI.Code.WebApi;

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

            this.InitializeAppTitleBar();

            TrySetAcrylicBackdrop();

            DispatcherQueue.GetForCurrentThread().TryEnqueue(DispatcherQueuePriority.Low, new DispatcherQueueHandler(() =>
            {
                this.InitializeWindow();
            }));
        }

        private void InitializeWindow()
        {
            try
            {
                LibrariesOrBooksCollectionNavigation();
                LibraryWebApi libApi = new();
                var task = libApi.GetLibraryVMsAsync().GetAwaiter();
                task.OnCompleted(() =>
                {
                    var result = task.GetResult();
                    Console.WriteLine(result);
                });
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainWindow), nameof(InitializeWindow), ex);
                return;
            }
        }

        #region TitleBar
        
        #endregion

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //var result = localFolder.CreateFolderAsync("HHH").GetAwaiter().GetResult();
            //var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + System.IO.Path.DirectorySeparatorChar + "G2H";
            //System.IO.Directory.CreateDirectory(path);

            //FileSavePicker savePicker = new FileSavePicker();
            FileOpenPicker openPicker = new FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            IntPtr hWnd = WindowNative.GetWindowHandle(this);

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            var files = await openPicker.PickMultipleFilesAsync();
            if (files != null)
            {
                foreach (var file in files)
                {

                }
            }
            else
            {
                //OutputTextBlock.Text = "Operation cancelled.";
            }
        }

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

                //ResourceLoader langResource = ResourceLoader.GetForViewIndependentUse("MainWindowRessources");
                if (itemTag == langResource.GetString("LibraryNavViewMenuItem/Tag"))
                {
                    LibrariesOrBooksCollectionNavigation();
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

        internal bool LibrariesOrBooksCollectionNavigation(LibraryVM library = null)
        {
            try
            {
                //this.ChangeAppTitle(ViewModelPage.MainTitleBar);
                return NavigateToView(typeof(MainCollectionPage), null);
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
