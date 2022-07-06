using AppHelpers.Strings;
using HtmlAgilityPack;
using LibraryWinUI.Code;
using LibraryWinUI.Code.Helpers;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Web;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI.Views.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HtmlServicesWindow : Window
    {
        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);
        bool isWebPageLoaded = false;
        public HtmlServicesWindow()
        {
            this.InitializeComponent();
            DispatcherQueue.GetForCurrentThread().TryEnqueue(DispatcherQueuePriority.Low, new DispatcherQueueHandler(() =>
            {
                TbxSearch.Text = "https://microsoft.github.io/microsoft-ui-xaml/";
            }));
        }

        private void MyWebView2_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            isWebPageLoaded = false;
        }

        private async void MyWebView2_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            isWebPageLoaded = true;
            await PrintToPdfAsync();
        }

        //private async void BtnTranslate_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (isWebPageLoaded)
        //        {
        //            InputOutputHelpers inputOutputHelpers = new InputOutputHelpers();
        //            var saveFile = await inputOutputHelpers.SaveFileAsync("snapshot.pdf", new Dictionary<string, IList<string>>()
        //    {
        //        {"images", new List<string>() { ".pdf" } }
        //    }, PickerLocationId.Downloads, Current);


        //            // Verify the user selected a file
        //            if (saveFile == null)
        //                return;
        //            //await MyWebView2.CoreWebView2.ExecuteScriptAsync("window.print();");
        //            await MyWebView2.CoreWebView2.PrintToPdfAsync(saveFile.Path, null);
        //            //string html = await MyWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");
        //            //if (html.IsStringNullOrEmptyOrWhiteSpace())
        //            //{
        //            //    return;
        //            //}
        //            //string sHtmlDecoded = System.Text.RegularExpressions.Regex.Unescape(html);
        //            //string sUrlDecoded = HttpUtility.HtmlDecode(sHtmlDecoded);
        //            //HtmlDocument htmlDocument = new();
        //            //htmlDocument.LoadHtml(sUrlDecoded);

        //            //TbxResult.Text = htmlDocument.Text;
        //        }

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


        private async void BtnTranslate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TbxSearch.Text.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return;
                }

                await MyWebView2.EnsureCoreWebView2Async();
                MyWebView2.CoreWebView2.Navigate($"https://translate.google.com/translate?sl=en&tl=fr&hl=fr&u={TbxSearch.Text.Trim()}&client=webapp");
                //MyWebView2.NavigateToString("https://translate.google.com/translate?sl=en&tl=fr&hl=fr&u=https://microsoft.github.io/microsoft-ui-xaml/&client=webapp");


            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Render to an image at the current system scale and retrieve pixel contents
            RenderTargetBitmap renderTargetBitmap = new ();
            grid.UpdateLayout();
            await renderTargetBitmap.RenderAsync(grid);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

            InputOutputHelpers inputOutputHelpers = new InputOutputHelpers();
            var saveFile = await inputOutputHelpers.SaveFileAsync("snapshot.png", new Dictionary<string, IList<string>>()
            {
                {"images", new List<string>() { ".png" } }
            }, PickerLocationId.Downloads, Current);


            // Verify the user selected a file
            if (saveFile == null)
                return;

            var buf = pixelBuffer.ToArray();

            // Encode the image to the selected file on disk
            using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                IntPtr hWnd = WindowNative.GetWindowHandle(this);
                WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
                DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
                IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

                // Get DPI.
                int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint dpiY);
                if (result != 0)
                {
                    throw new Exception("Could not get DPI for monitor.");
                }

                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    dpiX,
                    dpiY,
                    buf);

                await encoder.FlushAsync();
            }
        }

        private async void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TbxSearch.Text.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return;
                }

                await MyWebView2.EnsureCoreWebView2Async();
                MyWebView2.CoreWebView2.Navigate(TbxSearch.Text.Trim());
                //MyWebView2.NavigateToString("https://translate.google.com/translate?sl=en&tl=fr&hl=fr&u=https://microsoft.github.io/microsoft-ui-xaml/&client=webapp");

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void BtnPrintToPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isWebPageLoaded)
                {
                    InputOutputHelpers inputOutputHelpers = new InputOutputHelpers();
                    var saveFile = await inputOutputHelpers.SaveFileAsync("snapshot.pdf", new Dictionary<string, IList<string>>()
            {
                {"pdf", new List<string>() { ".pdf" } }
            }, PickerLocationId.Downloads, Current);


                    // Verify the user selected a file
                    if (saveFile == null)
                        return;
                    //await MyWebView2.CoreWebView2.ExecuteScriptAsync("window.print();");
                    await MyWebView2.CoreWebView2.PrintToPdfAsync(saveFile.Path, null);
                    //string html = await MyWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");
                    //if (html.IsStringNullOrEmptyOrWhiteSpace())
                    //{
                    //    return;
                    //}
                    //string sHtmlDecoded = System.Text.RegularExpressions.Regex.Unescape(html);
                    //string sUrlDecoded = HttpUtility.HtmlDecode(sHtmlDecoded);
                    //HtmlDocument htmlDocument = new();
                    //htmlDocument.LoadHtml(sUrlDecoded);

                    //TbxResult.Text = htmlDocument.Text;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void Btn_AllInOne_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //HtmlWeb hw = new HtmlWeb();
                //HtmlDocument doc = hw.Load(/* url */);

                string html = await MyWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");
                if (html.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return;
                }
                string sHtmlDecoded = System.Text.RegularExpressions.Regex.Unescape(html);
                string sUrlDecoded = HttpUtility.HtmlDecode(sHtmlDecoded);


                HtmlDocument htmlDocument = new();
                htmlDocument.LoadHtml(sUrlDecoded);
                foreach (HtmlNode link in htmlDocument.DocumentNode.SelectNodes("//a[@href]"))
                {
                    if (!link.HasAttributes || link.Attributes["href"] == null || link.Attributes["href"].Value.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        continue;
                    }

                    while (!isWebPageLoaded)
                    {
                        continue;
                    }
                    MyWebView2.CoreWebView2.Navigate(link.InnerText);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task PrintToPdfAsync()
        {
            try
            {
                if (isWebPageLoaded)
                {
                    
                    await MyWebView2.CoreWebView2.PrintToPdfAsync(@$"C:\Users\loicbastaraud\Downloads\{DateTime.Now:yyyyMMddHHmmss}.pdf", null);


                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
