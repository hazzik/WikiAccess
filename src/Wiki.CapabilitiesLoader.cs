/**********************************************************************************
 * Capabilities loader of WikiAccess Library                                      *
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
using System.Text.RegularExpressions;

namespace WikiTools.Access
{
	partial class Wiki
	{
		private WikiCapabilities LoadCapabilities()
		{
		    const string page = "index.php?title=Special:Version";
		    return ParseWikiCapabilities(ab.CreateGetQuery(page).DownloadText());
		}

	    private static WikiCapabilities ParseWikiCapabilities(string versionPage)
		{
			var result = new WikiCapabilities
                    {
                        HasCheckUser = versionPage.Contains("<i>CheckUser</i>"),
                        HasExpandTemplates = versionPage.Contains("<i>ExpandTemplates</i>"),
                        HasFilePath = versionPage.Contains("<i>Filepath</i>"),
                        HasMakeBot = versionPage.Contains("<i>MakeBot</i>"),
                        HasMakeSysop = versionPage.Contains("<i>Makesysop</i>"),
                        HasNewUserLog = versionPage.Contains("<i>Newuserlog</i>"),
                        HasOversight = versionPage.Contains("<i>Oversight</i>"),
                        HasRenameUser = versionPage.Contains("<i>Renameuser</i>")
                    };
	        Match match = Regex.Match(versionPage, @"MediaWiki</a>: (\d).(\d{1,2})");
			// 2008-11-16 BL - Ticket 2300889 - Modified to work with newer versions of the Special:Version page.
			if (match.Success)
			{
				result.Version = new Version(Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value));
			}
			else
			{
				match = Regex.Match(versionPage, @"MediaWiki</a></td>\s*<td>(\d).(\d{1,2})");
				if(match.Success)
					result.Version = new Version(Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value));
				else
					System.Diagnostics.Debug.WriteLine("Could not retrieve version information.");
			}
			return result;
		}
	}
}