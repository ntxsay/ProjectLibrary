using AppHelpers.Strings;
using HtmlAgilityPack;
using Microsoft.UI.Dispatching;
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
using System.Web;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI.Views.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HtmlServicesWindow : Window
    {
        bool isWebPageLoaded = false;
        public HtmlServicesWindow()
        {
            this.InitializeComponent();
            DispatcherQueue.GetForCurrentThread().TryEnqueue(DispatcherQueuePriority.Low, new DispatcherQueueHandler(() =>
            {
                MyWebView2.Source = new Uri("https://translate.google.com/translate?sl=en&tl=fr&hl=fr&u=https://microsoft.github.io/microsoft-ui-xaml/&client=webapp");
                //MyWebView2.NavigateToString("https://translate.google.com/translate?sl=en&tl=fr&hl=fr&u=https://microsoft.github.io/microsoft-ui-xaml/&client=webapp");
            }));
        }

        private void MyWebView2_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            isWebPageLoaded = false;
        }

        private async void MyWebView2_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            isWebPageLoaded = true;

            string html = await MyWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");
            if (html.IsStringNullOrEmptyOrWhiteSpace())
            {
                return;
            }
            HttpUtility.de

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            TbxResult.Text = htmlDocument.Text;
        }
    }
}
