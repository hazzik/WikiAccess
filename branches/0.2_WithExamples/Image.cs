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
		private bool existsLocaly = false;
		private ImageRevision[] revs = null;

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
		/// Downloads image from wiki.
		/// </summary>
		/// <returns>Image</returns>
		public byte[] Download()
		{
			if (wiki.Capabilities.HasFilePath)
				return ab.DownloadBinary("index.php?title=Special:Filepath/" + ab.EncodeUrl(name));
			else
				return CurrentRevision.Download();
		}
		
		/// <summary>
		/// Loads information about image. Called automatically, you should use it only for reloading info,
		/// </summary>
		public void LoadInfo()
		{
			string page_text 
				= ab.DownloadPage("api.php?action=query&prop=imageinfo&titles=Image:" + ab.EncodeUrl(name) +
				"&iiprop=timestamp|user|comment|url|size|sha1&iihistory&format=xml");
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(page_text);
			XmlElement pageelem = (XmlElement)doc.GetElementsByTagName("page")[0];
			existsLocaly = !pageelem.HasAttribute("missing");
			repotype = ParseRepoType(pageelem.Attributes["imagerepository"].Value);

			XmlElement iielem = (XmlElement)pageelem.GetElementsByTagName("imageinfo")[0];
			XmlNodeList revs_ii = pageelem.GetElementsByTagName("ii");
			List<ImageRevision> revs_temp = new List<ImageRevision>();
			foreach (XmlNode cnode in revs_ii)
			{
				XmlElement celem = (XmlElement)cnode;
				ImageRevision crev = new ImageRevision();
				crev.Wiki = wiki;
				crev.Image = name;
				crev.Time = DateTime.Parse(celem.Attributes["timestamp"].Value).ToUniversalTime();
				crev.Author = celem.Attributes["user"].Value;
				crev.Size = Int64.Parse(celem.Attributes["size"].Value);
				crev.Width = Int32.Parse(celem.Attributes["width"].Value);
				crev.Height = Int32.Parse(celem.Attributes["height"].Value);
				crev.Url = celem.Attributes["url"].Value;
				crev.Comment = celem.Attributes["comment"].Value;
				crev.Sha1 = celem.Attributes["sha1"].Value;
				revs_temp.Add(crev);
			}
			revs = revs_temp.ToArray();
			
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
		
		/// <summary>
		/// Repository where image is stored
		/// </summary>
		public ImageRepositoryType RepositoryType
		{
			get
			{
				if (!infoLoaded)
					LoadInfo();
				return repotype;
			}
		}
		
		/// <summary>
		/// Indicates if image exists localy
		/// </summary>
		public bool ExistsLocaly
		{
			get
			{
				if (!infoLoaded)
					LoadInfo();
				return this.existsLocaly;
			}
		}
		
		/// <summary>
		/// Revsions of this image
		/// </summary>
		public ImageRevision[] Revisions
		{
			get
			{
				return revs;
			}
		}
		
		/// <summary>
		/// Current revision of image
		/// </summary>
		public ImageRevision CurrentRevision
		{
			get
			{
				return revs[0];
			}
		}
	}
	
	/// <summary>
	/// Image repository type
	/// </summary>
	public enum ImageRepositoryType
	{
		/// <summary>
		/// Image is stored on local wiki
		/// </summary>
		Local,
		/// <summary>
		/// Image is stored on shared repository, like Wikimedia Commons
		/// </summary>
		Shared
	}
	
	/// <summary>
	/// Image revison
	/// </summary>
	public struct ImageRevision
	{
		/// <summary>
		/// Wiki that contains image
		/// </summary>
		public Wiki Wiki;
		/// <summary>
		/// Image name
		/// </summary>
		public string Image;
		/// <summary>
		/// Version upload Time
		/// </summary>
		public DateTime Time;
		/// <summary>
		/// Uploader
		/// </summary>
		public string Author;
		/// <summary>
		/// Image size
		/// </summary>
		public long Size;
		/// <summary>
		/// Image width (0 for non-images)
		/// </summary>
		public int Width;
		/// <summary>
		/// Image height (0 for non-images)
		/// </summary>
		public int Height;
		/// <summary>
		/// Image raw url
		/// </summary>
		public string Url;
		/// <summary>
		/// Revision comment
		/// </summary>
		public string Comment;
		/// <summary>
		/// SHA1 hash of image
		/// </summary>
		public string Sha1;
		
		public byte[] Download()
		{
			return Wiki.ab.DownloadBinaryFullUrl(Url);
		}
	}
}