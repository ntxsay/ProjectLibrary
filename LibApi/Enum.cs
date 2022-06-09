using System;
namespace LibApi
{
    public struct Search
    {
        public enum Terms
        {
            Equals,
            Contains,
            StartWith,
            EndWith,
        }

        public enum In
        {
            MainTitle,
            OtherTitle,
            Author,
            Collection,
            Editor,
        }

        public const string TermContains = "Contient";
        public const string TermEquals = "Correspond à";
        public const string TermStartWith = "Commence par";
        public const string TermEndWith = "Se termine par";
        public static IEnumerable<string> SearchOnList => new List<string>()
                {
                    TermContains,
                    TermEquals,
                    TermStartWith,
                    TermEndWith,
                };

        public static Dictionary<byte, string> SearchOnListDictionary => new Dictionary<byte, string>()
                {
                    {(byte)Terms.Contains, TermContains },
                    {(byte)Terms.Equals, TermEquals },
                    {(byte)Terms.StartWith, TermStartWith },
                    {(byte)Terms.EndWith, TermEndWith },
                };
    }
}

