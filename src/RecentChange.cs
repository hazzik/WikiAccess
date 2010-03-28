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

//using System.Xml.XPath;

namespace WikiTools.Access
{
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
}