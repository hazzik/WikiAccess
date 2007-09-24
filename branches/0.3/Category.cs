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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace WikiTools.Access
{
	/// <summary>
	/// Represents a category in wiki
	/// </summary>
	public class Category
	{
		Wiki wiki;
		string name;
		AccessBrowser ab;

		bool loaded;
		string[] subcats;
		string[] pagesincat;

		/// <summary>
		/// Initializes new instance of category class
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <param name="name">Name of the categry</param>
		public Category(Wiki wiki, string name)
		{
			this.wiki = wiki;
			this.name = name;
			this.ab = wiki.ab;
		}

		/// <summary>
		/// Loads category content
		/// </summary>
		public void Load()
		{
			ab.PageName = "api.php?action=query&format=xml&list=categorymembers&cmlimit=500&cmcategory=" + ab.EncodeUrl(name);
			List<string> subcats_tmp = new List<string>();
			List<string> pages_tmp = new List<string>();
			string cmcontinue;
			do
			{
				string[] cur_subcats, cur_pages;
				cmcontinue = ExtractCategoriesFromXML(ab.PageText, out cur_subcats, out cur_pages);
				subcats_tmp.AddRange(cur_subcats);
				pages_tmp.AddRange(cur_pages);
				if (!String.IsNullOrEmpty(cmcontinue))
					ab.PageName = "api.php?action=query&format=xml&list=categorymembers&cmlimit=500&cmcategory=" + ab.EncodeUrl(name)
						+ "&cmcontinue=" + ab.EncodeUrl(cmcontinue);
				else break;
			} while (true);
			loaded = true;
			subcats = subcats_tmp.ToArray();
			pagesincat = pages_tmp.ToArray();
		}

		private string ExtractCategoriesFromXML(string xml, out string[] subcats, out string[] pages)
		{
			List<string> subcats_tmp = new List<string>();
			List<string> pages_tmp = new List<string>();
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNodeList cmnodes = doc.GetElementsByTagName("cm");
			foreach (XmlNode cnode in cmnodes)
			{
				XmlElement celem = (XmlElement)cnode;
				if (celem.Attributes["ns"].Value == "14")
					subcats_tmp.Add(wiki.NamespacesUtils.RemoveNamespace(celem.Attributes["title"].Value));
				else
					pages_tmp.Add(celem.Attributes["title"].Value);
			}
			subcats = subcats_tmp.ToArray();
			pages = pages_tmp.ToArray();
			if (doc.GetElementsByTagName("query-continue").Count <= 0) return null;
			else
			{
				XmlElement elem = (XmlElement)doc.GetElementsByTagName("query-continue")[0].FirstChild;
				return elem.Attributes["cmcontinue"].Value;
			}
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
			if (removeDuplicates)
				return Utils.RemoveDuplicates(GetPagesRecursive(null));
			else
				return GetPagesRecursive(null);
		}

		private string[] GetPagesRecursive(List<string> _passed)
		{
			List<string> passed = (_passed == null ? new List<string>() : _passed);
			if (!passed.Contains(name)) passed.Add(name);
			List<string> result = new List<string>(Pages);
			foreach (string subcat in Subcategories)
			{
				if (passed.Contains(subcat)) continue;
				Category csubcat = new Category(wiki, subcat);
				result.AddRange(csubcat.GetPagesRecursive(passed));
			}
			return result.ToArray();
		}

		/// <summary>
		/// Gets category page
		/// </summary>
		public Page CategoryPage
		{
			get
			{
				return new Page(wiki, "Category:" + name);
			}
		}

		/// <summary>
		/// Checks if category has its page (if no, it is a wanted category)
		/// </summary>
		public bool HasCategoryPage
		{
			get
			{
				return CategoryPage.Exists;
			}
		}
	}
}
