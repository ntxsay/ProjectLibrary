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
        public enum FileSearchOptions
        {
            StartWith,
            Contains,
            EndWith,
            Egal,
        }

        public struct Extentions
        {
            
            public enum AuthorizedDocsExtension : byte
            {
                pdf,
                txt,
                rtf,
                doc,
                docx,
                ppt,
                pptx,
            }

            public Dictionary<byte, string> AuthorizedAllDocsFileExtension
            {
                get => new Dictionary<byte, string>()
            {
                { (byte)AuthorizedDocsExtension.pdf, ".pdf" },
                { (byte)AuthorizedDocsExtension.txt, ".txt" },
                { (byte)AuthorizedDocsExtension.rtf, ".rtf"},
                { (byte)AuthorizedDocsExtension.doc, ".doc"},
                { (byte)AuthorizedDocsExtension.docx, ".docx"},
                { (byte)AuthorizedDocsExtension.ppt, ".ppt"},
                { (byte)AuthorizedDocsExtension.pptx, ".pptx"},
            };
            }

            public Dictionary<byte, string> AuthorizedPresentationFileExtension
            {
                get => new Dictionary<byte, string>()
            {
                { (byte)AuthorizedDocsExtension.pdf, ".pdf" },
                { (byte)AuthorizedDocsExtension.txt, ".txt" },
                { (byte)AuthorizedDocsExtension.rtf, ".rtf"},
                { (byte)AuthorizedDocsExtension.doc, ".doc"},
                { (byte)AuthorizedDocsExtension.docx, ".docx"},
                { (byte)AuthorizedDocsExtension.ppt, ".ppt"},
                { (byte)AuthorizedDocsExtension.pptx, ".pptx"},
            };
            }

            public Dictionary<byte, string> AuthorizedDocsFileExtension
            {
                get => new Dictionary<byte, string>()
                        {
                            { (byte)AuthorizedDocsExtension.rtf, ".rtf"},
                            { (byte)AuthorizedDocsExtension.doc, ".doc"},
                            { (byte)AuthorizedDocsExtension.docx, ".docx"},
                        };
            }

            public class ExtMime
            {
                public string Extension { get; set; }
                public string Mime { get; set; }
            }

            public struct MimeList
            {
                public const string CSV = "text/csv";
            }

            public class ExtentionsList
            {
                public enum EnumExtentions : short
                {
                    txt,
                    pdf,
                    rtf,
                    doc,
                    docx,
                    ppt,
                    pptx,
                    xls,
                    xlsx,
                    png,
                    jpg,
                    jpeg,
                    gif,
                    csv,
                    mp4,
                    json,
                }

                public const string XMLExt = ".xml";
                public const string XMLFilter = "Fichiers xml (*.xml)|*.xml|Tous les fichiers (*.*)|*.*";
                /// <summary>
                /// Filtre de fichiers de type image pour OpenFileDialog
                /// </summary>
                public const string OpenFile_Ext_IMG = "Fichiers images|*.jpg;*.jpeg;*.gif;*.bmp;*.wmf;*.png";
                /// <summary>
                /// Filtre de fichiers de type vidéo pour OpenFileDialog
                /// </summary>
                public const string OpenFile_Ext_Movie = "Videos|*.mp4;*.m4v;*.mp4v;*.avi;*.wmv;*.mkv;*.mpg;*.mpeg;*.mov;*.3g2;*.3gp;*.3gp2;*.3gpp;*.m2ts;*.m2t";
                /// <summary>
                /// Filtre de fichiers de type music pour OpenFileDialog
                /// </summary>
                public const string OpenFile_Ext_Music = "Musiques|*.mp3;*.mid;*.wav;*.wma";
                public struct EspaceMultimedia
                {
                    public const string KeyWordExt = ".emkw";
                    public const string ThemesExt = ".emth";
                    public const string GroupExt = ".emgp";
                    public const string MediaExt = ".emmd";
                    public const string KeyWordFilter = "Fichiers xml (*.emkw)|*.emkw|Tous les fichiers (*.*)|*.*";
                    public const string ThemesFilter = "Fichiers xml (*.emth)|*.emth|Tous les fichiers (*.*)|*.*";
                    public const string GroupFilter = "Fichiers xml (*.emgp)|*.emgp|Tous les fichiers (*.*)|*.*";
                    public const string MediaFilter = "Fichiers xml (*.emmd)|*.emmd|Tous les fichiers (*.*)|*.*";
                }

                public struct PublicDocuments
                {
                    public const string CategorieExt = ".pdbcat";
                    public const string CategorieFilter = "Fichiers xml (*.pdbcat)|*.pdbcat|Tous les fichiers (*.*)|*.*";
                }

                public static readonly Dictionary<ExtensionPairKeyValue1, ExtensionPairKeyValue2> GetMimeTypes = new Dictionary<ExtensionPairKeyValue1, ExtensionPairKeyValue2>
                {
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.txt, ExtentionName = ".txt"}, new ExtensionPairKeyValue2() { ContentType = "text/plain", Type = (short)FileTypes.Document}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.pdf, ExtentionName = ".pdf"}, new ExtensionPairKeyValue2() { ContentType = "application/pdf", Type = (short)FileTypes.Document}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.doc, ExtentionName = ".doc"}, new ExtensionPairKeyValue2() { ContentType = "application/vnd.ms-word", Type = (short)FileTypes.Document}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.docx, ExtentionName = ".docx"},  new ExtensionPairKeyValue2() { ContentType = "application/vnd.ms-word", Type = (short)FileTypes.Document}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.xls, ExtentionName = ".xls"},  new ExtensionPairKeyValue2() { ContentType = "application/vnd.ms-excel", Type = (short)FileTypes.Document}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.xlsx, ExtentionName = ".xlsx"},  new ExtensionPairKeyValue2() { ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Type = (short)FileTypes.Document}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.png, ExtentionName = ".png"},  new ExtensionPairKeyValue2() { ContentType = "image/png", Type = (short)FileTypes.Image}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.jpg, ExtentionName = ".jpg"},  new ExtensionPairKeyValue2() { ContentType = "image/jpeg", Type = (short)FileTypes.Image}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.jpeg, ExtentionName = ".jpeg"},  new ExtensionPairKeyValue2() { ContentType = "image/jpeg", Type = (short)FileTypes.Image}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.gif, ExtentionName = ".gif"},  new ExtensionPairKeyValue2() { ContentType = "image/gif", Type = (short)FileTypes.Image}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.csv, ExtentionName = ".csv"},  new ExtensionPairKeyValue2() { ContentType = MimeList.CSV, Type = (short)FileTypes.Document}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.mp4, ExtentionName = ".mp4"},  new ExtensionPairKeyValue2() { ContentType = "video/mp4" , Type = (short)FileTypes.Video}},
                    {new ExtensionPairKeyValue1() { ID = (short)EnumExtentions.json, ExtentionName = ".json"},  new ExtensionPairKeyValue2() { ContentType = "application/json" , Type = (short)FileTypes.Document} }
                };

                public class ExtensionPairKeyValue1
                {
                    public short ID { get; set; }
                    public string? ExtentionName { get; set; }
                }

                public class ExtensionPairKeyValue2
                {
                    public short Type { get; set; }
                    public string? ContentType { get; set; }
                }
            }

            public static string? GetContentType(string path)
            {
                if (path.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                var types = ExtentionsList.GetMimeTypes;
                var ext = Path.GetExtension(path).ToLowerInvariant();
                return types.SingleOrDefault(w => w.Value.ContentType.ToLowerInvariant() == ext).Value?.ContentType;
            }


        }
    }

}
