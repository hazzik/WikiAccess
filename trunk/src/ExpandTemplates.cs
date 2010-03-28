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
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace WikiTools.Access
{
	partial class Wiki
	{
		private string ExpandTemplatesOrRender(string action, string text, string pagetitle)
		{
		    var query = ab.CreatePostQuery("api.php")
		        .Add("format", "xml")
		        .Add("action", action)
		        .Add("title", pagetitle)
		        .Add("text", text);
			var doc = new XmlDocument();
			doc.Load(query.GetResponseStream());
		    var xpni = (XPathNodeIterator) doc.CreateNavigator().Evaluate("/api/" + action);
		    return xpni.Cast<XPathItem>().Select(i => i.Value).FirstOrDefault();
		}

		public string ExpandTemplates(string text, string pagetitle)
		{
			return ExpandTemplatesOrRender("expandtemplates", text, pagetitle);
		}

		public string RenderText(string text, string pagetitle)
		{
			return ExpandTemplatesOrRender("render", text, pagetitle);
		}
	}
}