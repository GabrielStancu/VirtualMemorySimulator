using System;
using System.Globalization;

namespace Machine.Utilities
{
    internal static class CurrentTimeGetter
    {
        internal static string GetCrtTime()
            => DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
    }
}
