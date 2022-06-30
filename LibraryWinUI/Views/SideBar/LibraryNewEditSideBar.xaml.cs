using AppHelpers;
using AppHelpers.Extensions;
using AppHelpers.Strings;
using LibraryWinUI.ViewModels.Libraries;
using LibraryWinUI.ViewModels.SideBar;
using LibraryWinUI.Views.Pages;
using LibShared;
using LibShared.Services;
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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryWinUI.Views.SideBar
{
    public sealed partial class LibraryNewEditSideBar : PivotItem
    {
        public MainCollectionPage ParentPage { get; private set; }

        internal LibraryNewEditSideBarVM UiViewModel { get; set; } = new();
        public Guid ItemGuid { get; private set; } = Guid.NewGuid();

        internal LibraryVM OriginalViewModel { get; private set; }

        public delegate void CancelModificationEventHandler(LibraryNewEditSideBar sender, ExecuteRequestedEventArgs e);
        public event CancelModificationEventHandler CancelModificationRequested;

        internal delegate void ExecuteTaskEventHandler(LibraryNewEditSideBar sender, LibraryVM originalViewModel, OperationState e);
        internal event ExecuteTaskEventHandler ExecuteTaskRequested;

        public LibraryNewEditSideBar()
        {
            this.InitializeComponent();
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

                UiViewModel = new ()
                {
                    EditMode = editMode,
                };

                UiViewModel.Header = $"{(UiViewModel.EditMode == EditMode.Create ? "Ajouter" : "Editer")} une bibliothèque";

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
                Run runTitle = new ()
                {
                    Text = $"Vous êtes en train {(UiViewModel.EditMode == EditMode.Create ? "d'ajouter une nouvelle" : "d'éditer la")} bibliothèque",
                    //FontWeight = FontWeights.Medium,
                };
                TbcInfos.Inlines.Add(runTitle);

                if (UiViewModel.EditMode == EditMode.Edit)
                {
                    Run runCategorie = new ()
                    {
                        Text = " " + OriginalViewModel.Name,
                        FontWeight = FontWeights.Medium,
                    };
                    TbcInfos.Inlines.Add(runCategorie);
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

                OperationState result = null;
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
                var viewModelsEqual = CheckModificationsServices.GetPropertiesChanged(OriginalViewModel, UiViewModel.ViewModel);
                if (viewModelsEqual.Any())
                {
                    var dialog = new CheckModificationsStateCD(OriginalViewModel, viewModelsEqual)
                    {
                        Title = "Enregistrer vos modifications"
                    };

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        OperationState operationResult = null;
                        if (UiViewModel.EditMode == EditMode.Create)
                        {
                            operationResult = await CreateAsync();
                        }
                        else if (UiViewModel.EditMode == EditMode.Edit)
                        {
                            operationResult = await UpdateAsync();
                        }

                        return operationResult.IsSuccess;
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
                    UiViewModel.ResultMessageTitle = "Vérifiez vos informations";
                    UiViewModel.ResultMessage = $"Le nom de la bibliothèque ne peut pas être vide\nou ne contenir que des espaces blancs.";
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

        private async Task<OperationState> CreateAsync()
        {
            try
            {
                LibraryVM viewModel = this.UiViewModel.ViewModel;

                OperationState result = await DbServices.Library.CreateAsync(viewModel);
                if (result.IsSuccess)
                {
                    viewModel.Id = result.Id;
                    this.UiViewModel.ResultMessageTitle = "Succès";
                    this.UiViewModel.ResultMessage = result.Message;
                    this.UiViewModel.ResultMessageSeverity = InfoBarSeverity.Success;
                    this.UiViewModel.IsResultMessageOpen = true;
                    await eslibrary.SaveLibraryViewModelAsync(OriginalViewModel);
                }
                else
                {
                    //Erreur
                    this.UiViewModel.ResultMessageTitle = "Une erreur s'est produite";
                    this.UiViewModel.ResultMessage = result.Message;
                    this.UiViewModel.ResultMessageSeverity = InfoBarSeverity.Error;
                    this.UiViewModel.IsResultMessageOpen = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return null;
            }
        }

        private async Task<OperationState> UpdateAsync()
        {
            try
            {
                LibraryVM viewModel = this.UiViewModel.ViewModel;

                OperationState result = await DbServices.Library.UpdateAsync(viewModel);
                if (result.IsSuccess)
                {
                    OriginalViewModel.DeepCopy(viewModel);
                    await eslibrary.SaveLibraryViewModelAsync(OriginalViewModel);

                    this.UiViewModel.ResultMessageTitle = "Succès";
                    this.UiViewModel.ResultMessage = result.Message;
                    this.UiViewModel.ResultMessageSeverity = InfoBarSeverity.Success;
                    this.UiViewModel.IsResultMessageOpen = true;
                }
                else
                {
                    //Erreur
                    this.UiViewModel.ResultMessageTitle = "Une erreur s'est produite";
                    this.UiViewModel.ResultMessage = result.Message;
                    this.UiViewModel.ResultMessageSeverity = InfoBarSeverity.Error;
                    this.UiViewModel.IsResultMessageOpen = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(LibraryNewEditSideBar), exception: ex);
                return null;
            }
        }
        
    }
}
