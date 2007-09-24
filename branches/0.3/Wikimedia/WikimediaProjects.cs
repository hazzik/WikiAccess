/**********************************************************************************
 * Wikimedia-specific utils of WikiAccess Library                                 *
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

namespace WikiTools.Access.Wikimedia
{
	#region Wikimedia projects list and documentation

	/// <summary>
	/// Wikimedia projects list.
	/// Last update: 27.05.2007
	/// </summary>
	public enum WikimediaProjects
	{
		/// <summary>
		/// Wikipedia - a free encyclopedia
		/// </summary>
		Wikipedia,
		/// <summary>
		/// Wiktionary - a free dictionary
		/// </summary>
		Wiktionary,
		/// <summary>
		/// Wikibooks - free learning materials
		/// </summary>		
		Wikibooks,
		/// <summary>
		/// Wikinews - a free news source
		/// </summary>
		Wikinews,
		/// <summary>
		/// Wikiquote - a free online compendium of quotations
		/// </summary>
		Wikiquote,
		/// <summary>
		/// Wikisource - an online library of free content publications
		/// </summary>
		Wikisource,
		/// <summary>
		/// Wikiversity - a free learning comunity
		/// </summary>
		Wikiversity,
		/// <summary>
		/// Wikimedia Commons - shared media repository
		/// </summary>
		Commons,
		/// <summary>
		/// Meta-Wiki - Wikimedia project coordination
		/// </summary>
		Meta,
		/// <summary>
		/// Wikimedia Incubator - Incubator of Wikimedia projects
		/// </summary>
		Incubator,
		/// <summary>
		/// Wikisource central wiki
		/// </summary>
		Sources,
		/// <summary>
		/// Wikispecies - a free directory of species
		/// </summary>
		Species,
		/// <summary>
		/// Test wiki for developers and bot debugging
		/// </summary>
		Test,
		/// <summary>
		/// Wikimedia Foundation wiki
		/// </summary>
		Foundation,
		/// <summary>
		/// MediaWiki.org
		/// </summary>
		MediaWiki,
		/// <summary>
		/// Wikimania 2005 official site
		/// </summary>
		Wikimania2005,
		/// <summary>
		/// Wikimania 2006 official site
		/// </summary>
		Wikimania2006,
		/// <summary>
		/// Wikimania 2007 official site
		/// </summary>
		Wikimania2007,
	}
	#endregion

	/// <summary>
	/// Provides utils for wikimedia projects
	/// </summary>
	public class WikimediaUtils
	{
		/// <summary>
		/// Makes URI for specified Wikimedia project
		/// </summary>
		/// <param name="proj">Project</param>
		/// <returns>Project URI</returns>
		public static string MakeUri(WikimediaProjects proj)
		{
			return MakeUri(proj, "");
		}

		/// <summary>
		/// Makes URI for specified Wikimedia project
		/// </summary>
		/// <param name="proj">Project</param>
		/// <param name="langCode">Language code</param>
		/// <returns>Project URI</returns>
		public static string MakeUri(WikimediaProjects proj, string langCode)
		{
			if (IsMultilingualProject(proj))
			{
				return "http://" + langCode + "." + proj.ToString().ToLower() + ".org/w";
			}
			else
			{
				switch (proj)
				{
					case WikimediaProjects.Test:
						return "http://test.wikipedia.org/w";
					case WikimediaProjects.Foundation:
						return "http://wikimediafoundation.org/w";
					default:
						return "http://" + proj.ToString().ToLower() + ".wikimedia.org/w";
				}
			}
		}

		static bool IsMultilingualProject(WikimediaProjects proj)
		{
			WikimediaProjects[] mlingprojs = new WikimediaProjects[] { WikimediaProjects.Wikipedia,
				WikimediaProjects.Wiktionary, WikimediaProjects.Wikibooks, WikimediaProjects.Wikiversity,
				WikimediaProjects.Wikinews, WikimediaProjects.Wikiquote, WikimediaProjects.Wikisource };
			return Array.IndexOf(mlingprojs, proj) != -1;
		}

		/// <summary>
		/// Gets Wikimedia wikis capabilities
		/// </summary>
		public static WikiCapabilities Capabilities
		{
			get
			{
				WikiCapabilities result = new WikiCapabilities();
				result.HasCheckUser = result.HasExpandTemplates = result.HasFilePath = result.HasMakeBot = result.HasMakeSysop
					= result.HasNewUserLog = result.HasOversight = result.HasRenameUser = true;
				result.Version = new Version(1, 11);
				return result;
			}
		}
	}
}
