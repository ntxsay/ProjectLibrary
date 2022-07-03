using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppHelpers.ES
{
    public partial class FilesHelpers
    {
        public struct Extensions
        {

            public static IEnumerable<string> ImageExtensions
            {
                get => new List<string>()
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".svg",
                };
            }
        }
    }
    
}
