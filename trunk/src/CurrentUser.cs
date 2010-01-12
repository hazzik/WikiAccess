/**********************************************************************************
 * CurrentUser class of WikiAccess Library                                        *
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
using System.Xml;

namespace WikiTools.Access
{
	/// <summary>
	/// Gets info about current user using API
	/// </summary>
	public class CurrentUser
	{
		private string blockedby;
		private string blockreason;
		private bool hasnewmsg;
		private bool isBlocked;
		private string[] usergroups;
		private string[] userrights;
		private readonly Wiki w;

		/// <summary>
		/// Initializes new instance of object
		/// </summary>
		/// <param name="w">Wiki to use</param>
		public CurrentUser(Wiki w)
		{
			this.w = w;
			Reload();
		}

		/// <summary>
		/// Indicates if user has new messages
		/// </summary>
		public bool HasNewMessges
		{
			get { return hasnewmsg; }
		}

		/// <summary>
		/// Get rights (not flags/groups!) which user has
		/// </summary>
		public string[] UserRights
		{
			get { return userrights; }
		}

		/// <summary>
		/// Contains user groups/flags
		/// </summary>
		public string[] UserGroups
		{
			get { return usergroups; }
		}

		/// <summary>
		/// Indicates if user is blocked
		/// </summary>
		public bool IsBlocked
		{
			get { return isBlocked; }
		}

		/// <summary>
		/// Gets sysop which blocked current user
		/// </summary>
		public string BlockedBy
		{
			get { return blockedby; }
		}

		/// <summary>
		/// Reason why user is blocked
		/// </summary>
		public string BlockReason
		{
			get { return blockreason; }
		}

		/// <summary>
		/// Indicates if user can edit
		/// </summary>
		public bool CanEdit
		{
			get { return HasRight("edit") && !isBlocked; }
		}

		/// <summary>
		/// Reloads user info
		/// </summary>
		public void Reload()
		{
			const string page = "api.php?action=query&format=xml&meta=userinfo&uiprop=blockinfo|groups|rights|hasmsg";
			var doc = new XmlDocument();
			doc.Load(w.ab.CreateGetQuery(page).GetResponseStream());
			var rootelem = (XmlElement) doc.GetElementsByTagName("userinfo")[0];
			hasnewmsg = rootelem.HasAttribute("messages");
			var rightselem = (XmlElement) rootelem.GetElementsByTagName("rights")[0];
			var groupselem = (XmlElement) rootelem.GetElementsByTagName("groups")[0];
			var userrightsTemp = new List<string>();
			foreach (XmlNode cnode in rightselem.ChildNodes)
			{
				if (cnode.NodeType != XmlNodeType.Element || cnode.Name != "r") continue;
				var celem = (XmlElement) cnode;
				userrightsTemp.Add(celem.InnerText);
			}
			userrights = userrightsTemp.ToArray();
			var usergroupsTemp = new List<string>();
			foreach (XmlNode cnode in groupselem.ChildNodes)
			{
				if (cnode.NodeType != XmlNodeType.Element || cnode.Name != "g") continue;
				var celem = (XmlElement) cnode;
				usergroupsTemp.Add(celem.InnerText);
			}
			usergroups = usergroupsTemp.ToArray();
			isBlocked = rootelem.HasAttribute("blockedby") || rootelem.HasAttribute("blockreason");
			if (isBlocked)
			{
				blockedby = rootelem.Attributes["blockedby"].Value;
				blockreason = rootelem.Attributes["blockreason"].Value;
			}
			else
			{
				blockedby = null;
				blockreason = null;
			}
		}

		/// <summary>
		/// Indicates if user has specified right
		/// </summary>
		/// <param name="right"></param>
		/// <returns></returns>
		public bool HasRight(string right)
		{
			return new List<string>(userrights).Contains(right);
		}
	}
}