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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace WikiTools.Access
{
	/// <summary>
	/// Message cache
	/// </summary>
	public class MessageCache
	{
		private readonly string mcachetext;
		private IDictionary<string, string> _cache;
		private string[] months, months_gen;

		/// <summary>
		/// Loads message cache from files
		/// </summary>
		/// <param name="fname">File name</param>
		public MessageCache(string fname)
		{
			mcachetext = File.ReadAllText(fname);
		}

		/// <summary>
		/// Loads message cache from live wiki
		/// </summary>
		/// <param name="wiki">Wiki to load</param>
		public MessageCache(Wiki wiki)
		{
			mcachetext = wiki.ab.CreateGetQuery("index.php?title=Special:Allmessages&ot=xml").DownloadText();
		}

		/// <summary>
		/// Returns message cache messages
		/// </summary>
		/// <param name="str">Message name</param>
		/// <returns>Message contents</returns>
		public string this[string str]
		{
			get { return GetMessage(str); }
		}

		private IDictionary<string, string> Cache
		{
			get { return _cache ?? (_cache = GetMessages()); }
		}

		/// <summary>
		/// Months
		/// </summary>
		public string[] Months
		{
			get { return months ?? (months = GetMonths()); }
		}

		/// <summary>
		/// Months that used in date
		/// </summary>
		public string[] MonthsGen
		{
			get { return months_gen ?? (months_gen = GetMonthsGen()); }
		}

		/// <summary>
		/// Regular expression for month
		/// </summary>
		public string MonthRegex
		{
			get
			{
			    return string.Format("({0})", string.Join("|", MonthsGen.Select(x => Regex.Escape(x)).ToArray()));
			}
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
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Gets message from message cache
		/// </summary>
		/// <param name="name">Name of message</param>
		/// <returns>Message content</returns>
		public string GetMessage(string name)
		{
			string value;
			Cache.TryGetValue(name, out value);
			return value;
		}

		private IDictionary<string, string> GetMessages()
		{
			var xdoc = new XPathDocument(new StringReader(mcachetext));
			XPathNavigator nav = xdoc.CreateNavigator();
			IDictionary<string, string> result = new Dictionary<string, string>();
			foreach (XPathNavigator item in nav.Select("messages/message"))
			{
				result.Add(item.GetAttribute("name", ""), item.Value);
			}
			return result;
		}

		private string[] GetMonths()
		{
		    return new []
		               {
		                   GetMessage("january"),
		                   GetMessage("february"),
		                   GetMessage("march"),
		                   GetMessage("april"),
		                   GetMessage("may"),
		                   GetMessage("june"),
		                   GetMessage("july"),
		                   GetMessage("august"),
		                   GetMessage("september"),
		                   GetMessage("october"),
		                   GetMessage("november"),
		                   GetMessage("december")
		               };
		}

		private string[] GetMonthsGen()
		{
		    return new []
		               {
		                   GetMessage("january-gen"),
		                   GetMessage("february-gen"),
		                   GetMessage("march-gen"),
		                   GetMessage("april-gen"),
		                   GetMessage("may-gen"),
		                   GetMessage("june-gen"),
		                   GetMessage("july-gen"),
		                   GetMessage("august-gen"),
		                   GetMessage("september-gen"),
		                   GetMessage("october-gen"),
		                   GetMessage("november-gen"),
		                   GetMessage("december-gen")
		               };
		}
	}
}