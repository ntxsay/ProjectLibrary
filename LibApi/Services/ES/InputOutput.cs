using System;
using AppHelpers;
using AppHelpers.Strings;

namespace LibApi.Services.ES
{
    public class InputOutput
    {
        private const string mainRootName = "Ressources";
        public string MainRootPath { get; private set; }
        public static string RessourcesPath => System.IO.Path.Combine(Environment.CurrentDirectory, mainRootName);

        public static string LibrariesPath => $"{RessourcesPath}{System.IO.Path.DirectorySeparatorChar}Libraries";
        public static string BooksPath => $"{RessourcesPath}{System.IO.Path.DirectorySeparatorChar}Books";
        public static string ContactsPath => $"{RessourcesPath}{System.IO.Path.DirectorySeparatorChar}Contacts";


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
                    return null;
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
                        return ressourcesDirectory.CreateSubdirectory(NomDuDossierACreer);
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
                    return null;
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
                    return null;
                }

                string folderName = guid.ToString();

                DirectoryInfo? defaultFolderDirectoryInfo = CreateOrGetDefaultFolder(defaultFolder);
                if (defaultFolderDirectoryInfo == null || !defaultFolderDirectoryInfo.Exists)
                {
                    return null;
                }

                DirectoryInfo? existingFolder = defaultFolderDirectoryInfo.EnumerateDirectories().FirstOrDefault(d => d.Name.ToLower() == folderName.ToLower());
                if (existingFolder != null && existingFolder.Exists)
                {
                    return existingFolder;
                }
                else
                {
                    return defaultFolderDirectoryInfo.CreateSubdirectory(folderName);
                }
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(InputOutput), exception: ex);
                return null;
            }
        }
    }
}

