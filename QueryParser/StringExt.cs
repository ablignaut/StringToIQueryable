
namespace QueryParser
{
    public static class StringExt
    {
        public static string SeparateAt(this string @string, char until, out string rest)
        {
            int indexOfChar = @string.IndexOf(until);

            if (indexOfChar < 0)
            {
                rest = string.Empty;
                return @string;
            }

            rest = @string.Substring(indexOfChar, @string.Length - indexOfChar);
            return @string.Substring(0, indexOfChar);
        }

        public static string SeparateAt(this string @string, char until)
        {
            string rest; // unused
            return @string.SeparateAt(until, out rest);
        }
    }
}
