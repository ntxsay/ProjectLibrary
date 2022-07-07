using AppHelpers.Strings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.Code.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter is string param)
                {
                    if (param.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        if (value is int int32)
                        {
                            return int32 > 0 ? Visibility.Visible : Visibility.Collapsed;
                        }
                        else if (value is long int64)
                        {
                            return int64 > 0 ? Visibility.Visible : Visibility.Collapsed;
                        }
                        else if (value is short int16)
                        {
                            return int16 > 0 ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    else if (param.ToLower() == "invert")
                    {
                        if (value is int int32)
                        {
                            return int32 > 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                        else if (value is long int64)
                        {
                            return int64 > 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                        else if (value is short int16)
                        {
                            return int16 > 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                    }
                }
                
                return Visibility.Collapsed;
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
