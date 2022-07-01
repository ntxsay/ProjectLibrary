using LibraryWinUI.Code.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.Code.DaraTemplateSelectors
{
    internal class DataViewModeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GridViewTemplate { get; set; }
        public DataTemplate DataGridViewTemplate { get; set; }


        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            try
            {

                var _Container = VisualHelpers.FindVisualAncestor<Pivot>(container);
                if (_Container != null && _Container.Tag is DataViewMode dataViewMode)
                {
                    if (dataViewMode == DataViewMode.DataGridView)
                    {
                        return DataGridViewTemplate;
                    }
                    else if (dataViewMode == DataViewMode.GridView)
                    {
                        return GridViewTemplate;
                    }
                }

                return base.SelectTemplateCore(item);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
