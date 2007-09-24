/**********************************************************************************
 * Image class of WikiAccess Library                                              *
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
using System.Security.Cryptography;
using System.Text;

namespace WikiTools.Access
{
	/// <summary>
	/// Provides access to images
	/// </summary>
	public class Image
	{
		private AccessBrowser ab;
		private Wiki wiki;
		private string name;

		/// <summary>
		/// Initializes Image object
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <param name="name">Image name</param>
		public Image(Wiki wiki, string name)
		{
			this.wiki = wiki;
			this.name = name;
			ab = wiki.ab;
		}

		/// <summary>
		/// Downloads image from wiki. Needs Special:Filepath
		/// </summary>
		/// <returns>Image</returns>
		public byte[] Download()
		{
			if (!wiki.Capabilities.HasFilePath) throw new WikiNotSupportedException();
			return ab.DownloadBinary("index.php?title=Special:Filepath/" + ab.EncodeUrl(name));
		}

		#region Unimplemented Upload method
		/*/// <summary>
		/// ***NOT IMPLEMENTED***
		/// </summary>
		/// <param name="path">Path of file to upload</param>
		/// <param name="fname">Target name of file</param>
		/// <param name="description">File description</param>
		public void Upload(string path, string fname, string description)
		{
			throw new NotImplementedException();
			ab.PageName = "index.php?title=Special:Upload";
			if (!File.Exists(path)) throw new FileNotFoundException("File is not found", path);
			ab.SetValue("wpUploadFile", path);
			ab.SetTextboxField("wpDestFile", fname);
			ab.SetTextboxField("wpUploadDescription", description);
			ab.SetCheckbox("wpWatchthis", false);
			ab.SetCheckbox("wpIgnoreWarning", true);
			ab.ClickButton("wpUpload");
		} */
		#endregion

		/// <summary>
		/// Computes MD5 hash using .NET and converts it to string
		/// </summary>
		/// <param name="img">Image content</param>
		/// <returns>String-formatted MD5 hash</returns>
		public static string CalculateMD5Hash(byte[] img)
		{
			MD5CryptoServiceProvider mcsp = new MD5CryptoServiceProvider();
			byte[] hash = mcsp.ComputeHash(img);
			string result = "";
			foreach (byte cbyte in hash) result += cbyte.ToString("X");
			return result;
		}
	}
}
