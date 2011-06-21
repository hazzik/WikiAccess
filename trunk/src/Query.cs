using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using WikiTools.Access;

namespace WikiTools.Web
{
    public abstract class Query : IQuery
    {
		protected readonly CookieContainer _cookies;
		protected readonly IDictionary<string, string> _data;
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
			_data = data;
			_cookies = cookies;
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public IQuery Add(string key, string value)
		{
			_data.Add(key, value);
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
			_cookies.Add(response.Cookies);
			return response.GetResponseStream();
		}

		public TextReader GetTextReader()
		{
			return new StreamReader(GetResponseStream());
		}

		protected virtual HttpWebRequest CreateRequest()
		{
            ServicePointManager.Expect100Continue = false;
			var result = (HttpWebRequest) WebRequest.Create(Uri);
			result.UserAgent = "WikiAccess library v" + Utils.Version;
			result.Proxy.Credentials = CredentialCache.DefaultCredentials;
			result.UseDefaultCredentials = true;
			result.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			result.CookieContainer = _cookies;
			return result;
		}
	}
}