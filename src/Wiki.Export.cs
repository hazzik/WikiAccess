using System.Collections.Generic;
using System.Web;
using WikiTools.Web;

namespace WikiTools.Access
{
	partial class Wiki
	{
		/// <summary>
		/// Exports all pages from a given category in one xml file.
		/// Works similiar to Special:Export
		/// <remarks>
		/// If you set <paramref name="plainXmlDump"/> to true and save the content to a file
		/// this file can be easily imported in other wikis using Special:Import.
		/// </remarks>
		/// </summary>
		/// <param name="categoryName">category name without namespace prefix</param>
		/// <param name="plainXmlDump">if set to true the result is the same like Special:Export, otherwise the result is wrapped in the usual API block</param>
		/// <returns></returns>
		public string ExportPagesFromCategory(string categoryName, bool plainXmlDump = false)
		{
			string page = string.Format(Query.ExportFromCategory,
				string.Format("{0}:{1}", ns.GetNamespaceByID(Namespaces.Category), HttpUtility.UrlEncode(categoryName)));
			if (plainXmlDump)
				page += "&exportnowrap";
			return ab.CreateGetQuery(page).DownloadText();
		}

		/// <summary>
		/// Exports all given pages in one xml file.
		/// Works similiar to Special:Export.
		/// <remarks>
		/// If you set <paramref name="plainXmlDump"/> to true and save the content to a file
		/// this file can be easily imported in other wikis using Special:Import.
		/// </remarks>
		/// </summary>
		/// <param name="pages">page names</param>
		/// <param name="plainXmlDump">if set to true the result is the same like Special:Export, otherwise the result is wrapped in the usual API block</param>
		/// <returns></returns>
		public string ExportPages(IEnumerable<string> pages, bool plainXmlDump = false)
		{
			string page = string.Format(Query.ExportPages, string.Join("|", pages));
			if (plainXmlDump)
				page += "&exportnowrap";
			return ab.CreateGetQuery(page).DownloadText();
		}
	}
}
