using LibraryWinUI.ViewModels.Pages;
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
        MainCollectionPageVM ViewModelPage = new ();
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

    }
}
