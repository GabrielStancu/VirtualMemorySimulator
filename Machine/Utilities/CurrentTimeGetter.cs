using System;
using System.Globalization;

namespace Machine.Utilities
{
    /// <summary>
    /// Class that gets the current time. Used for maintaining the priority queue of the pages and frames.
    /// </summary>
    internal static class CurrentTimeGetter
    {
        /// <summary>
        /// Gets the current time in format HH:mm:ss.fff (3 decimal fractional part of the second).
        /// </summary>
        /// <returns></returns>
        internal static string GetCrtTime()
            => DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
    }
}
