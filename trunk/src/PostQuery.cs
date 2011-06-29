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
	public class PostQuery : Query
	{
		private static readonly string Boundary = CreateBoundary();
		private readonly IDictionary<string, byte[]> _extendedData = new Dictionary<string, byte[]>();

		private static readonly string FormValueSimple =
			"Content-Disposition: form-data; name=\"{0}\"" + Environment.NewLine +
			"Content-Type: text/plain; charset=utf-8" + Environment.NewLine +
			Environment.NewLine +
			"{1}" + Environment.NewLine +
			"--" + Boundary;

		private static readonly string FormValueFileHeader =
			"Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" + Environment.NewLine +
			"Content-Type: {2}; charset=utf-8" + Environment.NewLine +
			Environment.NewLine;

		public PostQuery(string uri)
			: base(uri)
		{
		}

		public PostQuery(string uri, CookieContainer cookies)
			: base(uri, cookies)
		{
		}

		public PostQuery(string uri, CookieContainer cookies, IDictionary<string, string> data)
			: base(uri, cookies, data)
		{
		}

		public PostQuery AddFile(string formKey, string filename, string contentType, Stream stream)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				// write header
				CopyToStream(ms,
					Encoding.UTF8.GetBytes(string.Format(FormValueFileHeader, formKey, filename, contentType)));
				// write content
				using (stream)
				{
					var data = new byte[1024];
					int bytesRead;
					while ((bytesRead = stream.Read(data, 0, data.Length)) != 0)
					{
						ms.Write(data, 0, bytesRead);
					}
				}
				// writer footer
				CopyToStream(ms, Encoding.UTF8.GetBytes(Environment.NewLine + "--" + Boundary));
				_extendedData.Add(formKey, ms.ToArray());
			}
			return this;
		}

		private static void CopyToStream(Stream ms, byte[] data)
		{
			ms.Write(data, 0, data.Length);
		}

		protected override HttpWebRequest CreateRequest()
		{
			byte[] data = GetDataBytes();

			HttpWebRequest request = base.CreateRequest();
			request.Method = WebRequestMethods.Http.Post;
			request.AllowAutoRedirect = false;
			request.ContentType = "multipart/form-data; boundary=" + Boundary;
			request.ContentLength = data.Length;

			using (Stream str = request.GetRequestStream())
			{
				str.Write(data, 0, data.Length);
			}
			return request;
		}

		private byte[] GetDataBytes()
		{
			string data = string.Format("--{0}{1}", Boundary, Environment.NewLine);
			data += Data.Select(kvp => CommitValue(kvp.Key, kvp.Value)).Join(Environment.NewLine);

			if(_extendedData.Count == 0)
				return Encoding.UTF8.GetBytes(data);

			using (var ms = new MemoryStream())
			{
				CopyToStream(ms, Encoding.UTF8.GetBytes(data));
				CopyToStream(ms, Encoding.UTF8.GetBytes(Environment.NewLine));
				foreach (var x in _extendedData.Values)
					CopyToStream(ms, x);

				return ms.ToArray();
			}
		}

		private static string CommitValue(string key, string value)
		{
			var sb = new StringBuilder();
			sb.AppendFormat(FormValueSimple, key, value);
			return sb.ToString();
		}

		private static string CreateBoundary()
		{
			return "-------" + Image.CalculateMD5Hash(Rnd.RandomBytes(1024));
		}
	}
}