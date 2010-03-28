/**********************************************************************************
 * Recent changes watcher for WikiAccess Library                                  *
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
using System.Threading;
using System.Xml;

//using System.Xml.XPath;

namespace WikiTools.Access
{
	/// <summary>
	/// Type of change
	/// </summary>
	public enum RecentChangeType
	{
		/// <summary>
		/// An edit to a page
		/// </summary>
		Edit,
		/// <summary>
		/// New page created
		/// </summary>
		New,
		/// <summary>
		/// Log action like page move of image upload
		/// </summary>
		Log,
	}

	/// <summary>
	/// Recent change
	/// </summary>
	public struct RecentChange
	{
		/// <summary>
		/// Is bot edit
		/// </summary>
		public bool Bot;

		/// <summary>
		/// Comment
		/// </summary>
		public string Comment;

		/// <summary>
		/// Is minor edit
		/// </summary>
		public bool Minor;

		/// <summary>
		/// Is new page
		/// </summary>
		public bool New;

		public int NewSize;

		/// <summary>
		/// Old revision ID
		/// </summary>
		public int OldRevisionID;

		public int OldSize;

		/// <summary>
		/// Page name or log name for Type = Log
		/// </summary>
		public string Page;

		/// <summary>
		/// ID of change. Used for page patrolling, also used for determinig new changes
		/// </summary>
		public int RCID;

		/// <summary>
		/// New revision ID
		/// </summary>
		public int RevisionID;

		/// <summary>
		/// Timestamp
		/// </summary>
		public DateTime Time;

		/// <summary>
		/// Type of change
		/// </summary>
		/// <see cref="RecentChangeType" />
		public RecentChangeType Type;

		/// <summary>
		/// Username
		/// </summary>
		public string User;

		public override string ToString()
		{
			string s = Type.ToString() + ": ";
			s += Page + " (rcid=" + RCID.ToString() + ";";
			s += "diff=" + OldRevisionID.ToString() + "-" + RevisionID.ToString() + ";";
			var flags = new List<string>();
			if (Bot) flags.Add("bot");
			if (Minor) flags.Add("minor");
			if (New) flags.Add("new");
			s += String.Join(";", flags.ToArray());
			s += ";comment=" + Comment;
			s += ";user=" + User;
			s += ";time=" + Time.ToString();
			s += ";size-change=" + OldSize.ToString() + "-" + NewSize.ToString() + ")";
			return s;
		}
	}

	/// <summary>
	/// Class to detect changes to wiki
	/// </summary>
	public class RecentChangesWatcher
	{
		private int lastrc;
		private readonly Thread t;
		private int updateTime = 5;
		internal Wiki w;

		public RecentChangesWatcher(Wiki w)
		{
			this.w = w;
			t = new Thread(ThreadProc);
		}

		public int UpdateTime
		{
			get { return updateTime; }
			set
			{
				if (value > 0)
					updateTime = value;
			}
		}

		public void Start()
		{
			if (t.ThreadState != ThreadState.Running)
				t.Start();
		}

		public void Stop()
		{
			t.Abort();
		}

		private void ThreadProc()
		{
			lastrc = GetRecentChanges()[0].RCID;
			while (true)
			{
				RecentChange[] changes = GetRecentChanges();
				Array.Reverse(changes);
				foreach (RecentChange change in changes)
				{
					if (change.RCID <= lastrc)
						continue;
					lastrc = change.RCID;
					var eea = new EditEventArgs(change);
					if (OnEdit != null)
						OnEdit((object) this, eea);
				}
				Thread.Sleep(new TimeSpan(0, 0, updateTime));
			}
		}

		public RecentChange[] GetRecentChanges()
		{
			const string page = "api.php?format=xml&action=query&list=recentchanges"
			                    + "&rclimit=max&rcprop=user|comment|flags|timestamp|title|sizes|ids";
			var doc = new XmlDocument();
			doc.Load(w.ab.CreateGetQuery(page).GetResponseStream());
			XmlNodeList nodes = doc.GetElementsByTagName("rc");

			var changes = new List<RecentChange>();
			foreach (XmlNode node in nodes)
			{
				changes.Add(ParseReventChange((XmlElement) node));
			}
			return changes.ToArray();
		}

		private static RecentChange ParseReventChange(XmlElement element)
		{
			var result = new RecentChange();
			result.Type = (RecentChangeType) Enum.Parse(typeof (RecentChangeType), element.GetAttribute("type"), true);
			result.Page = element.Attributes["title"].Value;
			result.RCID = int.Parse(element.GetAttribute("rcid"));
			result.OldRevisionID = int.Parse(element.GetAttribute("old_revid"));
			result.RevisionID = int.Parse(element.GetAttribute("revid"));
			result.Bot = element.HasAttribute("bot");
			result.Minor = element.HasAttribute("minor");
			result.New = element.HasAttribute("new");
			result.OldSize = int.Parse(element.GetAttribute("oldlen"));
			result.NewSize = int.Parse(element.GetAttribute("newlen"));
			result.User = element.GetAttribute("user");
			result.Time = DateTime.Parse(element.GetAttribute("timestamp")).ToUniversalTime();
			result.Comment = element.HasAttribute("comment") ? element.GetAttribute("comment") : "";
			return result;
		}

		public event EditEventHandler OnEdit;
	}

	public delegate void EditEventHandler(object sender, EditEventArgs e);

	public class EditEventArgs : EventArgs
	{
		private readonly RecentChange rc;

		public EditEventArgs(RecentChange rc)
		{
			this.rc = rc;
		}

		public RecentChange Change
		{
			get { return rc; }
		}
	}
}