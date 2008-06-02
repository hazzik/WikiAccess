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
	public enum RecentChangeType {
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
	public struct RecentChange {
		/// <summary>
		/// Type of change
		/// </summary>
		/// <see cref="RecentChangeType" />
		public RecentChangeType Type;
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
		/// Old revision ID
		/// </summary>
		public int OldRevisionID;
		/// <summary>
		/// Timestamp
		/// </summary>
		public DateTime Time;
		/// <summary>
		/// Username
		/// </summary>
		public string User;
		/// <summary>
		/// Comment
		/// </summary>
		public string Comment;
		/// <summary>
		/// Is new page
		/// </summary>
		public bool New;
		/// <summary>
		/// Is minor edit
		/// </summary>
		public bool Minor;
		/// <summary>
		/// Is bot edit
		/// </summary>
		public bool Bot;
		public int OldSize, NewSize;
		
		public override string ToString()
		{
			string s = Type.ToString() + ": ";
			s += Page + " (rcid=" + RCID.ToString() + ";";
			s += "diff=" + OldRevisionID.ToString() + "-" + RevisionID.ToString() + ";";
			List<string> flags = new List<string>();
			if( Bot ) flags.Add( "bot" );
			if( Minor ) flags.Add( "minor" );
			if( New ) flags.Add( "new" );
			s += String.Join( ";", flags.ToArray() );
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
		int updateTime = 5;
		internal Wiki w;
		Thread t;
		int lastrc = 0;
		
		public RecentChangesWatcher(Wiki w)
		{
			this.w = w;
			t = new Thread( new ThreadStart( ThreadProc ) );
		}
		
		public int UpdateTime {
			get {
				return updateTime;
			}
			set {
				if( value > 0 )
					updateTime = value;
			}
		}
		
		public void Start() {
			if( t.ThreadState != ThreadState.Running )
				t.Start();
		}
		
		public void Stop() {
			t.Abort();
		}
		
		void ThreadProc() {
			lastrc = GetRecentChanges()[0].RCID;
			while(true) {
				RecentChange[] changes = GetRecentChanges();
				Array.Reverse( changes );
				foreach( RecentChange change in changes ) {
					if( change.RCID <= lastrc )
						continue;
					lastrc = change.RCID;
					EditEventArgs eea = new EditEventArgs(change);
					if( OnEdit != null )
						OnEdit( (object)this, eea );
				}
				Thread.Sleep( new TimeSpan( 0, 0, updateTime ) );
			}
		}
		
		public RecentChange[] GetRecentChanges() {
			string uri =
				"api.php?format=xml&action=query&list=recentchanges"
				+ "&rclimit=max&rcprop=user|comment|flags|timestamp|title|sizes|ids";
			string xml = w.ab.DownloadPage(uri);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNodeList nodes = doc.GetElementsByTagName("rc");
			
			List<RecentChange> changes = new List<RecentChange>();
			foreach( XmlNode node in nodes ) {
				XmlElement elem = node as XmlElement;
				RecentChange change = new RecentChange();
				change.Type = (RecentChangeType)
					Enum.Parse( typeof(RecentChangeType), Utils.UCFisrt( elem.GetAttribute( "type" ) ) );
				change.Page = elem.Attributes["title"].Value;
				change.RCID = int.Parse( elem.GetAttribute("rcid") );
				change.OldRevisionID = int.Parse( elem.GetAttribute("old_revid") );
				change.RevisionID = int.Parse( elem.GetAttribute("revid") );
				change.Bot = elem.HasAttribute( "bot" );
				change.Minor = elem.HasAttribute( "minor" );
				change.New = elem.HasAttribute( "new" );
				change.OldSize = int.Parse( elem.GetAttribute( "oldlen" ) );
				change.NewSize = int.Parse( elem.GetAttribute( "newlen" ) );
				change.User = elem.GetAttribute( "user" );
				change.Time = DateTime.Parse(elem.GetAttribute("timestamp")).ToUniversalTime();
				if( elem.HasAttribute( "comment" ) )
					change.Comment = elem.GetAttribute( "comment" );
				else
					change.Comment = "";
				changes.Add(change);
			}
			return changes.ToArray();
		}
		
		public event EditEventHandler OnEdit;
	}
	
	public delegate void EditEventHandler(object sender, EditEventArgs e);
	
	public class EditEventArgs : EventArgs {
		RecentChange rc;
		
		public EditEventArgs(RecentChange rc) {
			this.rc = rc;
		}
		
		public RecentChange Change {
			get {
				return rc;
			}
		}
	}
}
