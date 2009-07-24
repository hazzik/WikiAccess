using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using WikiTools.Access;

namespace WikiTools.Web
{
	public abstract class Query
	{
		protected readonly CookieContainer _cookies;
		protected readonly IDictionary<string, string> _data;
		private readonly string _uri;

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
			_uri = uri;
			_data = data;
			_cookies = cookies;
		}

		public string Uri
		{
			get { return _uri; }
		}

		public Query Add(string key, string value)
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

		public StreamReader GetTextReader()
		{
			return new StreamReader(GetResponseStream());
		}

		protected virtual HttpWebRequest CreateRequest()
		{
			var result = (HttpWebRequest) WebRequest.Create(Uri);
			result.UserAgent = "WikiAccess library v" + Utils.Version;
			result.Proxy.Credentials = CredentialCache.DefaultCredentials;
			result.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			result.CookieContainer = _cookies;
			return result;
		}
	}

	public class GetQuery : Query
	{
		public GetQuery(string uri, CookieContainer cookies)
			: base(uri, cookies)
		{
		}
	}

	public class PostQuery : Query
	{
		private readonly string _boundary = CreateBoundary();

		public PostQuery(string uri)
			: base(uri)
		{
		}

		public PostQuery(string uri, CookieContainer cookies)
			: base(uri, cookies)
		{
		}

		public PostQuery(string uri, CookieContainer cookies, IDictionary<string, string> data) : base(uri, cookies, data)
		{
		}

		protected override HttpWebRequest CreateRequest()
		{
			byte[] data = GetDataBytes();

			HttpWebRequest result = base.CreateRequest();
			result.Method = "POST";
			result.AllowAutoRedirect = false;
			result.ContentType = "multipart/form-data; boundary=" + _boundary;
			result.ContentLength = data.Length;

			Stream str = result.GetRequestStream();
			str.Write(data, 0, data.Length);

			return result;
		}

		private byte[] GetDataBytes()
		{
			string postdata = "";
			foreach (var kvp in _data)
			{
				postdata += CommitValue(kvp.Key, kvp.Value);
			}
			postdata = postdata.Substring(0, postdata.Length - 2);
			return Encoding.UTF8.GetBytes(postdata);
		}

		private string CommitValue(string key, string value)
		{
			string result = "--" + _boundary + "\r\n";
			result += "Content-Disposition: form-data; name=\"" + key + "\"\r\n";
			result += "Content-Type: text/plain; charset=utf-8\r\n";
			result += "\r\n";
			result += value + "\r\n";
			return result;
		}

		private static string CreateBoundary()
		{
			return "-------" + Image.CalculateMD5Hash(Rnd.RandomBytes(1024));
		}
	}
}