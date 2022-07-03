using AppHelpers;
using AppHelpers.Strings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;

namespace LibraryWinUI.Code.Helpers
{
    internal class InputOutputHelpers
    {
        internal MainWindow window = (Application.Current as App)?.m_window as MainWindow;

        internal async Task<StorageFile> OpenFileAsync(IEnumerable<string> fileTypeFilter,  PickerViewMode pickerViewMode = PickerViewMode.Thumbnail, PickerLocationId pickerLocationId = PickerLocationId.Downloads)
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
                Logs.Log(className: nameof(InputOutputHelpers), exception: ex);
                return null;
            }
        }

        internal async Task<IEnumerable<StorageFile>> OpenFilesAsync(IEnumerable<string> fileTypeFilter, PickerViewMode pickerViewMode = PickerViewMode.Thumbnail, PickerLocationId pickerLocationId = PickerLocationId.Downloads)
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
                Logs.Log(className: nameof(InputOutputHelpers), exception: ex);
                return Enumerable.Empty<StorageFile>();
            }
        }

        internal async Task<BitmapImage> BitmapImageFromFileAsync(string imageFileName)
        {
            try
            {
                if (imageFileName.IsStringNullOrEmptyOrWhiteSpace())
                {
#warning "Logging"
                    return null;
                }

                BitmapImage image = new BitmapImage();

                if (imageFileName.StartsWith("ms-appx:///"))
                {
                    var uri = new System.Uri(imageFileName);
                    StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                    using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
                    {
                        await image.SetSourceAsync(stream);
                    }
                    return image;
                    //return new BitmapImage(new Uri(imageFileName));
                }
                else if (imageFileName.StartsWith("http") || imageFileName.StartsWith("ftp"))
                {
                    var uri = new System.Uri(imageFileName);
                    var randomAccessStreamReference = RandomAccessStreamReference.CreateFromUri(uri);
                    using (IRandomAccessStream stream = await randomAccessStreamReference.OpenReadAsync())
                    {
                        await image.SetSourceAsync(stream);
                    }
                    return image;
                }
                else if (System.IO.File.Exists(imageFileName))
                {
                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync(imageFileName);
                    using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
                    {
                        await image.SetSourceAsync(stream);
                    }
                    return image;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutputHelpers), exception: ex);
                return null;
            }

        }

        internal async Task<BitmapImage> BitmapImageFromFileAsync(StorageFile imageFile)
        {
            try
            {
                if (imageFile == null || !imageFile.IsAvailable)
                {
#warning "Logging"
                    return null;
                }

                BitmapImage image = new ();

                using (IRandomAccessStream stream = await imageFile.OpenAsync(FileAccessMode.Read))
                {
                    await image.SetSourceAsync(stream);
                }
                return image;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutputHelpers), exception: ex);
                return null;
            }

        }

    }
}
