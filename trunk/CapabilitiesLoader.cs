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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiTools.Access
{
	partial class Wiki
	{
		private WikiCapabilities LoadCapabilities()
		{
			WikiCapabilities result = new WikiCapabilities();
			ab.PageName = "index.php?title=Special:Version";
			string vesionPage = ab.PageText;
			result.HasCheckUser = vesionPage.Contains("<i>CheckUser</i>");
			result.HasExpandTemplates = vesionPage.Contains("<i>ExpandTemplates</i>");
			result.HasFilePath = vesionPage.Contains("<i>Filepath</i>");
			result.HasMakeBot = vesionPage.Contains("<i>MakeBot</i>");
			result.HasMakeSysop = vesionPage.Contains("<i>Makesysop</i>");
			result.HasNewUserLog = vesionPage.Contains("<i>Newuserlog</i>");
			result.HasOversight = vesionPage.Contains("<i>Oversight</i>");
			result.HasRenameUser = vesionPage.Contains("<i>Renameuser</i>");
			Match match = Regex.Match(vesionPage, @"MediaWiki</a>: (\d).(\d{1,2})");
			result.Version = new Version(Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value));
			return result;
		}
	}
}
