using System.Collections.Generic;

namespace ui
{
    public static class Extensions
    {
        public static string AsString(this IEnumerable<char> chars) => string.Join(null, chars);
    }
}