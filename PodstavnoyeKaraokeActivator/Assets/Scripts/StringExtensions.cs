namespace Extensions
{
    public static class StringExtensions
    {
        public static string DeleteLastCharacter(this string target)
        {
            if (target == "")
                return "";

            return target.Substring(0, target.Length - 1);
        }
    }
}