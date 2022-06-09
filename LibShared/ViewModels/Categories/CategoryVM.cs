using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibShared.ViewModels.Categories
{
    public class CategoryVM : GenericVM
    {
        public long IdLibrary { get; protected set; } = -1;
        public long? IdParentCategory { get; protected set; } = null;

    }
}
