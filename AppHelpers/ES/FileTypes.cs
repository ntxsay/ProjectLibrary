using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppHelpers.ES
{
    public partial class FilesHelpers
    {
        public enum FileTypes : short
        {
            Image,
            Video,
            Audio,
            Document
        }

        public struct FilesType
        {
            public static readonly Dictionary<short, string> GetFileTypes = new Dictionary<short, string>
                {
                    {(short)FileTypes.Image, "image"},
                    {(short)FileTypes.Video, "video"},
                    {(short)FileTypes.Audio, "audio"},
                    {(short)FileTypes.Document, "document"},
                };
        }
    }
}
