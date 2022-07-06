using AppHelpers;
using LibShared.ViewModels;
using LibShared.ViewModels.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibShared.Services
{
    public class CheckModificationsServices
    {
        public static IEnumerable<PropertiesChangedVM> GetPropertiesChanged(LibraryVM viewModelA, LibraryVM viewModelB)
        {
            try
            {
                if (viewModelA == null) return Enumerable.Empty<PropertiesChangedVM>();
                if (viewModelB == null) return Enumerable.Empty<PropertiesChangedVM>();

                List<PropertiesChangedVM> list = new ();

                if (viewModelA.Name != viewModelB.Name)
                {
                    list.Add(new PropertiesChangedVM()
                    {
                        PropertyName = "Nom de la bibliothèque",
                        Message = "Le nom de la bibliothèque a été changé"
                    });
                }

                if (viewModelA.Description != viewModelB.Description)
                {
                    list.Add(new PropertiesChangedVM()
                    {
                        PropertyName = "Description de la bibliothèque",
                        Message = "La description de la bibliothèque a été changé"
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                Logs.Log(className: nameof(CheckModificationsServices), exception: ex);
                return Enumerable.Empty<PropertiesChangedVM>();
            }
        }

    }
}
