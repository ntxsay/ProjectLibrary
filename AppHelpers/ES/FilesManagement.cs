using AppHelpers.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppHelpers.ES
{
    public partial class FilesHelpers
    {
        public struct Manage
        {
            public static async Task<bool> StreamToFileAsync(Stream stream, string fileName)
            {
                try
                {
                    if (stream == null)
                    {
                        throw new ArgumentNullException(nameof(stream), "Le stream ne peut pas être null.");
                    }

                    if (fileName.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        throw new ArgumentNullException(nameof(fileName), "Le chemin d'accès au fichier ne peut pas être vide.");
                    }

                    if (File.Exists(fileName) == true)
                    {
                        throw new IOException("Le fichier existe déjà.");
                    }

                    using (var fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    if (!File.Exists(fileName))
                    {
                        throw new IOException("Le fichier n'a pas pu être créé.");
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }

}
