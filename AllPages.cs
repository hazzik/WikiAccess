/**********************************************************************************
 * All pages list of WikiAccess Library                                           *
 * Copyright (C) 2007 Vasiliev V. V.                                              *
 *                                                                                *
 * This program is free software: you can redistribute it and/or modify           *
 * it under the terms of the GNU General Public License as published by           *
 * the Free Software Foundation, either version 3 of the License, or              *
 * (at your option) any later version.                                            *
 *                                                                                *
 * This program is distributed in the hope that it will be useful,                *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of                 *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                  *
 * GNU General Public License for more details.                                   *
 *                                                                                *
 * You should have received a copy of the GNU General Public License              *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>           *
 **********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WikiTools.Access
{
	/// <summary>
	/// Page type for AllPages filtering
	/// </summary>
	public enum PageTypes
	{
		/// <summary>
		/// Allow all pages
		/// </summary>
		All,
		/// <summary>
		/// Allow redirects only
		/// </summary>
		Redirects,
		/// <summary>
		/// Allow everything but redirects
		/// </summary>
		NonRedirects
	}

	partial class Wiki
	{
		/// <summary>
		/// Retrieves all pages list from wiki
		/// </summary>
		/// <param name="startfrom">Starts enumerating from this pages</param>
		/// <param name="limit">Limit of pages to get</param>
		/// <param name="filter">Redirects filter</param>
		/// <param name="namespaceID">Namespace to enumerate</param>
		/// <returns>All pages list</returns>
		public string[] GetAllPages(string startfrom, int limit, PageTypes filter, int namespaceID)
		{
			int walks_count = (int)Math.Floor((double)limit / 500), adittional_walk = limit % 500;
			string rq_uri;
			string next = startfrom;
			List<string> result = new List<string>();
			for (int i = 0; i < walks_count; i++)
			{
				rq_uri = "api.php?action=query&list=allpages&format=xml&aplimit=500&apfilterredir=" + filter.ToString().ToLower()
					+ "&apfrom=" + ab.EncodeUrl(next) + "&apnamespace=" + namespaceID;
				result.AddRange(ParseAllPages(ab.DownloadPage(rq_uri), out next));
				if (String.IsNullOrEmpty(next)) return result.ToArray();
			}
			if (adittional_walk > 0)
			{
				rq_uri = "api.php?action=query&list=allpages&format=xml&aplimit=" + adittional_walk + 
					"&apfilterredir=" + filter.ToString().ToLower()
					+ "&apfrom=" + ab.EncodeUrl(next) + "&apnamespace=" + namespaceID;
				result.AddRange(ParseAllPages(ab.DownloadPage(rq_uri), out next));
			}
			return result.ToArray();
		}

		private string[] ParseAllPages(string xml, out string next)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNodeList pgnodes = doc.GetElementsByTagName("p");
			List<string> result = new List<string>();
			foreach (XmlNode cnode in pgnodes)
			{
				XmlElement celem = (XmlElement)cnode;
				result.Add(celem.Attributes["title"].Value);
			}
			if (doc.GetElementsByTagName("query-continue").Count > 0)
			{
				XmlElement qcelem = (XmlElement)doc.GetElementsByTagName("query-continue")[0].FirstChild;
				next = qcelem.Attributes["apfrom"].Value;
			}
			else
				next = String.Empty;
			return result.ToArray();
		}

		/// <summary>
		/// Retrieves all pages list from wiki, that starts from specified prefix
		/// </summary>
		/// <param name="prefix">Prefix</param>
		/// <param name="filter">Redirects filter</param>
		/// <param name="namespaceID">Namespace to enumerate</param>
		/// <returns>All pages list</returns>
		public string[] GetPrefixIndex(string prefix, PageTypes filter, int namespaceID)
		{
			string rq_uri;
			string next = "";
			List<string> result = new List<string>();
			do
			{
				rq_uri = "api.php?action=query&list=allpages&format=xml&aplimit=500&apfilterredir=" + filter.ToString().ToLower()
					+ "&apfrom=" + ab.EncodeUrl(next) + "&apnamespace=" + namespaceID + "&apprefix=" + prefix;
				result.AddRange(ParseAllPages(ab.DownloadPage(rq_uri), out next));
			} while (!String.IsNullOrEmpty(next));
			return result.ToArray();
		}
	}
}
