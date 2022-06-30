using LibraryWinUI.ViewModels.Libraries;
using LibShared.ViewModels;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
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

namespace LibraryWinUI.Views.ContentDialogs
{
    public sealed partial class CheckModificationsStateCD : ContentDialog
    {
        private object ViewModel { get; set; }
        private ObservableCollection<PropertiesChangedVM> ChangedProperties { get; set; } = new ();

        public CheckModificationsStateCD()
        {
            this.InitializeComponent();
        }

        public CheckModificationsStateCD(object _viewModel, IEnumerable<PropertiesChangedVM> _changedProperties)
        {
            ViewModel = _viewModel;
            ChangedProperties = new ObservableCollection<PropertiesChangedVM>(_changedProperties);
            this.InitializeComponent();
            InitializeActionInfos();
        }

        private void InitializeActionInfos()
        {
            try
            {
                tbkName.Inlines.Clear();

                Run run1 = new ()
                {
                    Text = $"Souhaitez-vous enregistrer les modifications apportées ",
                };
                tbkName.Inlines.Add(run1);

                if (ViewModel is BookVM book)
                {
                    Run run2 = new ()
                    {
                        Text = $"au livre « ",
                    };
                    tbkName.Inlines.Add(run2);

                    Run run3 = new ()
                    {
                        Text = book.MainTitle ?? "nouveau livre",
                        FontWeight = FontWeights.SemiBold,
                    };
                    tbkName.Inlines.Add(run3);
                }
                else if (ViewModel is LibraryVM library)
                {
                    Run run2 = new ()
                    {
                        Text = $"à la bibliothèque « ",
                    };
                    tbkName.Inlines.Add(run2);

                    Run run3 = new ()
                    {
                        Text = library.Name ?? "nouvelle bibliothèque",
                        FontWeight = FontWeights.SemiBold,
                    };
                    tbkName.Inlines.Add(run3);
                }

                Run run4 = new ()
                {
                    Text = $" » ?",
                };
                tbkName.Inlines.Add(run4);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
