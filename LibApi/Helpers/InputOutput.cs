using System;
using System.IO;
using System.Security.AccessControl;
using AppHelpers;
using AppHelpers.Serialization;
using AppHelpers.Strings;
using LibShared.ViewModels.Contacts;
using LibShared.ViewModels.Libraries;

namespace LibApi.Helpers
{
    public class InputOutput
    {
        private const string mainRootName = "Ressources";
        public const string itemModelFile = "model.json";
        public const string LibraryJaquette = "Library_Jaquette";
        public static string RessourcesPath => Path.Combine(Environment.CurrentDirectory, mainRootName);

        public static string LibrariesPath => $"{RessourcesPath}{Path.DirectorySeparatorChar}Libraries";
        public static string BooksPath => $"{RessourcesPath}{Path.DirectorySeparatorChar}Books";
        public static string ContactsPath => $"{RessourcesPath}{Path.DirectorySeparatorChar}Contacts";

        public static Dictionary<byte, string> FoldersDictionary = new ()
        {
            {(byte)DefaultFolders.Libraries, LibrariesPath },
            {(byte)DefaultFolders.Books, BooksPath },
            {(byte)DefaultFolders.Contacts, ContactsPath },
        };

        public DirectoryInfo? GetRessourcesFolder()
        {
            try
            {
                DirectoryInfo directoryInfo = new(RessourcesPath);

                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                return directoryInfo;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(InputOutput), exception: ex);
                return null;
            }
        }

        public DirectoryInfo? CreateOrGetRootFolder(string NomDuDossierACreer)
        {
            try
            {
                if (NomDuDossierACreer.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(NomDuDossierACreer), "Le GUID n'est pas valide.");
                }

                DirectoryInfo? ressourcesDirectory = this.GetRessourcesFolder();
                if (ressourcesDirectory != null && ressourcesDirectory.Exists)
                {
                    DirectoryInfo? existingFolder = ressourcesDirectory.EnumerateDirectories().FirstOrDefault(d => d.Name.ToLower() == NomDuDossierACreer.ToLower());
                    if (existingFolder != null && existingFolder.Exists)
                    {
                        return existingFolder;
                    }
                    else
                    {
                        DirectoryInfo? newDirectory = ressourcesDirectory.CreateSubdirectory(NomDuDossierACreer);
                        if (newDirectory == null || !newDirectory.Exists)
                        {
                            throw new Exception("Le nouveau dossier n'est pas valide ou n'existe pas.");
                        }
                        return newDirectory;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(InputOutput), exception: ex);
                return null;
            }
        }

        public DirectoryInfo? CreateOrGetDefaultFolder(DefaultFolders defaultFolder)
        {
            try
            {
                string? folderPath = FoldersDictionary.GetValueOrDefault((byte)defaultFolder);
                if (folderPath == null)
                {
                    throw new Exception("Le chemin d'accès n'est pas valide ou n'existe pas.");
                }

                DirectoryInfo directoryInfo = new(folderPath);

                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                return directoryInfo;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(InputOutput), exception: ex);
                return null;
            }
        }

        public DirectoryInfo? GetOrCreateDefaultFolderItem(Guid guid, DefaultFolders defaultFolder)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(guid), "Le GUID n'est pas valide.");
                }

                string folderName = guid.ToString();

                DirectoryInfo? defaultFolderDirectoryInfo = CreateOrGetDefaultFolder(defaultFolder);
                if (defaultFolderDirectoryInfo == null || !defaultFolderDirectoryInfo.Exists)
                {
                    throw new Exception("Le dossier parent n'est pas valide ou n'existe pas.");
                }

                DirectoryInfo? existingFolder = defaultFolderDirectoryInfo.EnumerateDirectories().FirstOrDefault(d => d.Name.ToLower() == folderName.ToLower());
                if (existingFolder != null && existingFolder.Exists)
                {
                    return existingFolder;
                }
                else
                {
                    DirectoryInfo? newDirectory = defaultFolderDirectoryInfo.CreateSubdirectory(folderName);
                    if (newDirectory == null || !newDirectory.Exists)
                    {
                        throw new Exception("Le nouveau dossier n'est pas valide ou n'existe pas.");
                    }
                    return newDirectory;
                }
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(InputOutput), exception: ex);
                return null;
            }
        }

        public bool DeleteDefaultFolderItem(Guid guid, DefaultFolders defaultFolder)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(guid), "Le GUID n'est pas valide.");
                }

                string folderName = guid.ToString();

                DirectoryInfo? defaultFolderDirectoryInfo = CreateOrGetDefaultFolder(defaultFolder);
                if (defaultFolderDirectoryInfo == null || !defaultFolderDirectoryInfo.Exists)
                {
                    throw new Exception("Le dossier parent n'est pas valide ou n'existe pas.");
                }

                DirectoryInfo? existingFolder = defaultFolderDirectoryInfo.EnumerateDirectories().FirstOrDefault(d => d.Name.ToLower() == folderName.ToLower());
                if (existingFolder != null && existingFolder.Exists)
                {
                    existingFolder.Delete(true);
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutput), exception: ex);
                return false;
            }
        }

        public FileInfo? GetOrCreateItemModelConfigFile(Guid guid, DefaultFolders defaultFolder)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(guid), "Le GUID n'est pas valide.");
                }

                string? filePath = GetItemModelConfigFilePath(guid, defaultFolder);
                if (filePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(guid), "Le chemin d'accès n'est pas valide.");
                }

                FileInfo? newFile = new(filePath);
                if (newFile == null)
                {
                    throw new Exception("Le fichier n'est pas valide ou n'existe pas.");
                }
                else if (!newFile.Exists)
                {
                    using var fileStream = newFile.Create();
                    return newFile;
                }

                return newFile;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutput), exception: ex);
                return null;
            }
        }

        public string? GetItemModelConfigFilePath(Guid guid, DefaultFolders defaultFolder)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(guid), "Le GUID n'est pas valide.");
                }

                DirectoryInfo? defaultFolderDirectoryInfo = GetOrCreateDefaultFolderItem(guid, defaultFolder);
                if (defaultFolderDirectoryInfo != null && defaultFolderDirectoryInfo.Exists)
                {
                    string filePath = $"{defaultFolderDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{itemModelFile}";
                    return filePath;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutput), exception: ex);
                return null;
            }
        }

        public bool SaveToJson(Guid guid, DefaultFolders defaultFolder, object value)
        {
            try
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "La valeur est null.");
                }

                if (guid == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(guid), "Le GUID n'est pas valide.");
                }

                string? filePath = GetItemModelConfigFilePath(guid, defaultFolder);                
                if (filePath.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new Exception("Le chemin d'accès au fichier de configuration n'est pas valide");
                }

                bool isSaved = JsonSerialization.SerializeAsync(value, filePath);
                if (isSaved == false)
                {
                    throw new Exception("Le fichier n'a pas été enregistré.");
                }

                return isSaved;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutput), exception: ex);
                return false;
            }
        }

        public async Task<bool> CopyJaquetteFileAsync(Guid guid, DefaultFolders defaultFolder, byte[] fileBytes, string fileName)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(guid), "Le GUID n'est pas valide.");
                }

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    throw new ArgumentNullException(nameof(fileBytes), "Ce parmètre ne peut pas être null.");
                }

                if (fileName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(fileName), "Ce parmètre ne peut pas être null ou ne contenir que des espaces blancs.");
                }

                bool isOlderDeleted = RemoveJaquetteFile(guid, defaultFolder);
                if (!isOlderDeleted)
                {
                    throw new Exception();
                }

                DirectoryInfo? directoryInfo = GetOrCreateDefaultFolderItem(guid, defaultFolder);
                if (directoryInfo == null || !directoryInfo.Exists)
                {
                    throw new Exception("Le dossier n'est pas valide ou n'existe pas.");
                }

                string filePath = directoryInfo.FullName + Path.DirectorySeparatorChar + $"{LibraryJaquette}{Path.GetExtension(fileName)}";

                await File.WriteAllBytesAsync(filePath, fileBytes);
                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutput), exception: ex);
                return false;
            }
        }

        public async Task<byte[]> GetJaquetteFileAsync(Guid guid, DefaultFolders defaultFolder)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(guid), "Le GUID n'est pas valide.");
                }

                DirectoryInfo? directoryInfo = GetOrCreateDefaultFolderItem(guid, defaultFolder);
                if (directoryInfo == null || !directoryInfo.Exists)
                {
                    throw new Exception("Le dossier n'est pas valide ou n'existe pas.");
                }

                FileInfo? fileInfo = directoryInfo.EnumerateFiles().FirstOrDefault(s => s.Name.ToLower().StartsWith(LibraryJaquette.ToLower()));
                if (fileInfo == null || !fileInfo.Exists)
                {
                    throw new Exception("Le fichier n'est pas valide ou n'existe pas.");
                }
               
                using FileStream fileStream = fileInfo.OpenRead();
                using MemoryStream memoryStream = new ();
                await fileStream.CopyToAsync(memoryStream);
                byte[] byteArray = memoryStream.ToArray();

                return byteArray;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutput), exception: ex);
                return Array.Empty<byte>();
            }
        }


        public bool RemoveJaquetteFile(Guid guid, DefaultFolders defaultFolder)
        {
            try
            {
                if (guid == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(guid), "Le GUID n'est pas valide.");
                }

                DirectoryInfo? directoryInfo = GetOrCreateDefaultFolderItem(guid, defaultFolder);
                if (directoryInfo == null || !directoryInfo.Exists)
                {
                    throw new Exception("Le dossier n'est pas valide ou n'existe pas.");
                }

                switch (defaultFolder)
                {
                    case DefaultFolders.Libraries:
                        return RemoveFile(LibraryJaquette, directoryInfo, AppHelpers.ES.FilesHelpers.FileSearchOptions.StartWith);
                    case DefaultFolders.Books:
                        break;
                    case DefaultFolders.Contacts:
                        break;
                    default:
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutput), exception: ex);
                return false;
            }
        }

        public bool RemoveFile(string baseName, DirectoryInfo folderInfo, AppHelpers.ES.FilesHelpers.FileSearchOptions options = AppHelpers.ES.FilesHelpers.FileSearchOptions.StartWith)
        {
            try
            {
                if (baseName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(baseName), "Le chemin d'accès au fichier ne peut pas être vide ou ne contenir que des espaces blancs.");
                }

                if (folderInfo == null || !folderInfo.Exists)
                {
                    throw new ArgumentNullException(nameof(folderInfo), "Le dossier n'est pas valide ou n'existe pas.");
                }

                IEnumerable<FileInfo> fileInfosToDelete = Enumerable.Empty<FileInfo>();
                switch (options)
                {
                    case AppHelpers.ES.FilesHelpers.FileSearchOptions.StartWith:
                        fileInfosToDelete = folderInfo.GetFiles().Where(s => s.Name.ToLower().StartsWith(baseName.ToLower()));
                        break;
                    case AppHelpers.ES.FilesHelpers.FileSearchOptions.Contains:
                        fileInfosToDelete = folderInfo.GetFiles().Where(s => s.Name.ToLower().Contains(baseName.ToLower()));
                        break;
                    case AppHelpers.ES.FilesHelpers.FileSearchOptions.EndWith:
                        fileInfosToDelete = folderInfo.GetFiles().Where(s => s.Name.ToLower().EndsWith(baseName.ToLower()));
                        break;
                    case AppHelpers.ES.FilesHelpers.FileSearchOptions.Egal:
                        fileInfosToDelete = folderInfo.GetFiles().Where(s => s.Name.ToLower() == baseName.ToLower());
                        break;
                }

                if (fileInfosToDelete == null || !fileInfosToDelete.Any())
                {
                    return true;
                }

                foreach (var file in fileInfosToDelete)
                {
                    file.Delete();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(InputOutput), exception: ex);
                return false;
            }
        }
    }
}

