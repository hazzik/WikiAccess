/**********************************************************************************
 * Page class of WikiAccess Library                                               *
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
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using WikiTools.Web;

namespace WikiTools.Access
{
	/// <summary>
	/// Represents wiki page
	/// </summary>
	public class Page
	{
		//Permanent variables
		private string[] categories;
		private bool categoriesLoaded;
		private bool editPrepared;
		private string edittoken;
		private bool exists;

		private string[] externalLinks;
		private bool externalLinksLoaded;
		private bool historLoaded;
		private Revision[] history;

		private string[] images;
		private bool imagesLoaded;
		private string[] internalLinks;
		private bool internalLinksLoaded;

		private string lastedit;
		private int lastrevision;
		private int length;
		private string name;
		private int pgid;
		private bool propsLoaded;
		private bool redirect;
		private string redirectsOn;
		private bool redirectsOnLoaded;
		private string starttime;
		private string[] subpages;
		private bool subpagesLoaded;
		private string[] templates;
		private bool templatesLoaded;
		private string text;
		private bool textLoaded;
		private DateTime touched;
		private readonly Wiki wiki;

		/// <summary>
		/// Initializes new instance of Page
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <param name="pgname">Page name</param>
		[Obsolete("Please use Wiki.GetPage(string) factory method instead.")]
		public Page(Wiki wiki, string pgname)
		{
			this.wiki = wiki;
			name = pgname;
		}

		#region Text Property

		/// <summary>
		/// Returns page text
		/// </summary>
		public string Text
		{
			get
			{
				if (!textLoaded) LoadText();
				return text;
			}
		}

		/// <summary>
		/// Reloads page text.
		/// Note: when you get content of Text property for first time, it automatically calls this method
		/// </summary>
		public void LoadText()
		{
			try
			{
				string pgname = "index.php?action=raw&title=" + HttpUtility.UrlEncode(name).Replace("%3a", ":");
				text = wiki.ab.CreateGetQuery(pgname).DownloadText();
			}
			catch (WebException we)
			{
				if (((HttpWebResponse) we.Response).StatusCode == HttpStatusCode.NotFound)
					throw new WikiPageNotFoundExcecption();
				throw;
			}
			textLoaded = true;
		}

		#endregion

		/// <summary>
		/// Category of this page. Automatically calls LoadCategory() on first usage
		/// </summary>
		public string[] Categories
		{
			get
			{
				if (!categoriesLoaded) LoadCategories();
				return categories;
			}
		}

		/// <summary>
		/// Gets the page on which this page redirects. Automatically calls LoadRedirectsOn() on first usage
		/// </summary>
		public string RedirectsOn
		{
			get
			{
				if (!redirectsOnLoaded) LoadRedirectsOn();
				return redirectsOn;
			}
		}

		/// <summary>
		/// Gets history of this page. Automatically calls LoadHistory() on first usage
		/// </summary>
		public Revision[] History
		{
			get
			{
				if (!historLoaded) LoadHistory();
				return history;
			}
		}

		/// <summary>
		/// Gets internal links on this page. Automatically calls LoadInternalLinks() on first usage
		/// </summary>
		public string[] InternalLinks
		{
			get
			{
				if (!internalLinksLoaded) LoadInternalLinks();
				return internalLinks;
			}
		}

		/// <summary>
		/// Gets external links on this page. Automatically calls LoadExternalLinks() on first usage
		/// </summary>
		public string[] ExternalLinks
		{
			get
			{
				if (!externalLinksLoaded) LoadExternalLinks();
				return externalLinks;
			}
		}

		/// <summary>
		/// Gets subpages of this page. Automatically calls LoadSubpages() on first usage
		/// </summary>
		public string[] Subpages
		{
			get
			{
				if (!subpagesLoaded) LoadSubpages();
				return subpages;
			}
		}

		/// <summary>
		/// Gets list of templates used of this page. Automatically calls LoadTemplates() on first usage
		/// </summary>
		public string[] Templates
		{
			get
			{
				if (!templatesLoaded) LoadTemplates();
				return templates;
			}
		}

		/// <summary>
		/// Gets list of images used of this page. Automatically calls LoadImages() on first usage
		/// </summary>
		public string[] Images
		{
			get
			{
				if (!imagesLoaded) LoadImages();
				return images;
			}
		}

		#region Common page info (from api.php?action=query&prop=info)

		/// <summary>
		/// Gets page ID. Automatically calls LoadInfo() on first usage
		/// </summary>
		public int PageID
		{
			get
			{
				if (!propsLoaded) LoadInfo();
				return exists ? pgid : -1;
			}
		}

		/// <summary>
		/// Gets page name
		/// </summary>
		public string PageName
		{
			get { return name; }
		}

		/// <summary>
		/// Gets last edit time. Automatically calls LoadInfo() on first usage
		/// </summary>
		public DateTime Touched
		{
			get
			{
				if (!propsLoaded) LoadInfo();
				return exists ? touched : DateTime.MinValue;
			}
		}

		/// <summary>
		/// Gets last edit ID. Automatically calls LoadInfo() on first usage
		/// </summary>
		public int PageRevisionID
		{
			get
			{
				if (!propsLoaded) LoadInfo();
				return exists ? lastrevision : -1;
			}
		}

		/// <summary>
		/// Is this page redirect. Automatically calls LoadInfo() on first usage
		/// </summary>
		public bool IsRedirect
		{
			get
			{
				if (!propsLoaded) LoadInfo();
				return exists && redirect;
			}
		}

		/// <summary>
		/// Gets existance of this page. Automatically calls LoadInfo() on first usage
		/// </summary>
		public bool Exists
		{
			get
			{
				if (!propsLoaded) LoadInfo();
				return exists;
			}
		}

		/// <summary>
		/// Gets length of article in bytes using either API or page text
		/// </summary>
		public int Length
		{
			get
			{
				if (!propsLoaded && !textLoaded) LoadInfo();
				if (propsLoaded) return length;
				if (textLoaded) return Encoding.UTF8.GetByteCount(text);
				throw new Exception();
			}
		}

		#endregion

		#region Namespace-related properties

		/// <summary>
		/// Gets namespace ID of this page
		/// </summary>
		public int NamespaceID
		{
			get { return wiki.NamespacesUtils.GetNamespaceByTitle(name); }
		}

		/// <summary>
		/// Gets namespace name of this page
		/// </summary>
		public string NamespaceName
		{
			get { return NamespaceID != 0 ? name.Split(':')[0] : ""; }
		}

		/// <summary>
		/// Is this page talk?
		/// </summary>
		public bool IsTalkPage
		{
			get { return wiki.NamespacesUtils.IsTalkNamespace(name); }
		}

		/// <summary>
		/// Gets talk page of this page
		/// </summary>
		public Page TalkPage
		{
			get { return wiki.GetPage(wiki.NamespacesUtils.TitleToTalk(name)); }
		}

		#endregion

		#region SetText

		/// <summary>
		/// Saves this page
		/// </summary>
		/// <param name="newText">New text of this page</param>
		/// <param name="summary">Edit summary</param>
		/// <param name="minor">Mark revision as minor</param>
		/// <param name="watch">Add this page to watchlist</param>
		public void SetText(string newText, string summary, bool minor, bool watch)
		{
			SetText(newText, summary, minor, watch, -1);
		}

		/// <summary>
		/// Saves this page
		/// </summary>
		/// <param name="newText">New text of this page</param>
		/// <param name="summary">Edit summary</param>
		/// <param name="minor">Mark revision as minor</param>
		public void SetText(string newText, string summary, bool minor)
		{
			SetText(newText, summary, minor, false);
		}

		/// <summary>
		/// Saves this page
		/// </summary>
		/// <param name="newText">New text of this page</param>
		/// <param name="summary">Edit summary</param>
		public void SetText(string newText, string summary)
		{
			SetText(newText, summary, false, false);
		}

		/// <summary>
		/// Saves this page
		/// </summary>
		/// <param name="newText">New text of this page</param>
		public void SetText(string newText)
		{
			SetText(newText, "", false, false);
		}

		/// <summary>
		/// Saves this page
		/// </summary>
		/// <param name="newText">New text of this page</param>
		/// <param name="summary">Edit summary</param>
		/// <param name="minor">Mark revision as minor</param>
		/// <param name="watch">Add this page to watchlist</param>
		/// <param name="section">Section to edit</param>
		public void SetText(string newText, string summary, bool minor, bool watch, int section)
		{
			SetText(newText, summary, minor, watch, false, section);
		}

	    /// <summary> 
	    /// Saves this page
	    /// </summary>
	    /// <param name="newText">New text of this page</param>
	    /// <param name="summary">Edit summary</param>
	    /// <param name="minor">Mark revision as minor</param>
	    /// <param name="watch">Add this page to watchlist</param>
	    /// <param name="bot">
	    ///   Mark this edit as bot
	    /// </param>
	    public void SetText(string newText, string summary, bool minor, bool watch, bool bot)
		{
			SetText(newText, summary, minor, watch, bot, -1);
		}

		/// <summary> 
		/// Saves this page
		/// </summary>
		/// <param name="newText">New text of this page</param>
		/// <param name="summary">Edit summary</param>
		/// <param name="minor">Mark revision as minor</param>
		/// <param name="watch">Add this page to watchlist</param>
		/// <param name="bot">Mark this edit as bot</param>
		/// <param name="section">Section to edit</param>
		public virtual void SetText(string newText, string summary, bool minor, bool watch, bool bot, int section)
		{
			if (!editPrepared)
				PrepareToEdit();
			var query = wiki.ab.CreatePostQuery("api.php?format=xml")
				.Add("action", "edit")
				.Add("title", name)
				.Add("text", newText)
				.Add("token", edittoken)
				.Add("summary", summary)
				.Add(minor ? "minor" : "notminor", "1")
				.Add("basetimestamp", lastedit)
				.Add("starttimestamp", starttime)
				.Add(watch ? "watch" : "unwatch", "1");
			if (bot)
			{
				query.Add("bot", "1");
			}
			if (section != -1)
			{
				query.Add("section", section.ToString());
			}
			query.DownloadText();
			editPrepared = false;
		}

		#endregion

		/// <summary>
		/// Loads common page info: existance, page ID, last edit time, last edit ID, is redirect
		/// </summary>
		public void LoadInfo()
		{
			string pgname = string.Format(Query.PageInfo, HttpUtility.UrlEncode(name));
			var doc = new XmlDocument();
			doc.Load(wiki.ab.CreateGetQuery(pgname).GetResponseStream());
			var pageElem = (XmlElement) doc.GetElementsByTagName("page")[0];
			propsLoaded = true;
			exists = !pageElem.HasAttribute("missing");
			if (!exists)
				return;
			pgid = Int32.Parse(pageElem.Attributes["pageid"].Value);
			touched = DateTime.Parse(pageElem.Attributes["touched"].Value).ToUniversalTime();
			lastrevision = Int32.Parse(pageElem.Attributes["lastrevid"].Value);
			redirect = pageElem.HasAttribute("redirect");
			length = Int32.Parse(pageElem.Attributes["length"].Value);
		}

		/// <summary>
		/// Loads on which page this page redirects
		/// </summary>
		public void LoadRedirectsOn()
		{
			string pgname = "index.php?redirect=no&title=" + HttpUtility.UrlEncode(name);
			string pgtext = wiki.ab.CreateGetQuery(pgname).DownloadText();
			redirectsOnLoaded = true;
			if (!Regexes.HTMLRedirect.Match(pgtext).Success)
			{
				redirectsOn = null;
				return;
			}
			redirectsOn = (Regexes.HTMLRedirect.Match(pgtext).Success ? Regexes.HTMLRedirect.Match(pgtext).Groups[1].Value : "");
		}

		/// <summary>
		/// Loads internal links on this page
		/// </summary>
		public void LoadInternalLinks()
		{
			string pgname = string.Format(Query.PageLinksInternal, HttpUtility.UrlEncode(name));
			var doc = new XmlDocument();
			doc.Load(wiki.ab.CreateGetQuery(pgname).GetResponseStream());
			XmlNodeList nl = doc.GetElementsByTagName("pl");
			internalLinksLoaded = true;
			internalLinks = (from XmlElement node in nl select node.Attributes["title"].Value).ToArray();
		}

		/// <summary>
		/// Loads external links on this page
		/// </summary>
		public void LoadExternalLinks()
		{
			string pgname = string.Format(Query.PageLinksExternal, HttpUtility.UrlEncode(name));
			var doc = new XmlDocument();
			doc.Load(wiki.ab.CreateGetQuery(pgname).GetResponseStream());
			XmlNodeList nl = doc.GetElementsByTagName("el");
			externalLinksLoaded = true;
			externalLinks = (from XmlElement node in nl select node.InnerText).ToArray();
		}

		/// <summary>
		/// Loads list of templates used on this page
		/// </summary>
		public void LoadTemplates()
		{
			string pgname = string.Format(Query.PageTemplates, HttpUtility.UrlEncode(name));
			var doc = new XmlDocument();
			doc.Load(wiki.ab.CreateGetQuery(pgname).GetResponseStream());
			XmlNodeList nl = doc.GetElementsByTagName("tl");
			templatesLoaded = true;
			templates = (from XmlElement node in nl select node.Attributes["title"].Value).ToArray();
		}

		/// <summary>
		/// Loads images of templates used on this page
		/// </summary>
		public void LoadImages()
		{
			string pgname = string.Format(Query.PageImages, HttpUtility.UrlEncode(name));
			var doc = new XmlDocument();
			doc.Load(wiki.ab.CreateGetQuery(pgname).GetResponseStream());
			XmlNodeList nl = doc.GetElementsByTagName("im");
			imagesLoaded = true;
			images = (from XmlElement celem in nl select celem.Attributes["title"].Value).ToArray();
		}

		/// <summary>
		/// Loads history of this page
		/// </summary>
		public void LoadHistory()
		{
			string uri = string.Format(Query.PageHistory, HttpUtility.UrlEncode(name));
			bool needNext;
			historLoaded = true;
			var tmp = new List<Revision>();
			do
			{
				var doc = new XmlDocument();
				doc.Load(wiki.ab.CreateGetQuery(uri).GetResponseStream());
				if (((XmlElement) doc.GetElementsByTagName("page")[0]).HasAttribute("missing"))
					throw new WikiPageNotFoundExcecption();
				var revsroot = (XmlElement) doc.GetElementsByTagName("revisions")[0];
				tmp.AddRange(from XmlNode node in revsroot.ChildNodes
				             where node.NodeType == XmlNodeType.Element && node.Name == "rev"
				             select ParseRevision((XmlElement) node));
				if (doc.GetElementsByTagName("query-continue").Count > 0)
				{
					var qcelem = (XmlElement) doc.GetElementsByTagName("query-continue")[0].FirstChild;
					uri = string.Format(Query.PageHistoryContinue, HttpUtility.UrlEncode(name), qcelem.Attributes["rvstartid"].Value);
					needNext = true;
				}
				else
					needNext = false;
			} while (needNext);
			history = tmp.ToArray();
		}

		private Revision ParseRevision(XmlElement element)
		{
			var result = new Revision();
			result.Wiki = wiki;
			result.Minor = element.HasAttribute("minor");
			result.ID = Int32.Parse(element.Attributes["revid"].Value);
			result.Page = name;
			result.Author = element.Attributes["user"].Value;
			result.Time = DateTime.Parse(element.Attributes["timestamp"].Value).ToUniversalTime();
			result.Comment = (element.HasAttribute("comment") ? element.Attributes["comment"].Value : "");
			return result;
		}

		/// <summary>
		/// Loads category that contains this page
		/// </summary>
		public void LoadCategories()
		{
			string pgname = string.Format(Query.PageCategories, HttpUtility.UrlEncode(name));
			var doc = new XmlDocument();
			doc.Load(wiki.ab.CreateGetQuery(pgname).GetResponseStream());
			XmlNodeList nl = doc.GetElementsByTagName("cl");
			categoriesLoaded = true;
			categories = (from XmlElement node in nl
			              select node.Attributes["title"].Value.Substring(wiki.NamespacesUtils.GetNamespaceByID(Namespaces.Category).Length + 1)).ToArray();
		}

		/// <summary>
		/// Loads subpages of this page
		/// </summary>
		public void LoadSubpages()
		{
			subpages = wiki.GetPrefixIndex(
				wiki.NamespacesUtils.RemoveNamespace(name) + "/", PageTypes.All, NamespaceID);
			subpagesLoaded = true;
		}

		/*/// <summary>
		/// Deletes this page
		/// </summary>
		/// <param name="reason">Reason of deletion</param>
		public void Delete(string reason)
		{
			ab.PageName = "index.php?action=delete&title=" + ab.EncodeUrl(name);
			ab.SetTextboxField("wpReason", reason);
			ab.ClickButton("wpConfirmB");
		}

		public void Protect(string reason, ProtectionLevel edit, ProtectionLevel move, bool cascade, TimeSpan duration)
		{
			ab.PageName = "index.php?action=protect&title=" + ab.EncodeUrl(name);
			ab.SetTextboxField("mwProtect-reason", reason);
			ab.SetTextboxField("mwProtect-expiry", Utils.FormatDateTimeRFC2822(DateTime.UtcNow + duration));
			ab.SetCheckbox("mwProtect-cascade", cascade);
		}*/

		/// <summary>
		/// Renames this name
		/// </summary>
		/// <param name="newName">New page name</param>
		/// <param name="reason">Reason of name change</param>
		public void Rename(string newName, string reason)
		{
			var query = wiki.ab.CreatePostQuery("index.php?action=submit&title=Special:Movepage")
				.Add("wpOldTitle", name)
				.Add("wpNewTitle", newName)
				.Add("wpEditToken", GetToken("move"))
				.Add("wpReason", reason);
			query.DownloadText();
		}

		/// <summary>
		/// Cleans cache of this page
		/// </summary>
		public void Purge()
		{
			string pgname = "index.php?action=purge&title=" + HttpUtility.UrlEncode(name);
			wiki.ab.CreateGetQuery(pgname).DownloadText();
		}

		/// <summary>
		/// Bypasses redirect
		/// </summary>
		public void BypassRedirect()
		{
			if (!IsRedirect) return;
			name = RedirectsOn;
			LoadRedirectsOn();
			LoadInfo();
		}

		/// <summary>
		/// Initializes variables to prevent edit conflicts
		/// </summary>
		public void PrepareToEdit()
		{
			starttime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
			try
			{
				lastedit = GetLastEdit().ToString("yyyyMMddHHmmss");
				edittoken = GetToken("edit");
			}
			catch (WikiPageNotFoundExcecption)
			{
				lastedit = starttime;
				edittoken = GetToken(wiki.GetAllPages("", 1, PageTypes.All, 0)[0], "edit");
			}
			editPrepared = true;
		}

		/// <summary>
		/// Returns the date of last edit
		/// </summary>
		/// <returns></returns>
		public DateTime GetLastEdit()
		{
			string pgname = string.Format(Query.PageLastEdit, HttpUtility.UrlEncode(name));
			var doc = new XmlDocument();
			doc.Load(wiki.ab.CreateGetQuery(pgname).GetResponseStream());
			var pageelem = (XmlElement) doc.GetElementsByTagName("page")[0];
			if (pageelem.HasAttribute("missing")) throw new WikiPageNotFoundExcecption();
			var revelem = (XmlElement) doc.GetElementsByTagName("rev")[0];
			return DateTime.Parse(revelem.Attributes["timestamp"].Value).ToUniversalTime();
		}

		internal string GetToken(string type)
		{
			return GetToken(name, type);
		}

		private string GetToken(string page, string type)
		{
			/* Note from http://www.mediawiki.org/wiki/API:Import:
			 * To import pages, an import token is required. This token is equal to the edit token and
			 * the same for all pages, but changes at every login. */

			// We dont need to get the token every time for every page for every type!
			// this will save requests and time when doing mass imports/uploads
			if(wiki.EditToken == null)
			{
				// get once a token and store it until logout
				string pgname = string.Format(Query.PageToken, HttpUtility.UrlEncode(page), type);
				var doc = new XmlDocument();
				doc.Load(wiki.ab.CreateGetQuery(pgname).GetResponseStream());
				var elem = (XmlElement)doc.GetElementsByTagName("page")[0];
				if (elem == null || !elem.HasAttribute(type + "token")) throw new WikiPermissionsExpection();
				wiki.EditToken = elem.Attributes[type + "token"].Value;
			}
			return wiki.EditToken;
		}

		#region Watch / Unwatch

		/// <summary>
		/// Adds page to watchlist
		/// </summary>
		public void Watch()
		{
			string pgname = "index.php?action=watch&title=" + name;
			wiki.ab.CreateGetQuery(pgname).DownloadText();
		}

		/// <summary>
		/// Removes page from watchlist
		/// </summary>
		public void Unwatch()
		{
			string pgname = "index.php?action=unwatch&title=" + name;
			wiki.ab.CreateGetQuery(pgname).DownloadText();
		}

		#endregion
	}
}