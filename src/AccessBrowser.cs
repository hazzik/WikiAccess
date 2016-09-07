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
using System.Net;
using System.Net.Http;
using WikiTools.Web;

namespace WikiTools.Access
{
    /// <summary>
	/// Provides access to wiki via WebRequest
	/// </summary>
	public class AccessBrowser : IDisposable, IAccessBrowser
    {
		private readonly string baseUri;
        private CookieContainer cookies = new CookieContainer();

        /// <summary>
        /// Initializes new instance of AccessBrowser for the specified URI
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        public AccessBrowser(string uri)
        {
            baseUri = uri;

            HttpClient = new HttpClient(new HttpClientHandler
            {
                DefaultProxyCredentials = CredentialCache.DefaultCredentials,
                UseDefaultCredentials = true,
                PreAuthenticate = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                CookieContainer = cookies,
            })
            {

                BaseAddress = new Uri(baseUri),
            };
        }

        /// <summary>
		/// Release WebBrowser control
		/// </summary>
		public void Dispose()
		{
		}

        public void ClearCookies()
		{
			cookies = new CookieContainer();
		}

		public IQuery CreateGetQuery(string page)
		{
			return new GetQuery(baseUri + '/' + page, cookies);
		}

		public IQuery CreateGetQueryFullUrl(string uri)
		{
			return new GetQuery(uri, cookies);
		}

		public IQuery CreatePostQuery(string page)
		{
			return new PostQuery(baseUri + '/' + page, cookies);
		}

		public IQuery CreatePostQueryFullUrl(string uri)
		{
			return new PostQuery(uri, cookies);
		}

        public HttpClient HttpClient { get; }
    }
}