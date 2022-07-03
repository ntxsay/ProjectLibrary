using AppHelpers;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace LibraryWinUI.Code.Helpers
{
    internal class InputOutHelpers
    {
        internal MainWindow window = (Application.Current as App)?.m_window as MainWindow;

        internal async Task<StorageFile> OpenFile(IEnumerable<string> fileTypeFilter,  PickerViewMode pickerViewMode = PickerViewMode.Thumbnail, PickerLocationId pickerLocationId = PickerLocationId.Downloads)
        {
            try
            {
                if (fileTypeFilter == null || !fileTypeFilter.Any())
                {
                    throw new ArgumentNullException(nameof(fileTypeFilter));
                }

                FileOpenPicker openPicker = new ();

                // Retrieve the window handle (HWND) of the current WinUI 3 window.
                IntPtr hWnd = WindowNative.GetWindowHandle(window);

                // Initialize the folder picker with the window handle (HWND).
                WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

                openPicker.ViewMode = pickerViewMode;
                openPicker.SuggestedStartLocation = pickerLocationId;

                foreach (var fileType in fileTypeFilter)
                {
                    openPicker.FileTypeFilter.Add(fileType); //ex: ".jpg";
                }

                StorageFile file = await openPicker.PickSingleFileAsync();
                if (file == null)
                {
                    throw new Exception("Le fichier n'a pas pû être récupérer.");
                }

                return file;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutHelpers), exception: ex);
                return null;
            }
        }

        internal async Task<IEnumerable<StorageFile>> OpenFiles(IEnumerable<string> fileTypeFilter, PickerViewMode pickerViewMode = PickerViewMode.Thumbnail, PickerLocationId pickerLocationId = PickerLocationId.Downloads)
        {
            try
            {
                if (fileTypeFilter == null || !fileTypeFilter.Any())
                {
                    throw new ArgumentNullException(nameof(fileTypeFilter));
                }

                FileOpenPicker openPicker = new();

                // Retrieve the window handle (HWND) of the current WinUI 3 window.
                IntPtr hWnd = WindowNative.GetWindowHandle(window);

                // Initialize the folder picker with the window handle (HWND).
                WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

                openPicker.ViewMode = pickerViewMode;
                openPicker.SuggestedStartLocation = pickerLocationId;

                foreach (var fileType in fileTypeFilter)
                {
                    openPicker.FileTypeFilter.Add(fileType); //ex: ".jpg";
                }

                IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
                if (files == null || files.Count == 0)
                {
                    throw new Exception("Les fichiers n'ont pas pû être récupéré.");

                }

                return files;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutHelpers), exception: ex);
                return Enumerable.Empty<StorageFile>();
            }
        }

    }
}
