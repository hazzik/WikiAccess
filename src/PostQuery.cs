using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WikiTools.Access;
using StringExtensions = WikiTools.Access.Extensions.StringExtensions;

namespace WikiTools.Web
{
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

			HttpWebRequest request = base.CreateRequest();
			request.Method = WebRequestMethods.Http.Post;
			request.AllowAutoRedirect = false;
			request.ContentType = "multipart/form-data; boundary=" + _boundary;
			request.ContentLength = data.Length;

			Stream str = request.GetRequestStream();
			str.Write(data, 0, data.Length);
			return request;
		}

		private byte[] GetDataBytes()
		{
			string data = StringExtensions.Join(Data
						  .Select(kvp => CommitValue(kvp.Key, kvp.Value)), Environment.NewLine);
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