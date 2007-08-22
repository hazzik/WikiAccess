/**********************************************************************************
 * Page Class of WikiAcces Library                                                *
 * Copyright (C) 2007 Vasiliev V. V.                                              *
 *                                                                                *
 * This program is free software; you can redistribute it and/or                  *
 * modify it under the terms of the GNU General Public License                    *
 * as published by the Free Software Foundation; either version 2                 *
 * of the License, or (at your option) any later version.                         *
 *                                                                                *
 * This program is distributed in the hope that it will be useful,                *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of                 *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                  *
 * GNU General Public License for more details.                                   *
 *                                                                                *
 * You should have received a copy of the GNU General Public License              *
 * along with this program; if not, write to the Free Software                    *
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.*
 **********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace WikiTools.Access
{
	/// <summary>
	/// Represents wikipage
	/// </summary>
	public partial class Page
	{
		//Permanent variables
		Wiki wiki;
		string name;
		AccessBrowser ab;

		//Loadable variables & Load flags
		string text; bool textLoaded = false;

		int pgid; DateTime touched; int lastrevision; bool redirect; bool exists; int length;
		bool propsLoaded = false;

		string redirectsOn; bool redirectsOnLoaded = false;

		Revision[] history; bool historLoaded = false;

		string[] internalLinks; bool internalLinksLoaded = false;

		string[] externalLinks; bool externalLinksLoaded = false;

		string[] categories; bool categoriesLoaded = false;

		string[] templates; bool templatesLoaded = false;

		string[] images; bool imagesLoaded = false;

		string[] subpages; bool subpagesLoaded = false;
		
		string edittoken; string lastedit; string starttime; bool editPrepared = false;

		/// <summary>
		/// Initializes new instance of page
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <param name="pgname">Page name</param>
		public Page(Wiki wiki, string pgname)
		{
			this.wiki = wiki;
			name = pgname;
			ab = wiki.ab;
		}

		#region Text Property

		/// <summary>
		/// Returns page text
		/// </summary>
		public string Text
		{
			get
			{
				if (textLoaded)
					return text;
				else
				{
					LoadText();
					return text;
				}
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
				text = ab.DownloadPage("index.php?action=raw&title=" + ab.EncodeUrl(name));
			}
			catch (WebException we)
			{
				if (((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.NotFound)
					throw new WikiPageNotFoundExcecption();
				else
					throw we;
			}
			textLoaded = true;
		}

		#endregion

		/// <summary>
		/// Loads common page info: existance, page ID, last edit time, last edit ID, is redirect
		/// </summary>
		public void LoadInfo()
		{
			string opts = ab.DownloadPage("api.php?action=query&format=xml&prop=info&titles=" + ab.EncodeUrl(name));
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(opts);
			XmlElement pageElem = (XmlElement)doc.GetElementsByTagName("page")[0];
			propsLoaded = true;
			exists = !Utils.ContainsAttribure(pageElem, "missing");
			if (!exists)
				return;
			pgid = Int32.Parse(pageElem.Attributes["pageid"].Value);
			touched = ab.ParseAPITimestamp(pageElem.Attributes["touched"].Value);
			lastrevision = Int32.Parse(pageElem.Attributes["lastrevid"].Value);
			redirect = Utils.ContainsAttribure(pageElem, "redirect");
			length = Int32.Parse(pageElem.Attributes["length"].Value);
		}

		/// <summary>
		/// Loads on which page this page redirects
		/// </summary>
		public void LoadRedirectsOn()
		{
			ab.PageName = "index.php?redirect=no&title=" + ab.EncodeUrl(name);
			string pgtext = ab.PageText;
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
			string page = ab.DownloadPage("api.php?action=query&format=xml&prop=links&titles=" + ab.EncodeUrl(name));
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(page);
			XmlNodeList nl = doc.GetElementsByTagName("pl");
			List<string> tmp = new List<string>();
			foreach (XmlNode node in nl)
			{
				XmlElement celem = (XmlElement)node;
				tmp.Add(celem.Attributes["title"].Value);
			}
			internalLinksLoaded = true;
			internalLinks = tmp.ToArray();
		}

		/// <summary>
		/// Loads external links on this page
		/// </summary>
		public void LoadExternalLinks()
		{
			string page = ab.DownloadPage("api.php?action=query&format=xml&prop=extlinks&titles=" + ab.EncodeUrl(name));
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(page);
			XmlNodeList nl = doc.GetElementsByTagName("el");
			List<string> tmp = new List<string>();
			foreach (XmlNode node in nl)
			{
				XmlElement celem = (XmlElement)node;
				tmp.Add(celem.InnerText);
			}
			externalLinksLoaded = true;
			externalLinks = tmp.ToArray();
		}

		/// <summary>
		/// Loads list of templates used on this page
		/// </summary>
		public void LoadTemplates()
		{
			string page = ab.DownloadPage("api.php?action=query&format=xml&prop=templates&titles=" + ab.EncodeUrl(name));
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(page);
			XmlNodeList nl = doc.GetElementsByTagName("tl");
			List<string> tmp = new List<string>();
			foreach (XmlNode node in nl)
			{
				XmlElement celem = (XmlElement)node;
				tmp.Add(celem.Attributes["title"].Value);
			}
			templatesLoaded = true;
			templates = tmp.ToArray();
		}

		/// <summary>
		/// Loads images of templates used on this page
		/// </summary>
		public void LoadImages()
		{
			string page = ab.DownloadPage("api.php?action=query&format=xml&prop=images&titles=" + ab.EncodeUrl(name));
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(page);
			XmlNodeList nl = doc.GetElementsByTagName("im");
			List<string> tmp = new List<string>();
			foreach (XmlNode node in nl)
			{
				XmlElement celem = (XmlElement)node;
				tmp.Add(celem.Attributes["title"].Value);
			}
			imagesLoaded = true;
			images = tmp.ToArray();
		}

		/// <summary>
		/// Loads history of this page
		/// </summary>
		public void LoadHistory()
		{
			string uri = "api.php?action=query&format=xml&prop=revisions&rvdir=older&rvlimit=50&rvprop=ids|flags|timestamp|user|comment&titles="
				+ ab.EncodeUrl(name);
			bool needNext = false;
			historLoaded = true;
			List<Revision> tmp = new List<Revision>();
			do
			{
				string revsxml = ab.DownloadPage(uri);
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(revsxml);
				if (Utils.ContainsAttribure((XmlElement)doc.GetElementsByTagName("page")[0], "missing"))
					throw new WikiPageNotFoundExcecption();
				XmlElement revsroot = (XmlElement)doc.GetElementsByTagName("revisions")[0];
				foreach (XmlNode node in revsroot.ChildNodes)
				{
					if (node.NodeType == XmlNodeType.Element && node.Name == "rev")
					{
						XmlElement celem = (XmlElement)node;
						Revision crev = new Revision();
						crev.Minor = Utils.ContainsAttribure(celem, "minor");
						crev.ID = Int32.Parse(celem.Attributes["revid"].Value);
						crev.Page = name;
						crev.Author = celem.Attributes["user"].Value;
						crev.Time = ab.ParseAPITimestamp(celem.Attributes["timestamp"].Value);
						crev.Comment = (celem.HasAttribute("comment") ? celem.Attributes["comment"].Value : "");
						tmp.Add(crev);
					}
				}
				if (doc.GetElementsByTagName("query-continue").Count > 0)
				{
					XmlElement qcelem = (XmlElement)doc.GetElementsByTagName("query-continue")[0].FirstChild;
					uri = "api.php?action=query&format=xml&prop=revisions&rvdir=older&rvlimit=50&rvprop=ids|flags|timestamp|user|comment&titles=" +
						ab.EncodeUrl(name) + "&rvstartid=" + qcelem.Attributes["rvstartid"].Value;
					needNext = true;
				}
				else
					needNext = false;
			} while (needNext);
			history = tmp.ToArray();
		}

		/// <summary>
		/// Loads category that contains this page
		/// </summary>
		public void LoadCategories()
		{
			string page = ab.DownloadPage("api.php?action=query&format=xml&prop=categories&titles=" + ab.EncodeUrl(name));
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(page);
			XmlNodeList nl = doc.GetElementsByTagName("cl");
			List<string> tmp = new List<string>();
			foreach (XmlNode node in nl)
			{
				XmlElement celem = (XmlElement)node;
				tmp.Add(celem.Attributes["title"].Value.Substring(wiki.NamespacesUtils.GetNamespaceByID(14).Length + 1));
			}
			categoriesLoaded = true;
			categories = tmp.ToArray();
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

		/// <summary>
		/// Category of this page. Automatically calls LoadCategory() on first usage
		/// </summary>
		public string[] Categories
		{
			get
			{
				if (!categoriesLoaded)
					LoadCategories();
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
				if (!redirectsOnLoaded)
					LoadRedirectsOn();
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
				if (!historLoaded)
					LoadHistory();
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
				if (!internalLinksLoaded)
					LoadInternalLinks();
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
				if (!externalLinksLoaded)
					LoadExternalLinks();
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
				if (!subpagesLoaded)
					LoadSubpages();
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
				if (!templatesLoaded)
					LoadTemplates();
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
				if (!imagesLoaded)
					LoadImages();
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
				if (!exists) return -1;
				else return pgid;
			}
		}

		/// <summary>
		/// Gets page name
		/// </summary>
		public string PageName
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Gets last edit time. Automatically calls LoadInfo() on first usage
		/// </summary>
		public DateTime Touched
		{
			get
			{
				if (!propsLoaded) LoadInfo();
				if (!exists) return DateTime.MinValue;
				else return touched;
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
				if (!exists) return -1;
				else return lastrevision;
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
				if (!exists) return false;
				else return redirect;
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
			get
			{
				return wiki.NamespacesUtils.GetNamespaceByTitle(name);
			}
		}

		/// <summary>
		/// Gets namespace name of this page
		/// </summary>
		public string NamespeceName
		{
			get
			{
				if (NamespaceID == 0) return "";
				else return name.Split(':')[0];
			}
		}

		/// <summary>
		/// Is this page talk?
		/// </summary>
		public bool IsTalkPage
		{
			get
			{
				return wiki.NamespacesUtils.IsTalkNamespace(name);
			}
		}

		/// <summary>
		/// Gets talk page of this page
		/// </summary>
		public Page TalkPage
		{
			get
			{
				return new Page(wiki, wiki.NamespacesUtils.TitleToTalk(name));
			}
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
			if (!editPrepared) PrepareToEdit();
			Dictionary<string, string> postdata = new Dictionary<string,string>();
			postdata.Add("wpSection", (section == -1 ? "" : section.ToString()));
			postdata.Add("wpStarttime", starttime);
			postdata.Add("wpEdittime", lastedit);
			postdata.Add("wpEditToken", edittoken);
			postdata.Add("wpTextbox1", newText);
			postdata.Add("wpSummary", summary);
			if (minor) postdata.Add("wpMinoredit", "1");
			if (watch) postdata.Add("wpWatchthis", "1");
			
			ab.PostQuery("index.php?action=submit&title=" + ab.EncodeUrl(name), postdata);
		}
		#endregion

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
		/// <param name="NewName">New page name</param>
		/// <param name="Reason">Reason of name change</param>
		public void Rename(string newName, string reason)
		{
			Dictionary<string, string> postdata = new Dictionary<string, string>();
			postdata.Add("wpOldTitle", name);
			postdata.Add("wpNewTitle", newName);
			postdata.Add("wpEditToken", GetToken(name, "move"));
			postdata.Add("wpReason", reason);
			
			ab.PostQuery("index.php?action=submit&title=Special:Movepage", postdata);
		}

		/// <summary>
		/// Cleans cahce of this page
		/// </summary>
		public void Purge()
		{
			ab.PageName = "index.php?action=purge&title=" + ab.EncodeUrl(name);
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
		
		public void PrepareToEdit()
		{
			starttime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
			try
			{
				lastedit = GetLastEdit().ToString("yyyyMMddHHmmss");
				edittoken = GetToken(name, "edit");
			}
			catch (WikiPageNotFoundExcecption e)
			{
				lastedit = starttime;
				edittoken = GetToken(wiki.GetAllPages("!", 1, PageTypes.All, 0)[0], "edit");
			}
			editPrepared = true;
		}
		
		public DateTime GetLastEdit()
		{
			string xml = ab.DownloadPage(
				"api.php?action=query&prop=revisions&&rvprop=timestamp&limit=1&format=xml&titles=" +
				ab.EncodeUrl(name));
			XmlDocument doc = new XmlDocument(); doc.LoadXml(xml);
			XmlElement pageelem = (XmlElement)doc.GetElementsByTagName("page")[0];
			if (pageelem.HasAttribute("missing")) throw new WikiPageNotFoundExcecption();
			XmlElement revelem = (XmlElement)doc.GetElementsByTagName("rev")[0];
			return ab.ParseAPITimestamp(revelem.Attributes["timestamp"].Value);
		}
		
		private string GetToken(string page, string type)
		{
			string xml = ab.DownloadPage("api.php?action=query&format=xml&prop=info&intoken="
				+ type + "&titles=" + ab.EncodeUrl(page));
			XmlDocument doc = new XmlDocument(); doc.LoadXml(xml);
			XmlElement elem = (XmlElement)doc.GetElementsByTagName("page")[0];
			if (elem == null || !elem.HasAttribute(type + "token")) throw new WikiPermissionsExpection();
			if (elem.HasAttribute("missing")) throw new WikiPageNotFoundExcecption();
			return elem.Attributes[type + "token"].Value;
		}

		#region Watch / Unwatch

		/// <summary>
		/// Adds page to watchlist
		/// </summary>
		public void Watch()
		{
			ab.PageName = "index.php?action=watch&title=" + name;
		}

		/// <summary>
		/// Removes page from watchlist
		/// </summary>
		public void Unwatch()
		{
			ab.PageName = "index.php?action=unwatch&title=" + name;
		}

		#endregion
	}
}