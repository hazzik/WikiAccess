/**********************************************************************************
 * Page list utils of WikiAccess Library                                          *
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WikiTools.Access
{
	/// <summary>
	/// Represents page list
	/// </summary>
	public class PageList : IEnumerable, ICloneable
	{
		#region Page list loaders

		/// <summary>
		/// Initializes page list from category
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <param name="catname">Category name</param>
		/// <param name="includeSubCategories">Include subcategories pages</param>
		/// <returns>Page list</returns>
		public static PageList FromCategory(Wiki wiki, string catname, bool includeSubCategories)
		{
			var cat = new Category(wiki, catname);
			string[] catpages = cat.Pages;
			string[] result;
			if (includeSubCategories)
			{
				string[] subcats = cat.Subcategories;
				subcats = Utils.AddPrefix(subcats, wiki.NamespacesUtils.GetNamespaceByID(Namespaces.Category) + ":");
				result = new string[catpages.Length + subcats.Length];
				subcats.CopyTo(result, 0);
				catpages.CopyTo(result, subcats.Length);
			}
			else
				result = catpages;
			return new PageList(wiki, result);
		}

		/// <summary>
		/// Initializes page list from links on page
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <param name="pagename">Page name</param>
		/// <returns>Page list</returns>
		public static PageList FromLinksOnPage(Wiki wiki, string pagename)
		{
			Page pg = wiki.GetPage(pagename);
			return new PageList(wiki, pg.InternalLinks);
		}

		/// <summary>
		/// Initializes page list from watchlist
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <returns>Page list</returns>
		public static PageList FromWatchlist(Wiki wiki)
		{
			var wl = new WatchList(wiki);
			return new PageList(wiki, wl.Pages);
		}

		/// <summary>
		/// Loads list from all pages
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <param name="startfrom">Starts enumerating from this pages</param>
		/// <param name="limit">Limit of pages to get</param>
		/// <param name="filter">Redirects filter</param>
		/// <param name="namespaceID">Namespace to enumerate</param>
		/// <returns>Page list</returns>
		public static PageList FromAllPages(Wiki wiki, string startfrom, int limit, PageTypes filter, int namespaceID)
		{
			return new PageList(wiki, wiki.GetAllPages(startfrom, limit, filter, namespaceID));
		}

		#endregion

		#region PageList class

		private readonly Namespaces _ns;
		private readonly Wiki _wiki;

		/// <summary>
		/// Initializes new instance of page list
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <param name="pages">Pages in list</param>
		public PageList(Wiki wiki, string[] pages)
		{
			Pages = pages;
			_wiki = wiki;
			_ns = wiki.NamespacesUtils;
		}

		/// <summary>
		/// Gets page in this list
		/// </summary>
		public string[] Pages { get; private set; }

		/// <summary>
		/// Convert all pages in list to talk pages
		/// </summary>
		/// <returns>Number of affected titles</returns>
		public int AllTitlesToTalk()
		{
			int count = 0;
			for (int cidx = 0; cidx < Pages.Length; cidx++)
			{
				if (!_ns.IsTalkNamespace(Pages[cidx])) count++;
				Pages[cidx] = _ns.TitleToTalk(Pages[cidx]);
			}
			return count;
		}

		/// <summary>
		/// Removes duplicates from list
		/// </summary>
		public void RemoveDuplicates()
		{
			Pages = Pages.Distinct().ToArray();
		}

		/// <summary>
		/// Convert all pages in list from talk pages to normal
		/// </summary>
		/// <returns>Number of affected titles</returns>
		public int AllTitlesFromTalk()
		{
			int count = 0;
			for (int cidx = 0; cidx < Pages.Length; cidx++)
			{
				if (_ns.IsTalkNamespace(Pages[cidx])) count++;
				Pages[cidx] = _ns.TitleFromTalk(Pages[cidx]);
			}
			return count;
		}

		/// <summary>
		/// Filters out pages from page list
		/// </summary>
		/// <param name="plf">If this delegate return false, page will be filtered out from page list</param>
		/// <returns>Count of filtered out list</returns>
		public int Filter(PageListFilter plf)
		{
			int count = 0;
			var result = new List<string>();
			foreach (string cpage in Pages)
			{
				if (plf(_wiki.GetPage(cpage)))
					result.Add(cpage);
				else
					count++;
			}
			Pages = result.ToArray();
			return count;
		}

		/// <summary>
		/// Filters out pages from page list
		/// </summary>
		/// <typeparam name="T">Type of parameter of filter delegate</typeparam>
		/// <param name="plf">If this delegate return false, page will be filtered out from page list</param>
		/// <param name="param">Parameter of plf delegate</param>
		/// <returns>Count of filtered out pages</returns>
		public int Filter<T>(ParametrizedPageListFilter<T> plf, T param)
		{
			int count = 0;
			var result = new List<string>();
			foreach (string cpage in Pages)
			{
				if (plf(_wiki.GetPage(cpage), param))
					result.Add(cpage);
				else
					count++;
			}
			Pages = result.ToArray();
			return count;
		}

		#endregion

		#region Filter

		/// <summary>
		/// Filter page list by namespaces
		/// </summary>
		/// <param name="allowedNamespaces">List of IDs of namespaces, pages in which should be kept in list</param>
		/// <returns>Count of filtered out pages</returns>
		public int FilterAllowedNamespaces(int[] allowedNamespaces)
		{
			return Filter(FilterAllowedNamespacesDelegate, allowedNamespaces);
		}

		private static bool FilterAllowedNamespacesDelegate(Page pg, int[] ns)
		{
			return Array.IndexOf(ns, pg.NamespaceID) > -1;
		}

		/// <summary>
		/// Filter page list by namespaces
		/// </summary>
		/// <param name="disallowedNamespaces">List of IDs of namespaces, pages in which should be removed from list</param>
		/// <returns>Count of filtered out pages</returns>
		public int FilterDisallowedNamespaces(int[] disallowedNamespaces)
		{
			int cnum = Pages.Length;
			return cnum - Filter(FilterDisallowedNamespacesDelegate, disallowedNamespaces);
		}

		private static bool FilterDisallowedNamespacesDelegate(Page pg, int[] ns)
		{
			return Array.IndexOf(ns, pg.NamespaceID) < 0;
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// CLones this list
		/// </summary>
		/// <returns>New copy of this list</returns>
		public object Clone()
		{
			return new PageList(_wiki, Pages);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Pages.GetEnumerator();
		}

		#endregion
	}
}