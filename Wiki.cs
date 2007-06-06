/**********************************************************************************
 * Wiki class of WikiAcces Library                                                *
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
using System.Net;
using System.Text;
using System.Web;

namespace WikiTools.Access
{
	/// <summary>
	/// Provides access to wiki
	/// </summary>
    public partial class Wiki : IDisposable
    {
		string wikiURI;
        internal AccessBrowser ab;
		MessageCache mcache;
        string mcachepath, nscachepath, urcachepath;
		internal Namespaces ns;
        string[] userflags;

		/// <summary>
		/// URI of wiki in format http://mediawiki.org/w
		/// </summary>
        public string WikiURI
        {
            get { return wikiURI; }
            //set { wikiuri = value; }
        }

		/// <summary>
		/// Initializes new instance of wiki object. Message cache will be stored in current directory
		/// </summary>
		/// <param name="uri">URI of wiki. <see cref="Wiki.WikiURI"/></param>
		public Wiki(string uri) : this(uri, ".")
		{
		}

		/// <summary>
		/// Initializes new instance of wiki object.
		/// </summary>
		/// <param name="uri">URI of wiki. <see cref="Wiki.WikiURI"/></param>
		/// <param name="cachedir">Folder where Message cache will be stored</param>
        public Wiki(string uri, string cachedir)
        {
            wikiURI = uri;
			ab = new AccessBrowser(this);
			mcachepath = cachedir + "\\" + MessageCache.MkName(uri);
			nscachepath = cachedir + "\\" + Namespaces.MkName(uri);
            urcachepath = cachedir + "\\" + new Uri(uri).Host + ".userflags";
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
            if (File.Exists(urcachepath)) userflags = File.ReadAllLines(urcachepath);
            else
            {
                userflags = User.GetAvailableFlags(this);
                File.WriteAllLines(urcachepath, userflags);
            }
        }

        #region Login Functions

		/// <summary>
		/// Logs in
		/// </summary>
		/// <param name="username">User name</param>
		/// <param name="password">User password</param>
		/// <returns>Succes</returns>
        public bool Login(string username, string password)
		{
			ab.PageName = "index.php?title=Special:Userlogin";
			ab.SetTextboxField("wpName1", username);
			ab.SetTextboxField("wpPassword1", password);
			ab.SetCheckbox("wpRemember", true);
			ab.ClickButton("wpLoginattempt");
            ab.PageName = "index.php";
			return ab.IsLoggedIn();
		}

		/// <summary>
		/// Logs out
		/// </summary>
		public void Logout()
		{
			ab.PageName = "index.php?title=Special:Userlogout";
		}

		/// <summary>
		/// Checks if we are currently logged in
		/// </summary>
		/// <returns>True, if we are logged in</returns>
		public bool IsLoggedIn()
		{
			return ab.IsLoggedIn();
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
            userflags = User.GetAvailableFlags(this);
            File.WriteAllLines(urcachepath, userflags);
        }

        #endregion

		/// <summary>
		/// User flags available on this wiki
		/// </summary>
        public string[] UserFlags
        {
            get
            {
                return userflags;
            }
        }

		/// <summary>
		/// Instance of Namespaces class
		/// </summary>
		/// <seealso cref="Namespaces"/>
        public Namespaces NamespacesUtils
        {
            get
            {
                return ns;
            }
        }

		/// <summary>
		/// Message cache
		/// </summary>
        public MessageCache Messages
        {
            get
            {
                return mcache;
            }
        }

		/// <summary>
		/// Statistics for this site (see Special:Statistics?action=raw)
		/// </summary>
        public Statistics Statistics
        {
            get
            {
                Access.Statistics result = new Statistics();
                string statstr = ab.DownloadPage("index.php?title=Special:Statistics&action=raw");
                string[] _stats = statstr.Split(';');
                Dictionary<string, int> stats = new Dictionary<string,int>();
                for (int i = 0; i < _stats.Length; i++)
                    stats.Add(_stats[i].Split('=')[0], Int32.Parse(_stats[i].Split('=')[1]));
                result.Admins = stats["admins"];
                result.Edits = stats["edits"];
                result.GoodPages = stats["good"];
                result.Images = stats["images"];
                result.Jobs = stats["jobs"];
                result.TotalPages = stats["total"];
                result.Users = stats["users"];
                result.Views = stats["views"];
                return result;
            }
        }

        #region IDisposable Members

		/// <summary>
		/// Release all resources used by Wiki object
		/// </summary>
        public void Dispose()
		{
			ab.Dispose();
			ab = null;
		}

		#endregion
	}
}