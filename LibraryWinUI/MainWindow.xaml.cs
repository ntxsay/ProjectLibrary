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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindowVM ViewModelPage { get; set; } = new MainWindowVM();
        private AppWindow m_AppWindow;
        public MainWindow()
        {
            this.InitializeComponent();

            m_AppWindow = GetAppWindowForCurrentWindow();

            // Check to see if customization is supported.
            // Currently only supported on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                var titleBar = m_AppWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                AppTitleBar.Loaded += AppTitleBar_Loaded;
                AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
            }
            else
            {
                // Title bar customization using these APIs is currently
                // supported only on Windows 11. In other cases, hide
                // the custom title bar element.
                AppTitleBar.Visibility = Visibility.Collapsed;

                // Show alternative UI for any functionality in
                // the title bar, such as search.
            }

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
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(MainWindow), nameof(InitializeWindow), ex);
                return;
            }
        }

        #region TitleBar
        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            // Check to see if customization is supported.
            // Currently only supported on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                SetDragRegionForCustomTitleBar(m_AppWindow);
            }
        }

        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Check to see if customization is supported.
            // Currently only supported on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported()
                && m_AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                // Update drag region if the size of the title bar changes.
                SetDragRegionForCustomTitleBar(m_AppWindow);
            }
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

        private double GetScaleAdjustment()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            // Get DPI.
            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
            if (result != 0)
            {
                throw new Exception("Could not get DPI for monitor.");
            }

            uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorPercent / 100.0;
        }

        private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
        {
            // Check to see if customization is supported.
            // Currently only supported on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported()
                && appWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                double scaleAdjustment = GetScaleAdjustment();

                RightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);
                LeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

                List<Windows.Graphics.RectInt32> dragRectsList = new();

                Windows.Graphics.RectInt32 dragRectL;
                dragRectL.X = (int)((LeftPaddingColumn.ActualWidth + 80) * scaleAdjustment); //(int)((LeftPaddingColumn.ActualWidth) * scaleAdjustment);
                dragRectL.Y = 0;
                dragRectL.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
                dragRectL.Width = (int)((IconColumn.ActualWidth
                                        + TitleColumn.ActualWidth
                                        + LeftDragColumn.ActualWidth) * scaleAdjustment);
                dragRectsList.Add(dragRectL);

                Windows.Graphics.RectInt32 dragRectR;
                dragRectR.X = (int)((LeftPaddingColumn.ActualWidth
                                    + IconColumn.ActualWidth
                                    + TitleTextBlock.ActualWidth
                                    + LeftDragColumn.ActualWidth
                                    ) * scaleAdjustment);//+ SearchColumn.ActualWidth
                dragRectR.Y = 0;
                dragRectR.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
                dragRectR.Width = (int)(RightDragColumn.ActualWidth * scaleAdjustment);
                dragRectsList.Add(dragRectR);

                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

                appWindow.TitleBar.SetDragRectangles(dragRects);
            }

        }
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

                if (itemTag == ViewModelPage.LibraryCollectionMenuItem.Tag)
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

                //if (this.MainFrameContainer.CanGoBack)
                //{
                //    AppTitleBar.Margin = new Thickness(40, 0, 0, 0);
                //}
                //else
                //{
                //    AppTitleBar.Margin = new Thickness(0, 0, 0, 0);
                //}
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
