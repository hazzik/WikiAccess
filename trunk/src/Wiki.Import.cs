using System.IO;
using WikiTools.Web;

namespace WikiTools.Access
{
	partial class Wiki
	{
		/// <summary>
		/// Imports a XML File that was generated bei either Special:Export, <see cref="Wiki.ExportPagesFromCategory"/>
		/// or <see cref="Wiki.ExportPages"/>.
		/// </summary>
		/// <param name="stream">Stream to the XML File</param>
		/// <param name="summary">summary for history log</param>
		public void ImportPages(Stream stream, string summary)
		{
			/* Note from http://www.mediawiki.org/wiki/API:Import:
			 * To import pages, an import token is required. This token is equal to the edit token and
			 * the same for all pages, but changes at every login. */
			string token = GetPage("Main Page").GetToken("import");

			var qry = ((PostQuery) ab.CreatePostQuery("api.php?format=xml"))
				.AddFile("xml", "wikitext.xml", "text/xml", stream)
				.Add("action", "import")
				.Add("token", token)
				.Add("summary", summary);
			string s = qry.DownloadText();
		}
	}
}
