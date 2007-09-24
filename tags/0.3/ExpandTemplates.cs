/**********************************************************************************
 * ExpandTemplates draft of WikiAccess Library                                    *
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

namespace WikiTools.Access
{
	partial class Wiki
	{
		/* TODO: doesn't work
		 * public string ExpandTemplates(string text, string pagetitle, bool removeComments)
		{
			ab.PageName = "index.php?title=Special:Expandtemplates";
			ab.SetTextboxField("contexttitle", pagetitle);
			ab.SetTextboxField("input", text);
			ab.SetCheckbox("removecomments", removeComments);
			HtmlElementCollection hec = ab.WebBrowser.Document.GetElementsByTagName("input");
			foreach (HtmlElement helem in hec)
			{
				if (helem.GetAttribute("type") == "submit")
				{
					helem.InvokeMember("click");
					ab.Wait();
					break;
				}
			}
			return ab.GetTextboxField("output");
		 */
	}
}
