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
using System.Web;
using WikiTools.Web;

namespace WikiTools.Access
{
	/// <summary>
	/// Provides access to wiki via WebRequest
	/// </summary>
	public class AccessBrowser : IDisposable
	{
		private readonly string baseUri;
		private CookieContainer cookies = new CookieContainer();
		private string cpagename = "";
		private string cpagetext = "";

		/// <summary>
		/// Initializes new instance of AccessBrowser for the specified URI
		/// </summary>
		/// <param name="uri">Uniform Resource Identifier</param>
		public AccessBrowser(string uri)
		{
			baseUri = uri;
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
			: this(wiki.WikiURI)
		{
		}

		/// <summary>
		/// Allows to change current page
		/// </summary>
		[Obsolete]
		public string PageName
		{
			get { return cpagename; }
			set
			{
				cpagename = baseUri + "/" + value;
				cpagetext = CreateGetQuery(value).DownloadText();
			}
		}

		/// <summary>
		/// Current page text
		/// </summary>
		[Obsolete]
		public string PageText
		{
			get { return cpagetext; }
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
		/// Downloads page via WebRequest
		/// </summary>
		/// <param name="pgname">Page name</param>
		/// <returns>Page content</returns>
		[Obsolete("Please use instance of GetQuery class instead")]
		public string DownloadPage(string pgname)
		{
			return DownloadPageFullUrl(baseUri + "/" + pgname);
		}

		/// <summary>
		/// Downloads page via WebRequest
		/// </summary>
		/// <param name="pgname">URL</param>
		/// <returns>Page content</returns>
		[Obsolete("Please use instance of GetQuery class instead")]
		public string DownloadPageFullUrl(string pgname)
		{
			cpagename = pgname;
			return cpagetext = CreateGetQueryFullUrl(pgname).DownloadText();
		}

		/// <summary>
		/// Sends a HTTP request using POST method and multipart/form-data content type
		/// </summary>
		/// <param name="pgname">Page name</param>
		/// <param name="data">Post data</param>
		/// <returns>HTTP response</returns>
		[Obsolete("Please use instance of PostQuery class instead")]
		public string PostQuery(string pgname, IDictionary<string, string> data)
		{
			cpagename = pgname;
			return cpagetext = CreatePostQueryFullUrl(baseUri + "/" + pgname, data).DownloadText();
		}

		[Obsolete("Please use instance of GetQuery class instead")]
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
		[Obsolete("Please use instance of GetQuery class instead")]
		public byte[] DownloadBinaryFullUrl(string pgname)
		{
			return CreateGetQueryFullUrl(pgname).DownloadBinary();
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
		public string EncodeUrl(string str)
		{
			return HttpUtility.UrlEncode(str);
		}

		#endregion

		public void ClearCookies()
		{
			cookies = new CookieContainer();
		}

		public Query CreateGetQuery(string page)
		{
			return new GetQuery(baseUri + '/' + page, cookies);
		}

		public Query CreateGetQueryFullUrl(string uri)
		{
			return new GetQuery(uri, cookies);
		}

		public Query CreatePostQuery(string page)
		{
			return new PostQuery(baseUri + '/' + page, cookies);
		}

		public Query CreatePostQueryFullUrl(string uri)
		{
			return new PostQuery(uri, cookies);
		}

		public Query CreatePostQuery(string page, IDictionary<string, string> data)
		{
			return new PostQuery(baseUri + '/' + page, cookies, data);
		}

		public Query CreatePostQueryFullUrl(string uri, IDictionary<string, string> data)
		{
			return new PostQuery(uri, cookies, data);
		}
	}
}