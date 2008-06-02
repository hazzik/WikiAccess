/**********************************************************************************
 * Message cache of WikiAccess Library                                            *
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
using System.Text;
using System.Text.RegularExpressions;

namespace WikiTools.Access
{
	/// <summary>
	/// Message cache
	/// </summary>
	public class MessageCache
	{
		string mcachetext;
		string[] months, months_gen;

		/// <summary>
		/// Returns message cache messages
		/// </summary>
		/// <param name="str">Message name</param>
		/// <returns>Message contents</returns>
		public string this[string str]
		{
			get
			{
				return GetMessage(str);
			}
		}

		/// <summary>
		/// Loads message cache from files
		/// </summary>
		/// <param name="fname">File name</param>
		public MessageCache(string fname)
		{
			mcachetext = File.ReadAllText(fname);
			LoadMonths();
		}

		/// <summary>
		/// Loads message cache from live wiki
		/// </summary>
		/// <param name="wiki">Wiki to load</param>
		public MessageCache(Wiki wiki)
		{
			AccessBrowser ab = wiki.ab;
			ab.PageName = "index.php?title=Special:Allmessages&ot=php";
			mcachetext = ab.PageText;
			LoadMonths();
		}

		/// <summary>
		/// Makes file name for message cache file
		/// </summary>
		/// <param name="uri">URI of wiki</param>
		/// <returns>File name</returns>
		public static string MkName(string uri)
		{
			return (new Uri(uri).Host) + ".messages";
		}

		/// <summary>
		/// Saves message cache to file
		/// </summary>
		/// <param name="fname">File name to save</param>
		/// <returns>Success</returns>
		public bool SaveToFile(string fname)
		{
			try
			{
				File.WriteAllText(fname, mcachetext, Encoding.Unicode);
				return true;
			}
			catch { return false; }
		}

		/// <summary>
		/// Gets message from message cache
		/// </summary>
		/// <param name="name">Name of message</param>
		/// <returns>Message content</returns>
		public string GetMessage(string name)
		{
			Regex regex = new Regex(@"'" + Regex.Escape(name) + @"' =&gt; '([^\0]+?[^\\])',", RegexOptions.Singleline & RegexOptions.IgnoreCase);
			if (!regex.Match(mcachetext).Success) return null;
			string str = regex.Match(mcachetext).Groups[1].Value;
			while (Regex.Matches(str, @"([^\\])(\\')").Count > 0) str = Regex.Replace(str, @"([^\\])(\\')", "$1'");
			while (Regex.Matches(str, @"&lt;").Count > 0) str = Regex.Replace(str, @"&lt;", "$1'");
			while (Regex.Matches(str, @"&gt;").Count > 0) str = Regex.Replace(str, @"&gt;", "$1'");
			return str;
		}

		private void LoadMonths()
		{
			List<string> results = new List<string>();
			results.Add(GetMessage("january"));
			results.Add(GetMessage("february"));
			results.Add(GetMessage("march"));
			results.Add(GetMessage("april"));
			results.Add(GetMessage("may"));
			results.Add(GetMessage("june"));
			results.Add(GetMessage("july"));
			results.Add(GetMessage("august"));
			results.Add(GetMessage("september"));
			results.Add(GetMessage("october"));
			results.Add(GetMessage("november"));
			results.Add(GetMessage("december"));
			months = results.ToArray();
			//-----------------------------------------
			List<string> results_gen = new List<string>();
			results_gen.Add(GetMessage("january-gen"));
			results_gen.Add(GetMessage("february-gen"));
			results_gen.Add(GetMessage("march-gen"));
			results_gen.Add(GetMessage("april-gen"));
			results_gen.Add(GetMessage("may-gen"));
			results_gen.Add(GetMessage("june-gen"));
			results_gen.Add(GetMessage("july-gen"));
			results_gen.Add(GetMessage("august-gen"));
			results_gen.Add(GetMessage("september-gen"));
			results_gen.Add(GetMessage("october-gen"));
			results_gen.Add(GetMessage("november-gen"));
			results_gen.Add(GetMessage("december-gen"));
			months_gen = results_gen.ToArray();
		}

		/// <summary>
		/// Months
		/// </summary>
		public string[] Months
		{
			get
			{
				return months;
			}
		}

		/// <summary>
		/// Months that used in date
		/// </summary>
		public string[] MonthsGen
		{
			get
			{
				return months_gen;
			}
		}

		/// <summary>
		/// Regular expression for month
		/// </summary>
		public string MonthRegex
		{
			get
			{
				string str = "(";
				foreach (string cmonth in MonthsGen)
					str += Regex.Escape(cmonth) + "|";
				str = str.TrimEnd('|');
				return str + ")";
			}
		}
	}
}
