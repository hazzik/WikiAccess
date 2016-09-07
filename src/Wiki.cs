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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using WikiTools.Web;

namespace WikiTools.Access
{
	/// <summary>
	/// Provides access to wiki
	/// </summary>
	public partial class Wiki : IDisposable
	{
		private readonly WikiCapabilities capabilities;
		private readonly string capacachepath;
		private readonly string mcachepath;
		private readonly string nscachepath;
		private readonly string wikiUri;
		internal IAccessBrowser ab;
		private CurrentUser cu;
		private MessageCache mcache;
		internal Namespaces ns;
		private bool loggedIn;

		/// <summary>
		/// Token stored for editing, importing and so on.
		/// <remarks>
		/// Note from http://www.mediawiki.org/wiki/API:Import:
		/// To import pages, an import token is required. This token is equal to the edit token and
		/// the same for all pages, but changes at every login.
		/// </remarks>
		/// </summary>
		internal string EditToken { get; set; }

		/// <summary>
		/// Initializes new instance of a Wiki object. Message cache will be stored in current directory.
		/// </summary>
		/// <param name="uri">URI of wiki. <see cref="Wiki.WikiURI"/></param>
		public Wiki(string uri) : this(uri, ".")
		{
		}

		public Wiki(IAccessBrowser accessBrowser)
		{
			ab = accessBrowser;
		}

		/// <summary>
		/// Initializes new instance of a Wiki object.
		/// </summary>
		/// <param name="uri">URI of Wiki. <see cref="Wiki.WikiURI"/></param>
		/// <param name="cachedir">Folder where Message cache will be stored</param>
		public Wiki(string uri, string cachedir)
		{
			wikiUri = uri;
			ab = new AccessBrowser(uri);
			mcachepath = cachedir + "/" + MessageCache.MkName(uri);
			nscachepath = cachedir + "/" + Namespaces.MkName(uri);
			capacachepath = cachedir + "/" + new Uri(uri).Host + ".capabilities";
			if (File.Exists(mcachepath)) mcache = new MessageCache(mcachepath);
			else
			{
				mcache = new MessageCache(this);
				mcache.SaveToFile(mcachepath);
			}
			if (File.Exists(nscachepath)) ns = new Namespaces(nscachepath);
			else
			{
				ns = new Namespaces(this);
				ns.SaveToFile(nscachepath);
			}
			if (File.Exists(capacachepath)) capabilities.FromString(File.ReadAllText(capacachepath));
			else
			{
				capabilities = LoadCapabilities();
				File.WriteAllText(capacachepath, capabilities.ToString());
			}
		}

		#region Login Functions

		/// <summary>
		/// Logs in
		/// </summary>
		/// <param name="name">User name</param>
		/// <param name="password">User password</param>
		/// <returns>Succes</returns>
		public bool Login(string name, string password)
		{
			// if the user calls Login several time dont take any data from other sessions with us
			Logout();


		    var stream =
		        ab.HttpClient.PostAsync("api.php?format=xml", new FormUrlEncodedContent(new Dictionary<string, string>
		        {
		            {"action", "login"},
		            {"lgname", name},
		            {"lgpassword", password},

		        })).Result.Content.ReadAsStreamAsync().Result;

            XDocument xdoc = XDocument.Load(stream);
			var element = xdoc.CreateNavigator()
				.SelectSingleNode("//api/login/@result");

			// see http://www.mediawiki.org/wiki/API:Login
			switch (element.Value.ToLowerInvariant())
			{
				// until MediaWiki 1.15.3
				case "success":
					return loggedIn = true;
				// since 1.15.5 (1.15.4 is broken)
				case "needtoken":
			        element = xdoc.CreateNavigator().SelectSingleNode("//api/login/@token");

			        stream = ab.HttpClient.PostAsync("api.php?format=xml", new FormUrlEncodedContent(new Dictionary<string, string>
			            {
			                {"action", "login"},
			                {"lgname", name},
			                {"lgpassword", password},
			                {"lgtoken", element.Value},
			            })).Result.Content.ReadAsStreamAsync().Result;

                    xdoc = XDocument.Load(stream);
					element = xdoc.CreateNavigator().SelectSingleNode("//api/login/@result");
					return loggedIn = element.Value.ToLowerInvariant().Equals("success");
				default:
					return loggedIn = false;
			}
		}

		/// <summary>
		/// Logs out
		/// </summary>
		public void Logout()
		{
			EditToken = null;
			ab.ClearCookies();
		}

		/// <summary>
		/// Checks if we are currently logged in
		/// </summary>
		/// <returns>True, if we are logged in</returns>
		public bool IsLoggedIn()
		{
			return loggedIn;
		}

		#endregion

		#region Cache Functions

		/// <summary>
		/// Gets message from message cache
		/// </summary>
		/// <param name="messageName">Message name</param>
		/// <returns>Message content</returns>
		public string GetMessage(string messageName)
		{
			return mcache.GetMessage(messageName);
		}

		/// <summary>
		/// Updates Message Cache
		/// </summary>
		public void UpdateMessageCache()
		{
			mcache = new MessageCache(this);
			mcache.SaveToFile(mcachepath);
			Namespaces.SaveToFile(nscachepath, Namespaces.GetNamespaces(this));
		}

		#endregion

		/// <summary>
		/// URI of wiki in format http://mediawiki.org/w
		/// </summary>
		[Obsolete]
		public string WikiURI
		{
			get { return wikiUri; }
		}

		/// <summary>
		/// Returns wiki capabilities (version and extensions)
		/// </summary>
		public WikiCapabilities Capabilities
		{
			get { return capabilities; }
		}

		/// <summary>
		/// Instance of Namespaces class
		/// </summary>
		/// <seealso cref="Namespaces"/>
		public Namespaces NamespacesUtils
		{
			get { return ns; }
		}

		/// <summary>
		/// Message cache
		/// </summary>
		public MessageCache Messages
		{
			get { return mcache; }
		}

		/// <summary>
		/// Statistics for this site (see Special:Statistics?action=raw)
		/// </summary>
		public Statistics Statistics
		{
		    get { return GetStatisticsAsync(); }
		}

	    private Statistics GetStatisticsAsync()
	    {
	        string statstr = ab.HttpClient.GetStringAsync("index.php?title=Special:Statistics&action=raw").Result;

	        var stats = statstr.Split(';')
	            .Select(t => t.Split('='))
	            .ToDictionary(strings => strings[0], strings => int.Parse(strings[1]));

	        return new Statistics
	        {
	            Admins = stats["admins"],
	            Edits = stats["edits"],
	            GoodPages = stats["good"],
	            Images = stats["images"],
	            Jobs = stats["jobs"],
	            TotalPages = stats["total"],
	            Users = stats["users"],
	            Views = stats["views"]
	        };
	    }

	    /// <summary>
		/// Returns info about current user
		/// </summary>
		public CurrentUser CurrentUserInfo
		{
			get
			{
				if (cu == null)
					LoadCurrentUserInfo();
				return cu;
			}
		}

		#region CreatePage versions

		/// <summary>
		/// Creates a page on wiki if it doesn't exists
		/// </summary>
		/// <param name="name">Page name</param>
		/// <param name="text">Page text</param>
		/// <param name="summary">Page creation summry</param>
		public void CreatePage(string name, string text, string summary)
		{
			CreatePage(name, text, summary, false);
		}

		/// <summary>
		/// Creates a page on wiki if it doesn't exists
		/// </summary>
		/// <param name="name">Page name</param>
		/// <param name="text">Page text</param>
		public Page CreatePage(string name, string text)
		{
			return CreatePage(name, text, "", false);
		}

		/// <summary>
		/// Creates a page on wiki
		/// </summary>
		/// <param name="name">Page name</param>
		/// <param name="text">Page text</param>
		/// <param name="overwrite">Overwrite page, if it already exists</param>
		public Page CreatePage(string name, string text, bool overwrite)
		{
			return CreatePage(name, text, "", overwrite);
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Release all resources used by Wiki object
		/// </summary>
		public void Dispose()
		{
			ab = null;
		}

		#endregion

		/// <summary>
		/// Creates a page on wiki
		/// </summary>
		/// <param name="name">Page name</param>
		/// <param name="text">Page text</param>
		/// <param name="summary">Page creation summry</param>
		/// <param name="overwrite">Overwrite page, if it already exists</param>
		public Page CreatePage(string name, string text, string summary, bool overwrite)
		{
			Page page = GetPage(name);
			if (overwrite || !page.Exists)
			{
				page.SetText(text, summary);
			}
			return page;
		}

		/// <summary>
		/// Reads user talk page to remove new message notification
		/// </summary>
		public void ReadNewMessages()
		{
		    ab.HttpClient.GetStringAsync("index.php?title=Special:Mytalk").Wait();
		}

		private void LoadCurrentUserInfo()
		{
			cu = new CurrentUser(this);
		}

		public Page GetPage(string pgname)
		{
#pragma warning disable 618,612
			return new Page(this, pgname);
#pragma warning restore 618,612
		}
	}
}