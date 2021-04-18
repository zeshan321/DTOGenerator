namespace DTOGenerator.Extension
{
    public static class StringExtensionMethods
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string GetLastPart(this string text, string split)
        {
            var lastIndex = text.LastIndexOf(split);
            if (lastIndex + split.Length < text.Length)
            {
                return text.Substring(lastIndex + split.Length);
            }

            return null;
        }
    }
}