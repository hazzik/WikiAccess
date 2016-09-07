using System.Linq;
using System.Xml;

namespace WikiTools.Access
{
    /// <summary>
    /// Map of interwiki prefixes
    /// </summary>
    internal class InterwikiMap
    {
        private readonly InterwikiMapEntry[] entries;

        public InterwikiMap(Wiki w)
        {
            const string page = Web.Query.InterwikiMapInfo;
            var doc = new XmlDocument();
            doc.Load(w.ab.HttpClient.GetStreamAsync(page).Result);
            XmlNodeList nl = doc.GetElementsByTagName("iw");
        	entries = (from XmlNode node in nl
					   select ParseInterwikiMapEntry((XmlElement) node)).ToArray();
        }

        public InterwikiMapEntry[] Entries
        {
            get { return entries; }
        }

        private static InterwikiMapEntry ParseInterwikiMapEntry(XmlElement element)
        {
            var result = new InterwikiMapEntry();
            result.Prefix = element.Attributes["prefix"].Value;
            result.Uri = element.Attributes["url"].Value;
            result.Local = element.HasAttribute("local");
            return result;
        }
    }
}