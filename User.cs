/**********************************************************************************
 * User class of WikiAcces Library                                                *
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
using System.Text;
using System.Text.RegularExpressions;

namespace WikiTools.Access
{
    /// <summary>
    /// Provides interface to user-related functions
    /// </summary>
    public class User
    {
        static Regex UserGroup = new Regex("<option value=\"(.*?)\">", RegexOptions.Compiled);

        string name;
        Wiki wiki;
        AccessBrowser ab;
        string[] flags;
        bool flagsLoaded = false;

        /// <summary>
        /// Initializes new instance of User class
        /// </summary>
        /// <param name="wiki">Site, where user exists</param>
        /// <param name="name">User name</param>
        public User(Wiki wiki, string name)
        {
            this.wiki = wiki;
            this.name = name;
            ab = this.wiki.ab;
            LoadRights();
        }

        #region Rights loader and interface
        /// <summary>
        /// Loads user rights. 
        /// Note: when you call Right property for first time, this method will ba automatically called.
        /// </summary>
        public void LoadRights()
        {
            ab.PageName = "index.php?title=Special:Listusers&limit=1&username=" + ab.EncodeUrl(name);
            List<string> result = new List<string>();
            string cuserrights = Regex.Match(ab.PageText, @"<li><a href=.+?>.+?</a>([^(]*?\(?.*?\)?)</li>").Groups[1].Value;
            foreach (string cflag in wiki.UserFlags)
            {
                if (cuserrights.Contains(wiki.GetMessage("group-" + cflag + "-member")) || cuserrights.Contains(cflag)) result.Add(cflag);
            }
            flags = result.ToArray();
            flagsLoaded = true;
        }

        /// <summary>
        /// Checks if user has specified flag
        /// </summary>
        /// <param name="right">User flag to check</param>
        /// <returns>User flag availability</returns>
        public bool HasRight(string right)
        {
            if (!flagsLoaded)
                LoadRights();
            return Array.IndexOf(flags, right) != -1;
        }

        /// <summary>
        /// Load all available user flags in wiki
        /// </summary>
        /// <param name="wiki"></param>
        /// <returns></returns>
        public static string[] GetAvailableFlags(Wiki wiki)
        {
            AccessBrowser ab = wiki.ab;
            ab.PageName = "index.php?title=Special:Listusers&limit=0";
            string txt = ab.PageText;
            MatchCollection matches = UserGroup.Matches(txt);
            List<string> result = new List<string>();
            foreach (Match cmatch in matches)
                if (cmatch.Groups[1].Value != wiki.GetMessage("group-all") & cmatch.Groups[1].Value != "")
                    result.Add(cmatch.Groups[1].Value);
            return result.ToArray();
        }

        /// <summary>
        /// Gets user flags
        /// </summary>
        public string[] Rights
        {
            get
            {
                if (!flagsLoaded)
                    LoadRights();
                return flags;
            }
        }
        #endregion

        /// <summary>
        /// Renames user. Needs buraeucrat rights and Renameuser extension
        /// </summary>
        /// <param name="newname">New user name</param>
        /// <param name="movepages">If true, user pages will be also renamed</param>
        public void Rename(string newname, bool movepages)
        {
            ab.PageName = "index.php?title=Special:Renameuser";
            ab.SetTextboxField("oldusername", name);
            ab.SetTextboxField("newusername", newname);
            ab.SetCheckbox("movepages", movepages);
            ab.ClickButton("submit");
        }

        private void MakeBot(string reason, bool make)
        {
            //if (!wiki.Capabilities.HasMakeBot) throw new WikiNotSupportedException();
            ab.PageName = "index.php?title=Special:Makebot&username=" + ab.EncodeUrl(name);
            ab.SetTextboxField("comment", reason);
            ab.ClickButton(make ? "grant" : "revoke");
        }

        /// <summary>
        /// Grants bot flag to user
        /// </summary>
        /// <param name="comment">Reason</param>
        public void GrantBotFlag(string comment)
        {
            MakeBot(comment, true);
        }

        /// <summary>
        /// Revokes user flag from user
        /// </summary>
        /// <param name="comment">Reason</param>
        public void RevokeBotFlag(string comment)
        {
            MakeBot(comment, false);
        }

		/// <summary>
		/// Sends email to the user via Special:Emailuser
		/// </summary>
		/// <param name="subject">Subject of email</param>
		/// <param name="text">Email text</param>
		public void SendEmail(string subject, string text)
		{
			ab.PageName = "index.php?title=Special:Emailuser/" + ab.EncodeUrl(name);
			ab.SetTextboxField("wpSubject", subject);
			ab.SetTextboxField("wpText", text);
			ab.ClickButton("wpSend");
		}
    }
}
