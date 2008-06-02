/**********************************************************************************
 * Wtchlist class of WikiAccess Library                                           *
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
using System.Reflection;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WikiTools.Access
{
	/// <summary>
	/// Provides access to watch list
	/// </summary>
	public class WatchList
	{
		Wiki wiki;
		AccessBrowser ab;

		string[] contents; bool contentsLoaded = false;
		
		/// <summary>
		/// Initializes new instance of WatchList class
		/// </summary>
		/// <param name="site">Wiki, from which you need access to watch list</param>
		public WatchList(Wiki site)
		{
			wiki = site;
			ab = wiki.ab;
		}

		/// <summary>
		/// Loads watchlist
		/// </summary>
		public void LoadPages()
		{
			ab.PageName = "index.php?title=Special:Watchlist/edit";
			string resp = ab.PageText;
			MatchCollection mc = Regex.Matches(resp, "<input type=\"checkbox\" name=\"id\\[\\]\" value=\"(.*?)\" />", RegexOptions.IgnoreCase);
			List<String> result = new List<string>();
			foreach (Match cmatch in mc)
			{
				int startIdx, endIdx;
				startIdx = cmatch.Value.IndexOf("value=") + 7;
				endIdx = cmatch.Value.IndexOf('"', startIdx);
				result.Add(cmatch.Groups[0].Value.Substring(startIdx, endIdx - startIdx));
			}
			contents = result.ToArray();
			contentsLoaded = true;
		}

		/// <summary>
		/// Gets page in watch list
		/// </summary>
		public string[] Pages
		{
			get
			{
				if (!contentsLoaded)
					LoadPages();
				return contents;
			}
		}

		/// <summary>
		/// Add pages to watchlist
		/// </summary>
		/// <param name="page">Page name</param>
		public void Add(string page)
		{
			Page pg = new Page(wiki, page);
			pg.Watch();
		}

		/// <summary>
		/// Removes page from watchlist
		/// </summary>
		/// <param name="page">Page name</param>
		public void Remove(string page)
		{
			Page pg = new Page(wiki, page);
			pg.Unwatch();
		}
	}
}
