/**********************************************************************************
 * Common types of WikiAccess Library                                             *
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
using System.IO;

namespace WikiTools.Access
{

	#region Capabilities

	/// <summary>
	/// Capabilities of wiki
	/// </summary>
	public struct WikiCapabilities
	{
		/// <summary>
		/// Required to check users
		/// </summary>
		public bool HasCheckUser;

		/// <summary>
		/// Required for full template substitution
		/// </summary>
		public bool HasExpandTemplates;

		/// <summary>
		/// Required to get images
		/// </summary>
		public bool HasFilePath;

		/// <summary>
		/// Required to make bot via MakeBot interface and have access to MakeBot log
		/// </summary>
		public bool HasMakeBot;

		/// <summary>
		/// Required to determine what permission bureaucrat have
		/// </summary>
		public bool HasMakeSysop;

		/// <summary>
		/// Required to get new users log
		/// </summary>
		public bool HasNewUserLog;

		/// <summary>
		/// Required to hide revisions
		/// </summary>
		public bool HasOversight;

		/// <summary>
		/// Required to rename users
		/// </summary>
		public bool HasRenameUser;

		/// <summary>
		/// Version of MediaWiki
		/// </summary>
		public Version Version;

		/// <summary>
		/// Casts capabilities to string
		/// </summary>
		/// <returns>String</returns>
		public override string ToString()
		{
			var str = new []
			              {
			                  "!Wiki-capa",
			                  "version = " + Version,
			                  "ext:checkuser = " + HasCheckUser,
			                  "ext:exptl = " + HasExpandTemplates,
			                  "ext:fpath = " + HasFilePath,
			                  "ext:mkbot = " + HasMakeBot,
			                  "ext:mksysop = " + HasMakeSysop,
			                  "ext:newusers = " + HasNewUserLog,
			                  "ext:oversight = " + HasOversight,
			                  "ext:renameuser = " + HasRenameUser
			              };
		    return String.Join("\n", str);
		}

		/// <summary>
		/// Loads capabilities from string
		/// </summary>
		/// <param name="s">String to parse</param>
		/// <returns>Succes of parsing</returns>
		public bool FromString(string s)
		{
			using (var sr = new StringReader(s))
			{
				if (sr.ReadLine().Trim() != "!Wiki-capa")
					return false;
				string cstr;
				while (!String.IsNullOrEmpty(cstr = sr.ReadLine()))
				{
					string[] kv = cstr.Split(new[] {'='}, 2);
					kv[0] = kv[0].Trim();
					kv[1] = kv[1].Trim();
					switch (kv[0])
					{
						case "version":
							// if there was a problem reading version information, the persisted version will be empty
							if (!string.IsNullOrWhiteSpace(kv[1]))
								Version = new Version(kv[1]);
							break;
						case "ext:checkuser":
							HasCheckUser = Boolean.Parse(kv[1]);
							break;
						case "ext:exptl":
							HasExpandTemplates = Boolean.Parse(kv[1]);
							break;
						case "ext:fpath":
							HasFilePath = Boolean.Parse(kv[1]);
							break;
						case "ext:mkbot":
							HasMakeBot = Boolean.Parse(kv[1]);
							break;
						case "ext:mksysop":
							HasMakeSysop = Boolean.Parse(kv[1]);
							break;
						case "ext:newusers":
							HasNewUserLog = Boolean.Parse(kv[1]);
							break;
						case "ext:oversight":
							HasOversight = Boolean.Parse(kv[1]);
							break;
						case "ext:renameuser":
							HasRenameUser = Boolean.Parse(kv[1]);
							break;
					}
				}
			}
			return true;
		}
	}

	#endregion
}