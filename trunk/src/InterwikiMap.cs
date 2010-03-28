using System.Collections.Generic;
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
            const string page = "api.php?format=xml&action=query&meta=siteinfo&siprop=interwikimap";
            var doc = new XmlDocument();
            doc.Load(w.ab.CreateGetQuery(page).GetResponseStream());
            XmlNodeList nl = doc.GetElementsByTagName("iw");
            var entries_pre = new List<InterwikiMapEntry>();
            foreach (XmlNode node in nl)
            {
                entries_pre.Add(ParseInterwikiMapEntry((XmlElement) node));
            }
            entries = entries_pre.ToArray();
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