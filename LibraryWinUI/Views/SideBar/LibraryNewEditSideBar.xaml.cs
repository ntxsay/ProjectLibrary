using AppHelpers;
using AppHelpers.Extensions;
using AppHelpers.Strings;
using LibraryWinUI.Code.WebApi;
using LibraryWinUI.ViewModels.SideBar;
using LibraryWinUI.Views.ContentDialogs;
using LibraryWinUI.Views.Pages;
using LibraryWinUI.Views.UserControls.Components;
using LibShared;
using LibShared.Services;
using LibShared.ViewModels;
using LibShared.ViewModels.Libraries;
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI.Views.SideBar
{
    public sealed partial class LibraryNewEditSideBar : PivotItem
    {
        public MainCollectionPage ParentPage { get; private set; }
        ResourceLoader langResource = ResourceLoader.GetForViewIndependentUse("LibraryNewEditSideBarRessources");

        internal LibraryNewEditSideBarVM UiViewModel { get; set; } = new();
        public Guid ItemGuid { get; private set; } = Guid.NewGuid();

        internal LibraryVM OriginalViewModel { get; private set; }
        internal UIElement LibraryItem { get; private set; }

        public delegate void CancelModificationEventHandler(LibraryNewEditSideBar sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        internal delegate void ExecuteTaskEventHandler(LibraryNewEditSideBar sender, LibraryVM originalViewModel, LibraryVM editedViewModel);
        internal event ExecuteTaskEventHandler ExecuteTaskRequested;

        public LibraryNewEditSideBar()
        {
            this.InitializeComponent();
        }

        internal void InitializeSideBar(MainCollectionPage parentPage, UIElement element, EditMode editMode)
        {
            try
            {
                ParentPage = parentPage;
                LibraryItem = element ?? throw new Exception("Le modèle de vue ne peut pas être null en mode édition.");
                
                if (element is LibraryThumbnailV1 libraryThumbnailV1)
                {
                    if (libraryThumbnailV1.ViewModel == null && editMode != EditMode.Create)
                    {
                        throw new Exception("Le modèle de vue ne peut pas être null en mode édition.");
                    }

                    this.OriginalViewModel = libraryThumbnailV1.ViewModel; //Attention de ne pas casser lien
                    InitializeSideBar(parentPage, libraryThumbnailV1.ViewModel, editMode);
                }
                else if (element is LibraryListViewV1 libraryListViewV1)
                {
                    if (libraryListViewV1.ViewModel == null && editMode != EditMode.Create)
                    {
                        throw new Exception("Le modèle de vue ne peut pas être null en mode édition.");
                    }

                    this.OriginalViewModel = libraryListViewV1.ViewModel; //Attention de ne pas casser lien
                    InitializeSideBar(parentPage, libraryListViewV1.ViewModel, editMode);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return;
            }
        }

        internal void InitializeSideBar(MainCollectionPage parentPage, LibraryVM libraryVM, EditMode editMode)
        {
            try
            {
                if (libraryVM == null && editMode != EditMode.Create)
                {
                    throw new Exception("Le modèle de vue ne peut pas être null en mode édition.");
                }

                ParentPage = parentPage;
                this.OriginalViewModel = libraryVM; //Attention de ne pas casser lien

                UiViewModel = new()
                {
                    EditMode = editMode,
                    CreateButtonVisibility = editMode == EditMode.Create ? Visibility.Visible : Visibility.Collapsed,
                    EditButtonVisibility = editMode == EditMode.Edit ? Visibility.Visible : Visibility.Collapsed,
                };

                UiViewModel.Header = UiViewModel.EditMode == EditMode.Create ? langResource.GetString("AddItem") : langResource.GetString("EditItem");

                if (UiViewModel.EditMode == EditMode.Create)
                {
                    UiViewModel.ViewModel = libraryVM?.DeepCopy() ?? new LibraryVM();
                }
                else if (UiViewModel.EditMode == EditMode.Edit)
                {
                    UiViewModel.ViewModel = libraryVM.DeepCopy();
                }
                InitializeActionInfos();

                //if (UiViewModel.EditMode == EditMode.Edit)
                //{
                //    this.Bindings.Update();
                //}
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return;
            }
        }


        private void InitializeActionInfos()
        {
            try
            {
                string subTitle = UiViewModel.EditMode == EditMode.Create ? langResource.GetString("AddItemSubTitle") : langResource.GetString("EditItemSubTitle");
                if (!subTitle.IsStringNullOrEmptyOrWhiteSpace())
                {
                    string[] splitSubTitle = subTitle.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    if (splitSubTitle.Length > 0)
                    {
                        foreach (string subTitleItem in splitSubTitle)
                        {
                            Run run = new()
                            {
                                Text = !subTitleItem.Contains("{x}") ? subTitleItem : OriginalViewModel.Name,
                            };

                            if (subTitleItem.Contains("{x}"))
                            {
                                run.FontWeight = FontWeights.Medium;
                            }

                            TbcInfos.Inlines.Add(run);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return;
            }
        }

        private async void CancelModificationXUiCmd_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            try
            {
                var isModificationStateChecked = await this.CheckModificationsStateAsync();
                if (isModificationStateChecked)
                {
                    CancelModificationRequested?.Invoke(this, args);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return;
            }
        }

        private async void BtnExecuteAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isValided = IsModelValided();
                if (!isValided)
                {
                    return;
                }

                LibraryVM result = null;
                if (UiViewModel.EditMode == EditMode.Create)
                {
                    result = await CreateAsync();
                }
                else if (UiViewModel.EditMode == EditMode.Edit)
                {
                    result = await UpdateAsync();
                }

                ExecuteTaskRequested?.Invoke(this, OriginalViewModel, result);
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return;
            }
        }

        public async Task<bool> CheckModificationsStateAsync()
        {
            try
            {
                IEnumerable<PropertiesChangedVM> viewModelsEqual = CheckModificationsServices.GetPropertiesChanged(OriginalViewModel, UiViewModel.ViewModel);
                if (viewModelsEqual.Any())
                {
                    var dialog = new CheckModificationsStateCD(OriginalViewModel, viewModelsEqual)
                    {
                        Title = langResource.GetString("SaveYourWork"),
                        XamlRoot = this.XamlRoot,
                    };

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        LibraryVM operationResult = null;
                        if (UiViewModel.EditMode == EditMode.Create)
                        {
                            operationResult = await CreateAsync();
                        }
                        else if (UiViewModel.EditMode == EditMode.Edit)
                        {
                            operationResult = await UpdateAsync();
                        }

                        return operationResult != null;
                    }
                    else if (result == ContentDialogResult.None)//Si l'utilisateur a appuyé sur le bouton annuler
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return true;
            }
        }

        private bool IsModelValided()
        {
            try
            {
                if (UiViewModel.ViewModel.Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    UiViewModel.ResultMessageTitle = langResource.GetString("SubmittingErrorTitle");
                    UiViewModel.ResultMessage = langResource.GetString("SubmittingErrorMessage1"); ;
                    UiViewModel.ResultMessageSeverity = InfoBarSeverity.Warning;
                    UiViewModel.IsResultMessageOpen = true;
                    return false;
                }

                UiViewModel.IsResultMessageOpen = false;
                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return false;
            }
        }

        private async Task<LibraryVM> CreateAsync()
        {
            try
            {
                LibraryVM viewModel = this.UiViewModel.ViewModel;

                LibraryWebApi libraryWebApi = new ();
                LibraryVM result = await libraryWebApi.CreateAsync(viewModel);
                if (result != null)
                {
                    viewModel.Id = result.Id;
                    this.UiViewModel.ResultMessageTitle = langResource.GetString("Success");
                    this.UiViewModel.ResultMessage = langResource.GetString("CreatingSuccess");
                    this.UiViewModel.ResultMessageSeverity = InfoBarSeverity.Success;
                    this.UiViewModel.IsResultMessageOpen = true;
                    return result;
                }
                else
                {
                    //Erreur
                    this.UiViewModel.ResultMessageTitle = langResource.GetString("AnErrorOccured");
                    this.UiViewModel.ResultMessage = langResource.GetString("CreatingNotSuccess");
                    this.UiViewModel.ResultMessageSeverity = InfoBarSeverity.Error;
                    this.UiViewModel.IsResultMessageOpen = true;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return null;
            }
        }

        private async Task<LibraryVM> UpdateAsync()
        {
            try
            {
                LibraryVM viewModel = this.UiViewModel.ViewModel;

                LibraryWebApi libraryWebApi = new();
                LibraryVM result = await libraryWebApi.UpdateAsync(OriginalViewModel.Id, viewModel);
                if (result != null)
                {
                    OriginalViewModel.DeepCopy(viewModel);

                    this.UiViewModel.ResultMessageTitle = langResource.GetString("Success");
                    this.UiViewModel.ResultMessage = langResource.GetString("UpdatingSuccess");
                    this.UiViewModel.ResultMessageSeverity = InfoBarSeverity.Success;
                    this.UiViewModel.IsResultMessageOpen = true;
                    return result;
                }
                else
                {
                    //Erreur
                    this.UiViewModel.ResultMessageTitle = langResource.GetString("AnErrorOccured");
                    this.UiViewModel.ResultMessage = langResource.GetString("UpdatingNotSuccess");
                    this.UiViewModel.ResultMessageSeverity = InfoBarSeverity.Error;
                    this.UiViewModel.IsResultMessageOpen = true;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return null;
            }
        }
        
    }
}
