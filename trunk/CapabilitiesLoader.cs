using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiTools.Access
{
    partial class Wiki
    {
        public WikiCapabilities LoadCapabilities()
        {
            WikiCapabilities result = new WikiCapabilities();
            ab.PageName = "index.php?title=Special:Version";
            string vesionPage = ab.PageText;
            result.HasCheckUser = vesionPage.Contains("<i>CheckUser</i>");
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
