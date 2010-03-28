using System.Collections.Generic;
using System.Linq;

namespace WikiTools.Access.Extensions
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> l, string separator)
        {
            return string.Join(separator, l.ToArray());
        }
    }
}