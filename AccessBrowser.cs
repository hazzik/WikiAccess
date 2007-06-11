/**********************************************************************************
 * Message cache functions of WikiAcces Library                                   *
 * Copyright (C) 2007 Vasiliev V. V.                                              *
 *                                                                                *
 * This program is free software; you can redistribute it and/or                  *
 * modify it under the terms of the GNU General Public License                    *
 * as published by the Free Software Foundation; either version 2                 *
 * of the License, or (at your option) any later version.                         *
 *                                                                                *
 * This program is distributed in the hope that it will be useful,                *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of                 *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                  *
 * GNU General Public License for more details.                                   *
 *                                                                                *
 * You should have received a copy of the GNU General Public License              *
 * along with this program; if not, write to the Free Software                    *
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.*
 **********************************************************************************/
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Web;
using System.IO;

namespace WikiTools.Access
{
    /// <summary>
    /// Provides access to wiki via IE and WebRequest
    /// </summary>
	public class AccessBrowser : IDisposable
	{
		WebBrowser wb;
		Wiki wiki;
		string cpagename = "";
		//public bool Shutdown = false;

        Regex APITimestamp = new Regex(@"(\d{4})-(\d\d)-(\d\d)T(\d\d):(\d\d):(\d\d)Z", RegexOptions.Compiled);

        /// <summary>
        /// Initializes new instance of AccessBrowser
        /// </summary>
        /// <param name="wiki">Wiki to work with</param>
		public AccessBrowser(Wiki wiki)
		{
			wb = new WebBrowser();
			this.wiki = wiki;
			wb.ScriptErrorsSuppressed = true;
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
                    wb.AllowNavigation = true;
                    wb.Navigate(wiki.WikiURI + "/" + cpagename);
                    Wait();
                }
			}
		}

        /// <summary>
        /// Checks if we are currently logged in
        /// </summary>
        /// <returns>Login status</returns>
		public bool IsLoggedIn()
		{
			return !wb.DocumentText.Contains("var wgUserName = null;");
		}

		/// <summary>
		/// Sets textbox field content
		/// </summary>
		/// <param name="name">Name (ID of textbox)</param>
		/// <param name="value">Vaule to set</param>
		/// <returns>Existance of textbox</returns>
		public bool SetTextboxField(string name, string value)
		{
			if (wb.Document.GetElementById(name) == null) return false;
            wb.Document.GetElementById(name).InnerText = value;
			return true;
		}

		/// <summary>
		/// Gets textbox field content
		/// </summary>
		/// <param name="name">Name (ID of textbox)</param>
		/// <returns>Vaule of textbox (null if textbox doesn't exist)</returns>
		public string GetTextboxField(string name)
		{
			if (wb.Document.GetElementById(name) == null) return null;
			return wb.Document.GetElementById(name).InnerText;
		}

		/// <summary>
		/// Sets checkbox value
		/// </summary>
		/// <param name="name">Name (ID of checkbox)</param>
		/// <param name="value">Vaule of checkbox</param>
		/// <returns>Existance of checkbox</returns>
		public bool SetCheckbox(string name, bool value)
		{
			if (wb.Document.GetElementById(name) == null) return false;
			wb.Document.GetElementById(name).SetAttribute("checked", (value ? "checked" : ""));
			return true;
		}

		/// <summary>
		/// Click th specified button
		/// </summary>
		/// <param name="name">Name of button</param>
		/// <returns>Existance of button</returns>
		public bool ClickButton(string name)
		{
			if (wb.Document.GetElementById(name) == null) return false;
			wb.Document.GetElementById(name).InvokeMember("click");
			Wait();
			return true;
		}

		/// <summary>
		/// Waits until page loaded
		/// </summary>
		public void Wait()
		{
            while (wb.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();
		}

		/// <summary>
		/// Current page text
		/// </summary>
		public string PageText
		{
			get
			{
				return wb.DocumentText;
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
		/// Downloads page via WebRequest.
		/// Note: this method is blocking
		/// </summary>
		/// <param name="pgname">Page name</param>
		/// <returns>Page content</returns>
        public string DownloadPage(string pgname)
        {
			string result;
            WebRequest rq = WebRequest.Create(wiki.WikiURI + "/" + pgname);
            rq.Proxy.Credentials = CredentialCache.DefaultCredentials;
			Utils.DoEvents();
            result = new StreamReader(rq.GetResponse().GetResponseStream(), Encoding.UTF8).ReadToEnd();
			Utils.DoEvents();
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
				if (DateTime.Now.Ticks % 10 == 0) Utils.DoEvents();
            }
            return result.ToArray();
        }

		/// <summary>
		/// Instance of WebBrowser
		/// </summary>
        public WebBrowser WebBrowser
        {
            get 
            { 
                return wb; 
            }
        }

		#region IDisposable Members

		/// <summary>
		/// Release WebBrowser control
		/// </summary>
		public void Dispose()
		{
			wb.Dispose();
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

		/// <summary>
		/// Updates current page
		/// </summary>
        public void Update()
        {
            wb.Update();
            Wait();
        }
    }
}
