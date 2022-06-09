using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibShared.ViewModels.Categories
{
    public class CategoryVM : GenericVM
    {
        public long IdLibrary { get; set; } = -1;
        public long? IdParentCategory { get; set; } = null;

        protected ObservableCollection<CategoryVM> _SubCategories = new ObservableCollection<CategoryVM>();
        public ObservableCollection<CategoryVM> SubCategories
        {
            get => _SubCategories;
            set
            {
                if (_SubCategories != value)
                {
                    _SubCategories = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
