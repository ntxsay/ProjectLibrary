using AppHelpers;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.Code.Helpers
{
    internal class DatatemplateHelpers
    {
        private const string Message = "Le datatemplate n'a pas pu charger son contenu.";

        internal async Task<T> GetUiElementFromDataTemplate<T>(DataTemplate dataTemplate, string uiElementName = null) where T : DependencyObject
        {
            try
            {
                if (dataTemplate == null)
                {
                    throw new ArgumentNullException(nameof(dataTemplate));
                }

                FrameworkElement _container = dataTemplate.LoadContent() as FrameworkElement;
                if (_container == null)
                {
                    throw new Exception(Message);
                }

                T uiElement = VisualHelpers.FindVisualChild<T>(_container, uiElementName);
                return uiElement;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(DatatemplateHelpers), exception: ex);
                return null;
            }
        }

        private async Task WaitForFrameworkElementLoaded(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null || frameworkElement.IsLoaded)
            {
                return;
            }

            while (!frameworkElement.IsLoaded)
            {
                if (frameworkElement.IsLoaded)
                {
                    break;
                }

                await Task.Delay(500);
            }
        }

        public GridView GetSelectedGridViewFromPivotTemplate(Pivot pivot, string gridViewName = "GridViewItems")
        {
            try
            {
                if (pivot == null)
                {
                    return null;
                }

                if (pivot.Items.Count == 0 || pivot.SelectedIndex < 0)
                {
                    return null;
                }

                var _container = pivot.ContainerFromItem(pivot.Items[pivot.SelectedIndex]);
                GridView gridView = VisualHelpers.FindVisualChild<GridView>(_container, gridViewName);
                if (gridView != null)
                {
                    return gridView;
                }

                return null;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(DatatemplateHelpers), exception: ex);
                return null;
            }
        }

    }
}
