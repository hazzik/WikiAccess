using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WikiTools.Access;
using WikiTools.Access.Extensions;

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
		    string data = _data
		        .Select(kvp => CommitValue(kvp.Key, kvp.Value))
		        .Join("\r\n");
		    return Encoding.UTF8.GetBytes(data);
		}

		private string CommitValue(string key, string value)
		{
		    var sb = new StringBuilder();
		    sb.AppendFormat("--{0}", _boundary)
		        .AppendLine()
		        .AppendFormat("Content-Disposition: form-data; name=\"{0}\"", key)
		        .AppendLine()
		        .AppendFormat("Content-Type: text/plain; charset=utf-8")
		        .AppendLine()
		        .AppendLine()
		        .Append(value);
		    return sb.ToString();
		}

		private static string CreateBoundary()
		{
			return "-------" + Image.CalculateMD5Hash(Rnd.RandomBytes(1024));
		}
	}
}