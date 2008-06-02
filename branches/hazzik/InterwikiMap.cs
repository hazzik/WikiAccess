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
using System.Xml;
using System.Web;

namespace WikiTools.Access
{
	/// <summary>
	/// Map of interwiki prefixes
	/// </summary>
	internal class InterwikiMap
	{
		InterwikiMapEntry[] entries;
		
		public InterwikiMap(Wiki w)
		{
			string mapxml = w.ab.DownloadPage("api.php?format=xml&action=query&meta=siteinfo&siprop=interwikimap");
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(mapxml);
			XmlNodeList nl = doc.GetElementsByTagName("iw");
			List<InterwikiMapEntry> entries_pre = new List<InterwikiMapEntry>();
			foreach( XmlNode node in nl ) {
				XmlElement elem = node as XmlElement;
				InterwikiMapEntry entry = new InterwikiMapEntry();
				entry.Prefix = elem.Attributes["prefix"].Value;
				entry.Uri = elem.Attributes["url"].Value;
				entry.Local = elem.HasAttribute("local");
				entries_pre.Add(entry);
			}
			entries = entries_pre.ToArray();
		}
		
		public InterwikiMapEntry[] Entries {
			get {
				return entries;
			}
		}
	}
	
	public struct InterwikiMapEntry {
		public string Prefix;
		public string Uri;
		public bool Local;
		
		public string FormatUri(string s) {
			return Uri.Replace( "$1", HttpUtility.UrlEncode(s) );
		}
	}
	
	partial class Wiki
	{
		InterwikiMap iwikis;
		
		public void LoadInterwikiMap()
		{
			iwikis = new InterwikiMap(this);
		}
		
		public InterwikiMapEntry[] Interwikis
		{
			get
			{
				if( iwikis == null )
					LoadInterwikiMap();
				return iwikis.Entries;
			}
		}
	}
}
