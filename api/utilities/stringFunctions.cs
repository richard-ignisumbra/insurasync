using System;
namespace utilities
{
    public static class MyExtensions
    {
        /// <summary>
        /// Returns the left part of this string instance.
        /// </summary>
        /// <param name="count">Number of characters to return.</param>
        public static string Left(this string input, int count)
        {
            if (!String.IsNullOrEmpty(input))
            {
                return input.Substring(0, Math.Min(input.Length, count));
            }
            else
            {
                return null;
            }

        }
    }
}