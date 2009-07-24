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
using System.Text.RegularExpressions;

namespace WikiTools.Access
{
	/// <summary>
	/// Provides access to watch list
	/// </summary>
	public class WatchList
	{
		private readonly Wiki wiki;

		private string[] contents;
		private bool contentsLoaded;

		/// <summary>
		/// Initializes new instance of WatchList class
		/// </summary>
		/// <param name="site">Wiki, from which you need access to watch list</param>
		public WatchList(Wiki site)
		{
			wiki = site;
		}

		private AccessBrowser ab
		{
			get { return wiki.ab; }
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
		/// Loads watchlist
		/// </summary>
		public void LoadPages()
		{
			string resp = ab.CreateGetQuery("index.php?title=Special:Watchlist/edit").DownloadText();
			MatchCollection mc = Regex.Matches(resp, "<input type=\"checkbox\" name=\"id\\[\\]\" value=\"(.*?)\" />",
			                                   RegexOptions.IgnoreCase);
			var result = new List<string>();
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
		/// Add pages to watchlist
		/// </summary>
		/// <param name="page">Page name</param>
		public void Add(string page)
		{
			Page pg = wiki.GetPage(page);
			pg.Watch();
		}

		/// <summary>
		/// Removes page from watchlist
		/// </summary>
		/// <param name="page">Page name</param>
		public void Remove(string page)
		{
			Page pg = wiki.GetPage(page);
			pg.Unwatch();
		}
	}
}