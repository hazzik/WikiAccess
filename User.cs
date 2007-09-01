/**********************************************************************************
 * User class of WikiAccess Library                                               *
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
		}

		#region Rights loader and interface
		
		#endregion

		/// <summary>
		/// Renames user. Needs buraeucrat rights and Renameuser extension
		/// </summary>
		/// <param name="newname">New user name</param>
		/// <param name="movepages">If true, user pages will be also renamed</param>
		/*public void Rename(string newname, bool movepages)
		{
			if (!wiki.Capabilities.HasRenameUser) throw new WikiNotSupportedException();
			ab.PageName = "index.php?title=Special:Renameuser";
			ab.SetTextboxField("oldusername", name);
			ab.SetTextboxField("newusername", newname);
			ab.SetCheckbox("movepages", movepages);
			ab.ClickButton("submit");
		}*/
		
		/*private void MakeBot(string reason, bool make)
		{
			if (!wiki.Capabilities.HasMakeBot) throw new WikiNotSupportedException();
			ab.PageName = "index.php?title=Special:Makebot&username=" + ab.EncodeUrl(name);
			ab.SetTextboxField("comment", reason);
			ab.ClickButton(make ? "grant" : "revoke");
		}*/

		/// <summary>
		/// Grants bot flag to user
		/// </summary>
		/// <param name="comment">Reason</param>
		/*public void GrantBotFlag(string comment)
		{
			MakeBot(comment, true);
		}*/

		/// <summary>
		/// Revokes user flag from user
		/// </summary>
		/// <param name="comment">Reason</param>
		/*public void RevokeBotFlag(string comment)
		{
			MakeBot(comment, false);
		}*/

		/// <summary>
		/// Sends email to the user via Special:Emailuser
		/// </summary>
		/// <param name="subject">Subject of email</param>
		/// <param name="text">Email text</param>
		/*public void SendEmail(string subject, string text)
		{
			ab.PageName = "index.php?title=Special:Emailuser/" + ab.EncodeUrl(name);
			ab.SetTextboxField("wpSubject", subject);
			ab.SetTextboxField("wpText", text);
			ab.ClickButton("wpSend");
		}*/
	}
}
