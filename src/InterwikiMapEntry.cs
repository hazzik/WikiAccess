using System.Web;

namespace WikiTools.Access
{
    public struct InterwikiMapEntry
    {
        public bool Local;
        public string Prefix;
        public string Uri;

        public string FormatUri(string s)
        {
            return Uri.Replace("$1", HttpUtility.UrlEncode(s));
        }
    }
}