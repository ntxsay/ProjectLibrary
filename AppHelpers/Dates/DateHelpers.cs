using AppHelpers.Strings;
using System.Diagnostics;
using System.Globalization;

namespace AppHelpers.Dates
{
    public enum DateCompare : byte
    {
        /// <summary>
        /// La date comparée est suprérieur à la date Model.
        /// La date1 est plus grande que la date2
        /// </summary>
        DateSuperieur,
        /// <summary>
        /// La date comparée est égale à la date Model.
        /// La date1 est égale à la date2
        /// </summary>
        DateEgal,
        /// <summary>
        /// La date comparée est inférieur à la date Model.
        /// La date1 est plus petite que la date2
        /// </summary>
        DateInferieur,
        /// <summary>
        /// Inconnue
        /// </summary>
        Unknow
    }


    public enum DateRemainsModele
    {
        /// <summary>
        /// Il vous reste ...
        /// </summary>
        Modele1,
        /// <summary>
        /// Tant de jours restant ...
        /// </summary>
        Modele2
    }

    public static class DateTimeHelpersExtensions
    {
        //public static DateCompare CompareDate(this DateTime Date1, DateTime Date2)
        //{
        //return DateHelpers.Analysis.CompareDate(Date1, Date2);
        //}

    }

    public class DateHelpers
    {
        public const string NoAnswer = "N/A";
        public static IEnumerable<string> ChooseDays() => new List<string>()
        {
            {NoAnswer },
            {"1" },
            {"2" },
            {"3" },
            {"4" },
            {"5" },
            {"6" },
            {"7" },
            {"8" },
            {"9" },
            {"10" },
            {"11" },
            {"12" },
            {"13" },
            {"14" },
            {"15" },
            {"16" },
            {"17" },
            {"18" },
            {"19" },
            {"20" },
            {"21" },
            {"22" },
            {"23" },
            {"24" },
            {"25" },
            {"26" },
            {"27" },
            {"28" },
            {"29" },
            {"30" },
            {"31" },
        };

        public static IEnumerable<string> ChooseMonth() => new List<string>()
        {
            {NoAnswer },
            {"Janvier" },
            {"Février" },
            {"Mars" },
            {"Avril" },
            {"Mai" },
            {"Juin" },
            {"Juillet" },
            {"Août" },
            {"Septembre" },
            {"Octobre" },
            {"Novembre" },
            {"Décembre" },
        };

        public static IEnumerable<string> ChooseYear() => Enumerable.Range(1900, 130).Select(s => s.ToString());


        internal struct StringFormat
        {
            public const string FrenchDateStringFormat = "ddd dd MMMM yyyy";
            public const string FrenchDateMonthYearStringFormat = "MMMM yyyy";
            public const string FrenchDateYearStringFormat = "yyyy";
            public const string FrenchDateMinStringFormat = "dd/MM/yy";
            public const string FrenchDateMinStringFormat2 = "dd/MM/yyyy";
            public const string USADateStringFormat = "MM/dd/yyyy";
            public const string USADateStringFormat2 = "MM-dd-yyyy";
            public const string WebDateStringFormat = "yyyy-MM-dd";
            public const string WebDateTimeStringFormat = "yyyy-MM-ddTHH:mm";
            public const string FrenchDateTimeStringFormat = "dd/MM/yyyy à HH:mm";
            public const string FrenchDateTimeMinStringFormat = "dd/MM/yy à HH:mm";
            public const string FrenchFullDateTimeStringFormat2 = "dddd dd MMM yyyy à HH:mm";
            public const string FrenchFullDateTimeStringFormat = "ddd dd MMM yyyy à HH:mm";
            public const string DayFullOnlyDateStringFormat = "dddd";
            public const string MonthFullOnlyDateStringFormat = "MMMM";
            public const string YearFullOnlyDateStringFormat = "yyyy";
            public const string FrenchTimeStringFormat = "HH:mm";
            public const string DateTimeLogFormat = "yyyyMMddHHmmss";
        }

        public struct Converter
        {
            #region Nullable DateTimeOffset
            /// <summary>
            /// Convertit une chaine de caractères en <see cref="DateTimeOffset"/>
            /// </summary>
            /// <remarks>
            /// Remarques : Si <paramref name="value"/> ne peut pas être converti, null  sera retournée.
            /// </remarks>
            /// <param name="value">Chaîne de caractères à convertir en <see cref="DateTimeOffset"/></param>
            /// <returns></returns>
            public static DateTimeOffset? StringToNullableDateTimeOffset(string value)
            {
                try
                {
                    if (value.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        return null;
                    }

                    if (DateTimeOffset.TryParse(value, out DateTimeOffset nValue))
                    {
                        return nValue;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
            #endregion

            #region String
            public static string StringDateToStringDate(string value, char separator, out string? day, out string? month, out string? year, bool isMonthString = true)
            {
                try
                {
                    var splitString = value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (splitString != null && splitString.Length > 0)
                    {
                        if (splitString.Length == 1)
                        {
                            day = null;
                            month = null;
                            year = splitString[0] != "--" ? splitString[0] : null;
                            return $"--/--/{year}";
                        }
                        else if (splitString.Length == 2)
                        {
                            day = null;
                            if (isMonthString)
                            {
                                month = splitString[0] != "--" ? ChooseMonth().ToList()[Convert.ToInt32(splitString[0])] : null;
                            }
                            else
                            {
                                month = splitString[0] != "--" ? splitString[0] : null;
                            }
                            year = splitString[1];
                            return $"--/{month}/{year}";
                        }
                        else if (splitString.Length == 3)
                        {
                            day = splitString[0] != "--" ? splitString[0] : null;

                            if (isMonthString)
                            {
                                month = splitString[1] != "--" ? ChooseMonth().ToList()[Convert.ToInt32(splitString[1])] : null;
                            }
                            else
                            {
                                month = splitString[1] != "--" ? splitString[1] : null;
                            }

                            year = splitString[2] != "--" ? splitString[2] : null;

                            return $"{day ?? "--"}/{month ?? "--"}/{year ?? "--"}";
                        }
                    }

                    day = null;
                    month = null;
                    year = null;
                    return "--/--/--";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    day = null;
                    month = null;
                    year = null;
                    return "--/--/--";
                }
            }

            public static string? TimeSpanToStringDuration(TimeSpan value)
            {
                try
                {
                    if (value == TimeSpan.Zero)
                    {
                        return null;
                    }

                    var hours = (value.Hours > 0 ? value.Hours.ToString() + " h " : string.Empty);
                    var minutes = (value.Minutes > 0 ? value.Minutes.ToString() + " min " : string.Empty);
                    var seconds = (value.Seconds > 0 ? value.Seconds.ToString() + " s" : string.Empty);
                    return $"{hours}{minutes}{seconds}";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
            #endregion

            #region Nullable DateTime
            public static DateTime? GetNullableDateFromString(string? DateString)
            {
                try
                {
                    if (!DateString.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var IsDate = DateTime.TryParse(DateString, out DateTime result);
                        if (IsDate == true)
                        {
                            return result;
                        }
                        else
                        {
                            return null;
                        }
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }

            public static DateTime? GetDateFromNullableDateTimeOffSet(DateTimeOffset? dateTimeOffset)
            {
                try
                {
                    if (dateTimeOffset == null) return null;
                    return dateTimeOffset.Value.Date;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }

            #endregion

            #region DateTime
            public static DateTime GetDateFromString(string DateString)
            {
                try
                {
                    if (!DateString.IsStringNullOrEmptyOrWhiteSpace())
                    {
                        var IsDate = DateTime.TryParse(DateString, out DateTime result);
                        if (IsDate == true)
                        {
                            return result;
                        }
                    }

                    return new DateTime(1950, 01, 01);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }

            #endregion
        }
    }
}

