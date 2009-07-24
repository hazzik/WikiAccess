/**********************************************************************************
 * Wiki class of WikiAccess Library                                               *
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
	/// <summary>
	/// Map of interwiki prefixes
	/// </summary>
	internal class InterwikiMap
	{
		private readonly InterwikiMapEntry[] entries;

		public InterwikiMap(Wiki w)
		{
			string page = "api.php?format=xml&action=query&meta=siteinfo&siprop=interwikimap";
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

	partial class Wiki
	{
		private InterwikiMap iwikis;

		public InterwikiMapEntry[] Interwikis
		{
			get
			{
				if (iwikis == null)
					LoadInterwikiMap();
				return iwikis.Entries;
			}
		}

		public void LoadInterwikiMap()
		{
			iwikis = new InterwikiMap(this);
		}
	}
}