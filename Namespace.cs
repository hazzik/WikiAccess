/**********************************************************************************
 * Namespace utils of WikiAccess Library                                          *
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
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiTools.Access
{
	/// <summary>
	/// Namespace utils
	/// </summary>
	public class Namespaces
	{
		static Regex NamespaceFromMeta = new Regex("<ns id=\"(-?\\d{1,3})\">(.+?)</ns>", RegexOptions.Compiled);

		/// <summary>
		/// Media namesapce
		/// </summary>
		public static readonly int Media = -2;
		/// <summary>
		/// Special namespace, where specail pages are stored
		/// </summary>
		public static readonly int Special = -1;
		/// <summary>
		/// Main (article) namespace
		/// </summary>
		public static readonly int Main = 0;
		/// <summary>
		/// Talk for mainspace pages
		/// </summary>
		public static readonly int Talk = 1;
		/// <summary>
		/// Users' personal pages
		/// </summary>
		public static readonly int User = 2;
		/// <summary>
		/// Users' talk pages
		/// </summary>
		public static readonly int UserTalk = 3;
		/// <summary>
		/// Project pages
		/// </summary>
		public static readonly int Project = 4;
		/// <summary>
		/// Talk for project pages
		/// </summary>
		public static readonly int ProjectTalk = 5;
		/// <summary>
		/// Media files
		/// </summary>
		public static readonly int Image = 6;
		/// <summary>
		/// Media files' talk pages
		/// </summary>
		public static readonly int ImageTalk = 7;
		/// <summary>
		/// MediaWiki messages
		/// </summary>
		public static readonly int MediaWiki = 8;
		/// <summary>
		/// MediaWiki messages talk pages
		/// </summary>
		public static readonly int MediaWikiTalk = 9;
		/// <summary>
		/// Templates
		/// </summary>
		public static readonly int Template = 10;
		/// <summary>
		/// Templates' talk pages
		/// </summary>
		public static readonly int TemplateTalk = 11;
		/// <summary>
		/// Help
		/// </summary>
		public static readonly int Help = 12;
		/// <summary>
		/// Help's talk pages
		/// </summary>
		public static readonly int HelpTalk = 13;
		/// <summary>
		/// Categories
		/// </summary>
		public static readonly int Category = 14;
		/// <summary>
		/// Categories' talk pages
		/// </summary>
		public static readonly int CategoryTalk = 15;

		#region Load and save

		/// <summary>
		/// Loads namespaces from live wiki
		/// </summary>
		/// <param name="wiki">Source of namespaces</param>
		/// <returns>Namespace ID:Name list</returns>
		public static SortedList<int, string> GetNamespaces(Wiki wiki)
		{
			string uri = wiki.WikiURI + "/api.php?action=query&meta=siteinfo&siprop=namespaces&format=xml";
			WebRequest rq = WebRequest.Create(uri);
			string str = new StreamReader(rq.GetResponse().GetResponseStream(), Encoding.UTF8).ReadToEnd();
			SortedList<int, string> result = new SortedList<int, string>();
			result.Add(0, "");
			MatchCollection matches = NamespaceFromMeta.Matches(str);
			foreach (Match match in matches)
			{
				result.Add(Int32.Parse(match.Groups[1].Value), match.Groups[2].Value);
			}
			return result;
		}

		/// <summary>
		/// Loads namespaces from file
		/// </summary>
		/// <param name="fname">File name</param>
		/// <returns>Namespace ID:Name list</returns>
		public static SortedList<int, string> LoadFromFile(string fname)
		{
			string[] lines = File.ReadAllLines(fname, Encoding.UTF8);
			SortedList<int, string> result = new SortedList<int, string>();
			foreach (string cline in lines)
			{
				string[] parts = cline.Split(':');
				result.Add(Int32.Parse(parts[0]), parts[1]);
			}
			return result;
		}

		/// <summary>
		/// Saves namespaces to file
		/// </summary>
		/// <param name="fname">File name</param>
		/// <param name="ns">Namespaces list</param>
		public static void SaveToFile(string fname, SortedList<int, string> ns)
		{
			List<string> result = new List<string>();
			foreach (KeyValuePair<int, string> ckp in ns)
				result.Add(ckp.Key + ":" + ckp.Value);
			File.WriteAllLines(fname, result.ToArray(), Encoding.UTF8);
		}

		/// <summary>
		/// Makes name for cache file
		/// </summary>
		/// <param name="uri">URI of wiki</param>
		/// <returns>File name</returns>
		public static string MkName(string uri)
		{
			return (new Uri(uri).Host) + ".namespaces";
		}

		#endregion
		
		/// <summary>
		/// Gets canonical namespaces list
		/// </summary>
		/// <returns>Canonical namespaces list</returns>
		public static SortedList<int, string> GetStandardNamespaces()
		{
			SortedList<int, string> result = new SortedList<int, string>();
			result.Add(-2, "Media");
			result.Add(-1, "Special");
			result.Add(0, "");
			result.Add(1, "Talk");
			result.Add(2, "User");
			result.Add(3, "User talk");
			result.Add(4, "Project");
			result.Add(5, "Project talk");
			result.Add(6, "Image");
			result.Add(7, "Image talk");
			result.Add(8, "MediaWiki");
			result.Add(9, "MediaWiki talk");
			result.Add(10, "Template");
			result.Add(11, "Template talk");
			result.Add(12, "Help");
			result.Add(13, "Help talk");
			result.Add(14, "Category");
			result.Add(15, "Category talk");
			return result;
		}

		SortedList<int, string> namespaces;

		/// <summary>
		/// Initializes new instance of object from file
		/// </summary>
		/// <param name="fpath">File name</param>
		public Namespaces(string fpath)
		{
			namespaces = Namespaces.LoadFromFile(fpath);
		}

		/// <summary>
		/// Initializes new instance of object from live wiki
		/// </summary>
		/// <param name="wiki">Wiki</param>
		public Namespaces(Wiki wiki)
		{
			namespaces = Namespaces.GetNamespaces(wiki);
		}

		/// <summary>
		/// Gets IF of specified namespace
		/// </summary>
		/// <param name="nsName"></param>
		/// <returns></returns>
		public int GetNamespaceID(string nsName)
		{
			if (namespaces.ContainsValue(nsName))
				return namespaces.Keys[namespaces.Values.IndexOf(nsName)];
			else if (GetStandardNamespaces().ContainsValue(nsName))
				return GetStandardNamespaces().Keys[GetStandardNamespaces().Values.IndexOf(nsName)];
			else return 0;
		}

		/// <summary>
		/// Extracts namespace from title
		/// </summary>
		/// <param name="title">Page title</param>
		/// <returns>Namespace ID</returns>
		public int GetNamespaceByTitle(string title)
		{
			return GetNamespaceID(title.Split(':')[0]);
		}

		/// <summary>
		/// Saves namespaces to file
		/// </summary>
		/// <param name="fname">File name</param>
		public void SaveToFile(string fname)
		{
			Namespaces.SaveToFile(fname, namespaces);
		}

		/// <summary>
		/// Gets namespace by ID
		/// </summary>
		/// <param name="ID">Namespace ID</param>
		/// <returns>Namespace name</returns>
		public string GetNamespaceByID(int ID)
		{
			return namespaces[ID];
		}

		/// <summary>
		/// Get talk title of specified page
		/// </summary>
		/// <param name="title">Page title</param>
		/// <returns>Talk title</returns>
		public string TitleToTalk(string title)
		{
			int nid = GetNamespaceByTitle(title);
			if (IsTalkNamespace(title) | nid < 0) return title;
			if (nid == 0) return namespaces[1] + ":" + title;
			else if (title.StartsWith(namespaces[nid]))
				return namespaces[nid + 1] + title.Substring(title.IndexOf(":"));
			else
				return GetStandardNamespaces()[nid + 1] + title.Substring(title.IndexOf(":"));
		}

		/// <summary>
		/// Get page title of specified talk page
		/// </summary>
		/// <param name="title">Talk title</param>
		/// <returns>Page title</returns>
		public string TitleFromTalk(string title)
		{
			if (!IsTalkNamespace(title)) return title;
			int nid = GetNamespaceByTitle(title);
			if (nid == 1) return title.Substring(title.IndexOf(":") + 1);
			else if (title.StartsWith(namespaces[nid]))
				return namespaces[nid - 1] + title.Substring(title.IndexOf(":"));
			else
				return GetStandardNamespaces()[nid - 1] + title.Substring(title.IndexOf(":"));
		}

		/// <summary>
		/// Checks if page is in talk namespace
		/// </summary>
		/// <param name="title">Page title</param>
		/// <returns>Is talk namespace</returns>
		public bool IsTalkNamespace(string title)
		{
			return GetNamespaceByTitle(title) > 0 && GetNamespaceByTitle(title) % 2 == 1;
		}

		/// <summary>
		/// Removes namespace prefix from 
		/// </summary>
		/// <param name="pgname">Page name</param>
		/// <returns>Page name without namespace</returns>
		public string RemoveNamespace(string pgname)
		{
			pgname = pgname.Trim();
			int ns = GetNamespaceByTitle(pgname);
			if (ns == 0) return pgname;
			else if (pgname.StartsWith(namespaces[ns])) return pgname.Substring(namespaces[ns].Length + 1);
			else return pgname.Substring(GetStandardNamespaces()[ns].Length + 1);
		}
	}
}
