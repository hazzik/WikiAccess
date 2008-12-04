/**********************************************************************************
 * Revision type of WikiAccess Library                                            *
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
using System.Web;

namespace WikiTools.Access
{

	/// <summary>
	/// Page revision
	/// </summary>
	public struct Revision
	{
		/// <summary>
		/// Wiki which contains this revision
		/// </summary>
		public Wiki Wiki;
		/// <summary>
		/// Page name
		/// </summary>
		public string Page;
		/// <summary>
		/// Revision ID
		/// </summary>
		public int ID;
		/// <summary>
		/// Revision creation time
		/// </summary>
		public DateTime Time;
		/// <summary>
		/// Revision author
		/// </summary>
		public string Author;
		/// <summary>
		/// Is minor revision
		/// </summary>
		public bool Minor;
		/// <summary>
		/// Comment
		/// </summary>
		public string Comment;

		/// <summary>
		/// Gets revsion content
		/// </summary>
		/// <returns>Revision</returns>
		public string GetContent()
		{
			AccessBrowser ab = Wiki.ab;
			return ab.DownloadPage("index.php?action=raw&title=" + HttpUtility.UrlEncode(Page) + "&oldid=" + ID);
		}

		/// <summary>
		/// Creates new structure instance
		/// </summary>
		/// <param name="w">Wiki, which contatins this revision</param>
		/// <param name="ID">Revision ID</param>
		/// <param name="Page">Page name</param>
		/// <param name="Author">Revision author</param>
		/// <param name="Time">Revision creation time</param>
		/// <param name="Minor">Is minor</param>
		/// <param name="Comment">Comment</param>
		public Revision(Wiki w, int ID, string Page, string Author, DateTime Time, bool Minor, string Comment)
		{
			this.Wiki = w;
			this.ID = ID;
			this.Page = Page;
			this.Author = Author;
			this.Time = Time;
			this.Minor = Minor;
			this.Comment = Comment;
		}

		/// <summary>
		/// Represents revision as string
		/// </summary>
		/// <returns>String view of revision</returns>
		public override string ToString()
		{
			string str = Page + ": " + (Minor?"minor":"") + "revision #" + ID + " by " + Author + 
				" at " + Time.ToString("HH:mm:ss d MMMM yyyy");
			return str;
		}
	}
}
