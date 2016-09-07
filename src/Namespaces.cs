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
using System.Linq;
using System.Text;
using System.Xml.XPath;
using WikiTools.Web;

namespace WikiTools.Access
{
	/// <summary>
	/// Namespace utils
	/// </summary>
	public class Namespaces
	{
		/// <summary>
		/// Categories
		/// </summary>
		public const int Category = 14;

		/// <summary>
		/// Categories' talk pages
		/// </summary>
		public const int CategoryTalk = 15;

		/// <summary>
		/// Help
		/// </summary>
		public const int Help = 12;

		/// <summary>
		/// Help's talk pages
		/// </summary>
		public const int HelpTalk = 13;

		/// <summary>
		/// Media files
		/// </summary>
		public const int Image = 6;

		/// <summary>
		/// Media files' talk pages
		/// </summary>
		public const int ImageTalk = 7;

		/// <summary>
		/// Main (article) namespace
		/// </summary>
		public const int Main = 0;

		/// <summary>
		/// Media namesapce
		/// </summary>
		public const int Media = -2;

		/// <summary>
		/// MediaWiki messages
		/// </summary>
		public const int MediaWiki = 8;

		/// <summary>
		/// MediaWiki messages talk pages
		/// </summary>
		public const int MediaWikiTalk = 9;

		/// <summary>
		/// Project pages
		/// </summary>
		public const int Project = 4;

		/// <summary>
		/// Talk for project pages
		/// </summary>
		public const int ProjectTalk = 5;

		/// <summary>
		/// Special namespace, where specail pages are stored
		/// </summary>
		public const int Special = -1;

		/// <summary>
		/// Talk for mainspace pages
		/// </summary>
		public const int Talk = 1;

		/// <summary>
		/// Templates
		/// </summary>
		public const int Template = 10;

		/// <summary>
		/// Templates' talk pages
		/// </summary>
		public const int TemplateTalk = 11;

		/// <summary>
		/// Users' personal pages
		/// </summary>
		public const int User = 2;

		/// <summary>
		/// Users' talk pages
		/// </summary>
		public const int UserTalk = 3;

		private readonly SortedList<int, string> _namespaces;

		#region Load and save

		/// <summary>
		/// Loads namespaces from live wiki
		/// </summary>
		/// <param name="wiki">Source of namespaces</param>
		/// <returns>Namespace ID:Name list</returns>
		public static SortedList<int, string> GetNamespaces(Wiki wiki)
		{
		    var response = wiki.ab.HttpClient.GetStreamAsync(Query.Namespaces).Result;
		    var xdoc = new XPathDocument(response);
			var result = new SortedList<int, string>();
			foreach (XPathNavigator element in xdoc.CreateNavigator().Select("api/query/namespaces/ns"))
			{
				result.Add(int.Parse(element.GetAttribute("id", "")), element.Value);
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
			var result = new SortedList<int, string>();
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
			File.WriteAllLines(fname, ns.Select(ckp => ckp.Key + ":" + ckp.Value).ToArray(), Encoding.UTF8);
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
		/// Initializes new instance of object from file
		/// </summary>
		/// <param name="fpath">File name</param>
		public Namespaces(string fpath)
		{
			_namespaces = LoadFromFile(fpath);
		}

		/// <summary>
		/// Initializes new instance of object from live wiki
		/// </summary>
		/// <param name="wiki">Wiki</param>
		public Namespaces(Wiki wiki)
		{
			_namespaces = GetNamespaces(wiki);
		}

		/// <summary>
		/// Gets canonical namespaces list
		/// </summary>
		/// <returns>Canonical namespaces list</returns>
		public static SortedList<int, string> GetStandardNamespaces()
		{
			var result = new SortedList<int, string>();
			result.Add(Media, "Media");
			result.Add(Special, "Special");
			result.Add(Main, "");
			result.Add(Talk, "Talk");
			result.Add(User, "User");
			result.Add(UserTalk, "User talk");
			result.Add(Project, "Project");
			result.Add(ProjectTalk, "Project talk");
			result.Add(Image, "Image");
			result.Add(ImageTalk, "Image talk");
			result.Add(MediaWiki, "MediaWiki");
			result.Add(MediaWikiTalk, "MediaWiki talk");
			result.Add(Template, "Template");
			result.Add(TemplateTalk, "Template talk");
			result.Add(Help, "Help");
			result.Add(HelpTalk, "Help talk");
			result.Add(Category, "Category");
			result.Add(CategoryTalk, "Category talk");
			return result;
		}

		/// <summary>
		/// Gets IF of specified namespace
		/// </summary>
		/// <param name="nsName"></param>
		/// <returns></returns>
		public int GetNamespaceID(string nsName)
		{
			if (_namespaces.ContainsValue(nsName))
				return _namespaces.Keys[_namespaces.Values.IndexOf(nsName)];
			if (GetStandardNamespaces().ContainsValue(nsName))
				return GetStandardNamespaces().Keys[GetStandardNamespaces().Values.IndexOf(nsName)];
			return 0;
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
			SaveToFile(fname, _namespaces);
		}

		/// <summary>
		/// Gets namespace by ID
		/// </summary>
		/// <param name="ID">Namespace ID</param>
		/// <returns>Namespace name</returns>
		public string GetNamespaceByID(int ID)
		{
			return _namespaces[ID];
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
			if (nid == 0) return _namespaces[1] + ":" + title;
			if (title.StartsWith(_namespaces[nid]))
				return _namespaces[nid + 1] + title.Substring(title.IndexOf(":"));
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
			if (title.StartsWith(_namespaces[nid]))
				return _namespaces[nid - 1] + title.Substring(title.IndexOf(":"));
			return GetStandardNamespaces()[nid - 1] + title.Substring(title.IndexOf(":"));
		}

		/// <summary>
		/// Checks if page is in talk namespace
		/// </summary>
		/// <param name="title">Page title</param>
		/// <returns>Is talk namespace</returns>
		public bool IsTalkNamespace(string title)
		{
			return GetNamespaceByTitle(title) > 0 && GetNamespaceByTitle(title)%2 == 1;
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
			if (pgname.StartsWith(_namespaces[ns])) return pgname.Substring(_namespaces[ns].Length + 1);
			return pgname.Substring(GetStandardNamespaces()[ns].Length + 1);
		}
	}
}