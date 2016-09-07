using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using WikiTools.Access;

namespace WikiTools.Web
{
	public abstract class Query : IQuery
	{
		// TODO this should be replaced by some classes that provide correct argument types and argument validation
		#region query uris
		private const string DefaultQuery = "api.php?action=query&format=xml";

		/// <summary>
		/// {0} Categoryname (including namespace prefix)
		/// </summary>
		internal const string CategoryMembers =
			DefaultQuery +"&list=categorymembers&cmlimit=500&cmtitle={0}";
		/// <summary>
		/// {0} Categoryname (including namespace prefix)
		/// {1} When more results are available, use this to continue.
		/// </summary>
		internal const string CategoryMembersContinue = CategoryMembers + "&cmcontinue={1}";
		/// <summary>
		/// 
		/// </summary>
		internal const string UserCurrentInfo =
			DefaultQuery + "&meta=userinfo&uiprop=blockinfo|groups|rights|hasmsg";

		internal const string UserInfo =
			DefaultQuery + "&list=allusers&aulimit=1&auprop=editcount|groups&aufrom={0}";

		/// <summary>
		/// {0} Filename (without namespace prefix)
		/// </summary>
		internal const string ImageInfo =
			DefaultQuery + "&prop=imageinfo&titles=Image:{0}&iiprop=timestamp|user|comment|url|size|dimensions|sha1|mime|metadata|archivename|bitdepth&iilimit=500&redirects";
		/// <summary>
		/// 
		/// </summary>
		internal const string InterwikiMapInfo =
			DefaultQuery + "&meta=siteinfo&siprop=interwikimap";
		/// <summary>
		/// 
		/// </summary>
		internal const string Namespaces = DefaultQuery + "&meta=siteinfo&siprop=namespaces";
		/// <summary>
		/// {0} pagename
		/// </summary>
		internal const string PageInfo = DefaultQuery + "&prop=info&titles={0}";
		/// <summary>
		/// {0} pagename
		/// </summary>
		internal const string PageLinksInternal = DefaultQuery + "&prop=links&titles={0}";
		/// <summary>
		/// {0} pagename
		/// </summary>
		internal const string PageLinksExternal = DefaultQuery + "&prop=extlinks&titles={0}";
		/// <summary>
		/// {0} pagename
		/// </summary>
		internal const string PageTemplates = DefaultQuery + "&prop=templates&titles={0}";
		/// <summary>
		/// {0} pagename
		/// </summary>
		internal const string PageImages = DefaultQuery + "&prop=images&titles={0}&imlimit=50";
		/// <summary>
		/// {0} pagename
		/// </summary>
		internal const string PageHistory = DefaultQuery + "&prop=revisions&rvdir=older&rvlimit=50&rvprop=ids|flags|timestamp|user|comment&titles={0}";
		/// <summary>
		/// {0} pagename
		/// {1} From which revision id to start enumeration (enum)
		/// </summary>
		internal const string PageHistoryContinue = PageHistory + "&rvstartid={1}";
		/// <summary>
		/// {0} pagename
		/// </summary>
		internal const string PageCategories = DefaultQuery + "&prop=categories&titles={0}";
		/// <summary>
		/// {0} pagename
		/// </summary>
		internal const string PageLastEdit = DefaultQuery + "&prop=revisions&rvprop=timestamp&limit=1&titles={0}";
		/// <summary>
		/// {0} pagename
		/// {1} Request a token to perform a data-modifying action on a page; Values (separate with '|'): edit, delete, protect, move, block, unblock, email, import
		/// </summary>
		internal const string PageToken = DefaultQuery + "&prop=info&titles={0}&intoken={1}";
		/// <summary>
		/// 
		/// </summary>
		internal const string RecentChanges = DefaultQuery + "&list=recentchanges&rclimit=max&rcprop=user|comment|flags|timestamp|title|sizes|ids";
		/// <summary>
		/// {0} How many total pages to return.
		/// {1} Which pages to list. One value: all, redirects, nonredirects; Default: all
		/// {2} The page title to start enumerating from.
		/// {3} The namespace to enumerate.
		/// </summary>
		internal const string PageList = DefaultQuery + "&list=allpages&aplimit={0}&apfilterredir={1}&apfrom={2}&apnamespace={3}";
		/// <summary>
		/// {0} How many total pages to return.
		/// {1} Which pages to list. One value: all, redirects, nonredirects; Default: all
		/// {2} The page title to start enumerating from.
		/// {3} The namespace to enumerate.
		/// {4} Search for all page titles that begin with this value.
		/// </summary>
		internal const string PageListPrefix = PageList + "&apprefix={4}";
		/// <summary>
		/// {0} Categoryname (including namespace prefix)
		/// </summary>
		// api.php?action=query&export&exportnowrap&generator=categorymembers&gcmtitle=category:XYZ
		internal const string ExportFromCategory = DefaultQuery + "&export&generator=categorymembers&gcmtitle={0}";
		/// <summary>
		/// {0} one or more pagenames separated with |
		/// </summary>
		// api.php?action=query&export&exportnowrap&titles=article1|article2
		internal const string ExportPages = DefaultQuery + "&export&titles={0}";

		internal const string BlockLog = "api.php?action=query&list=logevents&letype=block&leuser={0}&lelimit=500&format=xml";
		#endregion

		protected readonly CookieContainer Cookies;
		protected readonly IDictionary<string, string> Data;
		private readonly Uri _uri;

		protected Query(string uri)
			: this(uri, new CookieContainer())
		{
		}

		protected Query(string uri, CookieContainer cookies)
			: this(uri, cookies, new Dictionary<string, string>())
		{
		}

		protected Query(string uri, CookieContainer cookies, IDictionary<string, string> data)
		{
			_uri = new Uri(uri);
			Data = data;
			Cookies = cookies;
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public IQuery Add(string key, string value)
		{
			Data.Add(key, value);
			return this;
		}

		public string DownloadText()
		{
			return GetTextReader().ReadToEnd();
		}

		public byte[] DownloadBinary()
		{
			return Utils.ReadAllBytes(GetResponseStream());
		}

		public Stream GetResponseStream()
		{

			var response = (HttpWebResponse) CreateRequest().GetResponse();
			Cookies.Add(response.Cookies);
			return response.GetResponseStream();
		}

		public TextReader GetTextReader()
		{
			return new StreamReader(GetResponseStream());
		}

		protected virtual HttpWebRequest CreateRequest()
		{
			ServicePointManager.Expect100Continue = false;
			var request = (HttpWebRequest) WebRequest.Create(Uri);
			request.UserAgent = string.Format("{0} v{1}", AssemblyConfig.Title, AssemblyConfig.Version);
			request.Proxy.Credentials = CredentialCache.DefaultCredentials;
			request.UseDefaultCredentials = true;
			// needed authentication (ntlm, kerberos) is preserved after first request and used for further requests
			// saves N/2-1 http requests
			request.PreAuthenticate = true;
			request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			request.CookieContainer = Cookies;
			return request;
		}
	}
}