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
using System.Xml;

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
		private bool infoLoaded = false;
		private ImageRepositoryType repotype = ImageRepositoryType.Local;
		private bool existsLocally = false;

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
			if (!wiki.Capabilities.HasFilePath) throw new WikiNotSupportedException("FilePath extension is needed");
			return ab.DownloadBinary("index.php?title=Special:Filepath/" + ab.EncodeUrl(name));
		}
		
		public void LoadInfo()
		{
			string page_text 
				= ab.DownloadPage("api.php?action=query&prop=imageinfo&titles=Image:" + ab.EncodeUrl(name) +
				"&iiprop=timestamp|user|comment|url|size|sha1|metadata&iihistory&format=xml");
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(page_text);
			XmlElement pageelem = (XmlElement)doc.GetElementsByTagName("page")[0];
			existsLocally = !pageelem.HasAttribute("missing");
			repotype = ParseRepoType(pageelem.Attributes["imagerepository"].Value);
			infoLoaded = true;
		}
		
		ImageRepositoryType ParseRepoType(string type)
		{
			switch(type)
			{
				case "shared":
					return ImageRepositoryType.Shared;
				case "local":
				default:
					return ImageRepositoryType.Local;
			}
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

		/// <summary>
		/// Computes SHA1 hash using .NET and converts it to string
		/// </summary>
		/// <param name="img">Image content</param>
		/// <returns>String-formatted SHA1 hash</returns>
		public static string CalculateSHA1Hash(byte[] img)
		{
			SHA1CryptoServiceProvider scsp = new SHA1CryptoServiceProvider();
			byte[] hash = scsp.ComputeHash(img);
			string result = "";
			foreach (byte cbyte in hash) result += cbyte.ToString("X");
			return result;
		}
		
		public ImageRepositoryType RepositoryType
		{
			get
			{
				if (!infoLoaded)
					LoadInfo();
				return repotype;
			}
		}
	}
	
	public enum ImageRepositoryType
	{
		Local,
		Shared
	}
	
	public struct ImageRevision
	{
		public Wiki Wiki;
		public string Image;
		public DateTime Time;
		public string Author;
		public ulong Size;
		public int Width;
		public int Height;
		public string Url;
		public string Comment;
		public string Sha1;
		public string Metadata;
	}
}
