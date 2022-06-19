using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AppHelpers
{
	public class Logs
    {
        public static void Log(string className, string methodName, Exception exception)
        {
            try
            {
                Debug.WriteLine($"{className}.{methodName} : {exception.Message}{(exception.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + exception.InnerException?.Message) }");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(Logs)}.{nameof(Log)} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }

        public static void Log(string className, [CallerMemberName] string? methodName = null, string message = "Message sans objet")
        {
            try
            {
                Debug.WriteLine($"{className}.{methodName} : {message}");
                Console.WriteLine($"{className}.{methodName} : {message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(Logs)}.{nameof(Log)} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return;
            }
        }
    }
}

