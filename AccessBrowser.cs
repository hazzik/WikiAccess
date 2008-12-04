/**********************************************************************************
 * Web access layer of WikiAccess Library                                         *
 * Copyright (C) 2007 Vasiliev V. V.                                              *
 *                                                                                *
 * This program is free software: you can redistribute it and/or modify           *
 * it under the terms of the GNU General Public License as published by           *
 * the Free Software Foundation, either version 3 of the License, or              *
 * (at your option) any later version.                                            *
 *                                                                                *
 * This program is distributed in the hope that it will be useful,                *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of                 *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                  *
 * GNU General Public License for more details.                                   *
 *                                                                                *
 * You should have received a copy of the GNU General Public License              *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>           *
 **********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace WikiTools.Access
{
	/// <summary>
	/// Provides access to wiki via WebRequest
	/// </summary>
	public class AccessBrowser : IDisposable
	{
		private string cpagename = "";
		private string cpagetext = "";
		private CookieContainer cookies = new CookieContainer();
		private string baseUri;

		/// <summary>
		/// Initializes new instance of AccessBrowser for the specified URI
		/// </summary>
		/// <param name="uri">Uniform Resource Identifier</param>
		public AccessBrowser(string uri) 
		{
			this.baseUri = uri;
		}

		/// <summary>
		/// Allows to change current page
		/// </summary>
		public string PageName
		{
			get
			{
				return cpagename;
			}
			set
			{
				if (cpagename != value)
				{
					cpagename = value;
					cpagetext = DownloadPage(value);
				}
			}
		}

		/// <summary>
		/// Checks if we are currently logged in
		/// </summary>
		/// <returns>Login status</returns>
		public bool IsLoggedIn()
		{
			return !cpagetext.Contains("var wgUserName = null;");
		}

		/// <summary>
		/// Current page text
		/// </summary>
		public string PageText
		{
			get
			{
				return cpagetext;
			}
		}

		/// <summary>
		/// Downloads page via WebRequest
		/// </summary>
		/// <param name="pgname">Page name</param>
		/// <returns>Page content</returns>
		public string DownloadPage(string pgname)
		{
			return DownloadPageFullUrl(baseUri + "/" + pgname);
		}


		/// <summary>
		/// Downloads page via WebRequest
		/// </summary>
		/// <param name="pgname">URL</param>
		/// <returns>Page content</returns>
		public string DownloadPageFullUrl(string pgname)
		{
			HttpWebRequest rq = CreateGetRequest(pgname);
			string result = ReadAllText(rq.GetResponse().GetResponseStream());
			cpagename = pgname;
			cpagetext = result;
			return result;
		}


		/// <summary>
		/// Sends a HTTP request using POST method and multipart/form-data content type
		/// </summary>
		/// <param name="pgname">Page name</param>
		/// <param name="data">Post data</param>
		/// <returns>HTTP response</returns>
		public string PostQuery(string pgname, IDictionary<string, string> data) 
		{
			string result = PostQueryFullUrl(baseUri + "/" + pgname, data);
			cpagename = pgname;
			return cpagetext = result;
		}
		
		//TODO: need refactor this, via "replace method with method object" refactoring
		private string PostQueryFullUrl(string uri, IDictionary<string, string> data) 
		{
			HttpWebRequest rq = CreatePostRequest(uri);
			string boundary = CreateBoundary(); //TODO: introduce field
			rq.ContentType = "multipart/form-data; boundary=" + boundary;

			string postdata = "";
			foreach(KeyValuePair<string, string> kvp in data) {
				postdata += CommitValue(boundary, kvp.Key, kvp.Value);
			}
			postdata = postdata.Substring(0, postdata.Length - 2);
			rq.ContentLength = Encoding.UTF8.GetByteCount(postdata);
			Stream str = rq.GetRequestStream();
			str.Write(Encoding.UTF8.GetBytes(postdata), 0, Encoding.UTF8.GetByteCount(postdata));
			
			HttpWebResponse resp = (HttpWebResponse)rq.GetResponse();
			string result = ReadAllText(resp.GetResponseStream());
			cookies.Add(resp.Cookies);
			return result;
		}

		private HttpWebRequest CreateGetRequest(string uri) 
		{
			HttpWebRequest result = (HttpWebRequest)WebRequest.Create(uri);
			result.UserAgent = "WikiAccess library v" + Utils.Version.ToString();
			result.Proxy.Credentials = CredentialCache.DefaultCredentials;
			result.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			result.CookieContainer = cookies;
			return result;
		}

		private HttpWebRequest CreatePostRequest(string uri) 
		{
			HttpWebRequest result = CreateGetRequest(uri);
			result.AllowAutoRedirect = false;
			result.Method = "POST";
			return result;
		}

		private static string CommitValue(string boundary, string key, string value) 
		{
			string result = "--" + boundary + "\r\n";
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

		public byte[] DownloadBinary(string pgname)
		{
			return DownloadBinaryFullUrl(baseUri + "/" + pgname);
		}

		/// <summary>
		/// Downloads page via WebRequest.
		/// Note: this method is blocking
		/// </summary>
		/// <param name="pgname">Page name</param>
		/// <returns>Page content</returns>
		public byte[] DownloadBinaryFullUrl(string pgname)
		{
			HttpWebRequest rq = CreateGetRequest(pgname);

			return ReadAllBytes(rq.GetResponse().GetResponseStream());
		}

		private static byte[] ReadAllBytes(Stream stream) 
		{
			int cbyte;
			List<Byte> result = new List<byte>();
			while((cbyte = stream.ReadByte()) != -1) {
				result.Add((byte)cbyte);
			}
			return result.ToArray();
		}
		
		private static string ReadAllText(Stream stream) 
		{
			return new StreamReader(stream, Encoding.UTF8).ReadToEnd();
		}

		public void ClearCookies() 
		{
			cookies = new CookieContainer();
		}

		#region IDisposable Members

		/// <summary>
		/// Release WebBrowser control
		/// </summary>
		public void Dispose()
		{
			//wb.Dispose();
		}

		#endregion

		#region Obsolete members, can be deleted anytime

		/// <summary>
		/// Initializes new instance of AccessBrowser
		/// </summary>
		/// <param name="wiki">Wiki to work with</param>
		[Obsolete("Please use constructor with string argument")]
		public AccessBrowser(Wiki wiki)
			: this(wiki.WikiURI) {
		}

		/// <summary>
		/// Parses API timestamp
		/// </summary>
		/// <param name="p">API timestamp in string</param>
		/// <returns>Result in DateTime</returns>
		[Obsolete("Please use DateTime.Parse(time).ToUniversalTime() instead.")]
		public DateTime ParseAPITimestamp(string p)
		{
			return DateTime.Parse(p).ToUniversalTime();
		}

		/// <summary>
		/// Encodes URL
		/// </summary>
		/// <param name="str">String to encode</param>
		/// <returns>Encoded URL</returns>
		[Obsolete("Please use HttpUtility.UrlEncode(url) instead")]
		public string EncodeUrl(string str) {
			return HttpUtility.UrlEncode(str);
		}

		#endregion
	}
}
