using System.IO;

namespace WikiTools.Access
{
	partial class Wiki
	{
		/// <summary>
		/// Imports a XML File that was generated bei either Special:Export, <see cref="Wiki.ExportPagesFromCategory"/>
		/// or <see cref="Wiki.ExportPages"/>.
		/// </summary>
		/// <param name="streamReader">Stream to the XML File</param>
		/// <param name="summary">summary for history log</param>
		public void ImportPages(StreamReader streamReader, string summary)
		{
			/* Note from http://www.mediawiki.org/wiki/API:Import:
			 * To import pages, an import token is required. This token is equal to the edit token and
			 * the same for all pages, but changes at every login. */
			string token = GetPage("Main Page").GetToken("import");

			using(streamReader)
			{
				string content = streamReader.ReadToEnd();
				var qry = ab.CreatePostQuery("api.php?format=xml")
					.Add("action", "import")
					.Add("token", token)
					.Add("summary", summary)
					// ToDo although this works, its bad code, we need another method than Add(key, value)
					// that allows modifications of the post data which is set in PostQuery.CommitValue
					// the filename attribute is needed, otherwise the post data will not go into phps $_Files variable
					// which seems kind of weird for an api call, but thats life
					.Add("xml\"; filename=\"wikitext.xml", content);
				// Note if fixed, remove comment in PostQuery.cs:56
				string s = qry.DownloadText();
			}
		}
	}
}
