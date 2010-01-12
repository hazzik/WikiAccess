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
using System.Web;
using System.Xml;

namespace WikiTools.Access
{
	partial class Wiki
	{
		/// <summary>
		/// Retrieves all pages list from wiki
		/// </summary>
		/// <param name="startfrom">Starts enumerating from this pages</param>
		/// <param name="limit">Limit of pages to get</param>
		/// <param name="filter">Redirects filter</param>
		/// <param name="namespaceId">Namespace to enumerate</param>
		/// <returns>All pages list</returns>
		public string[] GetAllPages(string startfrom, int limit, PageTypes filter, int namespaceId)
		{
			int walksCount = (int) Math.Floor((double) limit/500), adittionalWalk = limit%500;
			string rqUri;
			string next = startfrom;
			var result = new List<string>();
			for (int i = 0; i < walksCount; i++)
			{
				rqUri = "api.php?action=query&list=allpages&format=xml&aplimit=500&apfilterredir=" + filter.ToString().ToLower()
				         + "&apfrom=" + HttpUtility.UrlEncode(next) + "&apnamespace=" + namespaceId;
				result.AddRange(ParseAllPages(ab.CreateGetQuery(rqUri).DownloadText(), out next));
				if (String.IsNullOrEmpty(next)) return result.ToArray();
			}
			if (adittionalWalk > 0)
			{
				rqUri = "api.php?action=query&list=allpages&format=xml&aplimit=" + adittionalWalk +
				         "&apfilterredir=" + filter.ToString().ToLower()
				         + "&apfrom=" + HttpUtility.UrlEncode(next) + "&apnamespace=" + namespaceId;
				result.AddRange(ParseAllPages(ab.CreateGetQuery(rqUri).DownloadText(), out next));
			}
			return result.ToArray();
		}

		private static IEnumerable<string> ParseAllPages(string xml, out string next)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNodeList pgnodes = doc.GetElementsByTagName("p");
			var result = new List<string>();
			foreach (XmlNode cnode in pgnodes)
			{
				var celem = (XmlElement) cnode;
				result.Add(celem.Attributes["title"].Value);
			}
			if (doc.GetElementsByTagName("query-continue").Count > 0)
			{
				var qcelem = (XmlElement) doc.GetElementsByTagName("query-continue")[0].FirstChild;
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
		/// <param name="namespaceId">Namespace to enumerate</param>
		/// <returns>All pages list</returns>
		public string[] GetPrefixIndex(string prefix, PageTypes filter, int namespaceId)
		{
		    string next = "";
			var result = new List<string>();
			do
			{
			    string rqUri = "api.php?action=query&list=allpages&format=xml&aplimit=500&apfilterredir=" + filter.ToString().ToLower()
			                    + "&apfrom=" + HttpUtility.UrlEncode(next) + "&apnamespace=" + namespaceId + "&apprefix=" + prefix;
			    result.AddRange(ParseAllPages(ab.CreateGetQuery(rqUri).DownloadText(), out next));
			} while (!String.IsNullOrEmpty(next));
			return result.ToArray();
		}
	}
}