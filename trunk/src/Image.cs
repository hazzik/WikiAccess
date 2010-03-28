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
using System.Security.Cryptography;
using System.Web;
using System.Xml;

namespace WikiTools.Access
{
	/// <summary>
	/// Provides access to images
	/// </summary>
	public class Image
	{
		private bool existsLocaly;
		private bool infoLoaded;
		private readonly string name;
		private ImageRepositoryType repotype = ImageRepositoryType.Local;
		private ImageRevision[] revs;
		private readonly Wiki wiki;

		/// <summary>
		/// Initializes Image object
		/// </summary>
		/// <param name="wiki">Wiki to use</param>
		/// <param name="name">Image name</param>
		public Image(Wiki wiki, string name)
		{
			this.wiki = wiki;
			this.name = name;
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
				return existsLocaly;
			}
		}

		/// <summary>
		/// Revsions of this image
		/// </summary>
		public ImageRevision[] Revisions
		{
			get { return revs; }
		}

		/// <summary>
		/// Current revision of image
		/// </summary>
		public ImageRevision CurrentRevision
		{
			get { return revs[0]; }
		}

		/// <summary>
		/// Downloads image from wiki.
		/// </summary>
		/// <returns>Image</returns>
		public byte[] Download()
		{
			if (wiki.Capabilities.HasFilePath)
				return wiki.ab.DownloadBinary("index.php?title=Special:Filepath/" + HttpUtility.UrlEncode(name));
			else
				return CurrentRevision.Download();
		}

		/// <summary>
		/// Loads information about image. Called automatically, you should use it only for reloading info,
		/// </summary>
		public void LoadInfo()
		{
			string pgname = "api.php?action=query&prop=imageinfo&titles=Image:" + HttpUtility.UrlEncode(name) +
			                "&iiprop=timestamp|user|comment|url|size|sha1&iihistory&format=xml";
			var doc = new XmlDocument();
			doc.Load(wiki.ab.CreateGetQuery(pgname).GetResponseStream());
			var pageelem = (XmlElement) doc.GetElementsByTagName("page")[0];
			existsLocaly = !pageelem.HasAttribute("missing");
			repotype = ParseRepoType(pageelem.Attributes["imagerepository"].Value);

			var iielem = (XmlElement) pageelem.GetElementsByTagName("imageinfo")[0];
			XmlNodeList revs_ii = pageelem.GetElementsByTagName("ii");
			var revs_temp = new List<ImageRevision>();
			foreach (XmlNode cnode in revs_ii)
			{
				revs_temp.Add(ParseImageRevision((XmlElement) cnode));
			}
			revs = revs_temp.ToArray();

			infoLoaded = true;
		}

		private ImageRevision ParseImageRevision(XmlElement element)
		{
			var result = new ImageRevision();
			result.Wiki = wiki;
			result.Image = name;
			result.Time = DateTime.Parse(element.Attributes["timestamp"].Value).ToUniversalTime();
			result.Author = element.Attributes["user"].Value;
			result.Size = Int64.Parse(element.Attributes["size"].Value);
			result.Width = Int32.Parse(element.Attributes["width"].Value);
			result.Height = Int32.Parse(element.Attributes["height"].Value);
			result.Url = element.Attributes["url"].Value;
			result.Comment = element.Attributes["comment"].Value;
			result.Sha1 = element.Attributes["sha1"].Value;
			return result;
		}

		private static ImageRepositoryType ParseRepoType(string type)
		{
			switch (type)
			{
				case "shared":
					return ImageRepositoryType.Shared;
				case "local":
				default:
					return ImageRepositoryType.Local;
			}
		}

		/// <summary>
		/// Computes MD5 hash using .NET and converts it to string
		/// </summary>
		/// <param name="img">Image content</param>
		/// <returns>String-formatted MD5 hash</returns>
		public static string CalculateMD5Hash(byte[] img)
		{
		    return MD5.Create().ComputeHash(img).BinaryToHexString();
		}

		/// <summary>
		/// Computes SHA1 hash using .NET and converts it to string
		/// </summary>
		/// <param name="img">Image content</param>
		/// <returns>String-formatted SHA1 hash</returns>
		public static string CalculateSHA1Hash(byte[] img)
		{
		    return SHA1.Create().ComputeHash(img).BinaryToHexString();
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
		/// Uploader
		/// </summary>
		public string Author;

		/// <summary>
		/// Revision comment
		/// </summary>
		public string Comment;

		/// <summary>
		/// Image height (0 for non-images)
		/// </summary>
		public int Height;

		/// <summary>
		/// Image name
		/// </summary>
		public string Image;

		/// <summary>
		/// SHA1 hash of image
		/// </summary>
		public string Sha1;

		/// <summary>
		/// Image size
		/// </summary>
		public long Size;

		/// <summary>
		/// Version upload Time
		/// </summary>
		public DateTime Time;

		/// <summary>
		/// Image raw url
		/// </summary>
		public string Url;

		/// <summary>
		/// Image width (0 for non-images)
		/// </summary>
		public int Width;

		/// <summary>
		/// Wiki that contains image
		/// </summary>
		public Wiki Wiki;

		public byte[] Download()
		{
			return Wiki.ab.DownloadBinaryFullUrl(Url);
		}
	}
}