namespace MyraPad
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Returns the character at the index of string, or returns null when out of range and does not throw exception.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static char? GetCharSafely(this string @this, int index)
        {
            if (index < @this.Length && index >= 0)
            {
                return @this[index];
            }

            return null;
        }
        
        /// <summary>
        /// Returns a substring of this string, or returns null when out of range and does not throw exception.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static string SubstringSafely(this string @this, int index, int count)
        {
            if (count <= 0)
            {
                return string.Empty;
            }

            if (index < 0)
            {
                return null;
            }

            if (index >= @this.Length)
            {
                return null;
            }

            if (index + count >= @this.Length)
            {
                return null;
            }

            return @this.Substring(index, count);

        }
    }
}