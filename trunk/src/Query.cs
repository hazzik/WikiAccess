using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using WikiTools.Access;

namespace WikiTools.Web
{
	public abstract class Query : IQuery
	{
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