/**********************************************************************************
 * Category class of WikiAccess Library                                           *
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
using System.Linq;
using System.Web;
using System.Xml;

namespace WikiTools.Access
{
	/// <summary>
	/// Represents a category in wiki
	/// </summary>
	public class Category
	{
		private bool loaded;
		private readonly string name;
		private string[] pagesincat;
		private string[] subcats;
		private readonly Wiki wiki;

		/// <summary>
		/// Initializes new instance of category class
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <param name="name">Name of the categry</param>
		public Category(Wiki wiki, string name)
		{
			this.wiki = wiki;
			this.name = name;
		}

		/// <summary>
		/// Gets subcategories.  Automatically calls Load() on first usage
		/// </summary>
		public string[] Subcategories
		{
			get
			{
				if (!loaded)
					Load();
				return subcats;
			}
		}

		/// <summary>
		/// Gets pages in it.  Automatically calls Load() on first usage
		/// </summary>
		public string[] Pages
		{
			get
			{
				if (!loaded)
					Load();
				return pagesincat;
			}
		}

		/// <summary>
		/// Gets category page
		/// </summary>
		public Page CategoryPage
		{
			get { return wiki.GetPage("Category:" + name); }
		}

		/// <summary>
		/// Checks if category has its page (if no, it is a wanted category)
		/// </summary>
		public bool HasCategoryPage
		{
			get { return CategoryPage.Exists; }
		}

		/// <summary>
		/// Loads category content
		/// </summary>
		public void Load()
		{
			string pgname = "api.php?action=query&format=xml&list=categorymembers&cmlimit=500&cmcategory=" +
			                HttpUtility.UrlEncode(name);
			string text = wiki.ab.CreateGetQuery(pgname).DownloadText();
			var subcatsTmp = new List<string>();
			var pagesTmp = new List<string>();
		    do
			{
				string[] curSubcats, curPages;
				string cmcontinue = ExtractCategoriesFromXML(text, out curSubcats, out curPages);
				subcatsTmp.AddRange(curSubcats);
				pagesTmp.AddRange(curPages);
				if (!String.IsNullOrEmpty(cmcontinue))
				{
					string pgname1 = "api.php?action=query&format=xml&list=categorymembers&cmlimit=500&cmcategory=" +
					                 HttpUtility.UrlEncode(name) + "&cmcontinue=" + HttpUtility.UrlEncode(cmcontinue);
					text = wiki.ab.CreateGetQuery(pgname1).DownloadText();
				}
				else break;
			} while (true);
			loaded = true;
			subcats = subcatsTmp.ToArray();
			pagesincat = pagesTmp.ToArray();
		}

		private string ExtractCategoriesFromXML(string xml, out string[] subcats, out string[] pages)
		{
			var subcatsTmp = new List<string>();
			var pagesTmp = new List<string>();
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNodeList cmnodes = doc.GetElementsByTagName("cm");
			foreach (XmlNode cnode in cmnodes)
			{
				var celem = (XmlElement) cnode;
				if (celem.Attributes["ns"].Value == Namespaces.Category.ToString())
					subcatsTmp.Add(wiki.NamespacesUtils.RemoveNamespace(celem.Attributes["title"].Value));
				else
					pagesTmp.Add(celem.Attributes["title"].Value);
			}
			subcats = subcatsTmp.ToArray();
			pages = pagesTmp.ToArray();
			if (doc.GetElementsByTagName("query-continue").Count <= 0) return null;
			var elem = (XmlElement) doc.GetElementsByTagName("query-continue")[0].FirstChild;
			return elem.Attributes["cmcontinue"].Value;
		}

		/// <summary>
		/// Loads page in this category and all subcategories
		/// </summary>
		/// <returns>Pages</returns>
		public string[] GetPagesRecursive()
		{
			return GetPagesRecursive(true);
		}

		/// <summary>
		/// Loads page in this category and all subcategories
		/// </summary>
		/// <param name="removeDuplicates">Remove duplicates from list</param>
		/// <returns>Pages</returns>
		public string[] GetPagesRecursive(bool removeDuplicates)
		{
		    var pagesRecursive = GetPagesRecursive(null);
		    if (removeDuplicates)
		        return pagesRecursive.Distinct().ToArray();
		    return pagesRecursive;
		}

	    private string[] GetPagesRecursive(List<string> _passed)
		{
			List<string> passed = (_passed ?? new List<string>());
		    if (passed.Contains(name) == false)
		        passed.Add(name);

		    var selectMany = Subcategories
		        .Where(subcat => !passed.Contains(subcat))
		        .Select(subcat => new Category(wiki, subcat))
		        .SelectMany(csubcat => csubcat.GetPagesRecursive(passed));
		    var result = new List<string>(Pages);
		    result.AddRange(selectMany);
		    return result.ToArray();
		}
	}
}