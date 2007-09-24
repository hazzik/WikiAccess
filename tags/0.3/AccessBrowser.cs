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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

namespace WikiTools.Access
{
	/// <summary>
	/// Provides access to wiki via IE and WebRequest
	/// </summary>
	public class AccessBrowser : IDisposable
	{
		Wiki wiki;
		string cpagename = "";
		string cpagetext = "";
		internal CookieCollection cookiesGotInLastQuery = new CookieCollection();
		//public bool Shutdown = false;

		Regex APITimestamp = new Regex(@"(\d{4})-(\d\d)-(\d\d)T(\d\d):(\d\d):(\d\d)Z", RegexOptions.Compiled);

		/// <summary>
		/// Initializes new instance of AccessBrowser
		/// </summary>
		/// <param name="wiki">Wiki to work with</param>
		public AccessBrowser(Wiki wiki)
		{
			//wb = new WebBrowser();
			this.wiki = wiki;
			//wb.ScriptErrorsSuppressed = true;
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
		/// Encodes URL
		/// </summary>
		/// <param name="str">String to encode</param>
		/// <returns>Encoded URL</returns>
		public string EncodeUrl(string str)
		{
			return HttpUtility.UrlEncode(str);
		}

		/// <summary>
		/// Downloads page via WebRequest
		/// </summary>
		/// <param name="pgname">Page name</param>
		/// <returns>Page content</returns>
		public string DownloadPage(string pgname)
		{
			string result;
			HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(wiki.WikiURI + "/" + pgname);
			rq.Proxy.Credentials = CredentialCache.DefaultCredentials;
			rq.UserAgent = "WikiAccess library v" + Utils.Version.ToString();
			rq.CookieContainer = wiki.cookies;
			result = new StreamReader(rq.GetResponse().GetResponseStream(), Encoding.UTF8).ReadToEnd();
			return result;
		}

		/// <summary>
		/// Sends a HTTP request using POST method and multipart/form-data content type
		/// </summary>
		/// <param name="pgname">Page name</param>
		/// <param name="data">Post data</param>
		/// <returns>HTTP response</returns>
		public string PostQuery(string pgname, Dictionary<string, string> data)
		{
			string result;
			HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(wiki.WikiURI + "/" + pgname);
			rq.Proxy.Credentials = CredentialCache.DefaultCredentials;
			rq.UserAgent = "WikiAccess library v" + Utils.Version.ToString();
			rq.CookieContainer = wiki.cookies;
			rq.AllowAutoRedirect = false;
			rq.Method = "POST";
			Random rnd = new Random(); byte[] rndbytes = new byte[1024]; rnd.NextBytes(rndbytes);
			string boundary = "-------" + Image.CalculateMD5Hash(rndbytes);
			rq.ContentType = "multipart/form-data; boundary=" + boundary;
			string postdata = "";
			foreach (KeyValuePair<string, string> kvp in data)
			{
				string ckey = kvp.Key;
				string cvalue = kvp.Value;
				postdata += "--" + boundary + "\r\n";
				postdata += "Content-Disposition: form-data; name=\"" + ckey + "\"\r\n";
				postdata += "Content-Type: text/plain; charset=utf-8\r\n";
				postdata += "\r\n";
				postdata += cvalue + "\r\n";
			}
			postdata = postdata.Substring(0, postdata.Length - 2);
			rq.ContentLength = Encoding.UTF8.GetByteCount(postdata);
			Stream str = rq.GetRequestStream();
			str.Write(Encoding.UTF8.GetBytes(postdata), 0, Encoding.UTF8.GetByteCount(postdata));
			HttpWebResponse resp = (HttpWebResponse)rq.GetResponse();
			result = new StreamReader(resp.GetResponseStream(), Encoding.UTF8).ReadToEnd();
			cookiesGotInLastQuery = resp.Cookies;
			return result;
		}

		/// <summary>
		/// Downloads page via WebRequest.
		/// Note: this method is blocking
		/// </summary>
		/// <param name="pgname">Page name</param>
		/// <returns>Page content</returns>
		public byte[] DownloadBinary(string pgname)
		{
			WebRequest rq = WebRequest.Create(wiki.WikiURI + "/" + pgname);
			List<Byte> result = new List<byte>();
			int cbyte; Stream rpstream = rq.GetResponse().GetResponseStream();
			while ((cbyte = rpstream.ReadByte()) != -1)
			{
				result.Add((byte)cbyte);
			}
			return result.ToArray();
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

		/// <summary>
		/// Parses API timestamp
		/// </summary>
		/// <param name="p">API timestamp in string</param>
		/// <returns>Result in DateTime</returns>
		public DateTime ParseAPITimestamp(string p)
		{
			Match match = APITimestamp.Match(p);
			return new DateTime(
				int.Parse(match.Groups[1].Value),
				int.Parse(match.Groups[2].Value),
				int.Parse(match.Groups[3].Value),
				int.Parse(match.Groups[4].Value),
				int.Parse(match.Groups[5].Value),
				int.Parse(match.Groups[6].Value)
			);
		}
	}
}
