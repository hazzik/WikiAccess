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
using System.Linq;
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
		/// Revisions of this image
		/// Revisions are ordered descending (more recent first)
		/// Revisionhistory is limited to 500 entries.
		/// </summary>
		public ImageRevision[] Revisions
		{
			get
			{
				if (!infoLoaded)
					LoadInfo();
				return revs;
			}
		}

		/// <summary>
		/// Current revision of image
		/// </summary>
		public ImageRevision CurrentRevision
		{
			get { return Revisions[0]; }
		}

		/// <summary>
		/// Downloads image from wiki.
		/// </summary>
		/// <returns>Image</returns>
		public byte[] Download()
		{
			return CurrentRevision.Download();
		}

		/// <summary>
		/// Loads information about image. Called automatically, you should use it only for reloading info,
		/// </summary>
		public void LoadInfo()
		{
			string pgname = string.Format(Web.Query.ImageInfo, HttpUtility.UrlEncode(name));
			var doc = new XmlDocument();
			doc.LoadXml(wiki.ab.CreateGetQuery(pgname).DownloadText());
			var pageelem = (XmlElement) doc.GetElementsByTagName("page")[0];
			existsLocaly = !pageelem.HasAttribute("missing");
			repotype = ParseRepoType(pageelem.Attributes["imagerepository"].Value);

			XmlNodeList revs_ii = pageelem.GetElementsByTagName("ii");
			revs = (from XmlNode cnode in revs_ii
					select ParseImageRevision(cnode)).ToArray();

			infoLoaded = true;
		}

		private ImageRevision ParseImageRevision(XmlNode element)
		{
			var result = new ImageRevision();
			result.Wiki = wiki;
			result.Name = name;
			result.Time = DateTime.Parse(element.Attributes["timestamp"].Value).ToUniversalTime();
			result.Author = element.Attributes["user"].Value;
			result.Size = Int64.Parse(element.Attributes["size"].Value);
			result.Width = Int32.Parse(element.Attributes["width"].Value);
			result.Height = Int32.Parse(element.Attributes["height"].Value);
			result.Comment = element.Attributes["comment"].Value;
			result.Url = element.Attributes["url"].Value;
			result.Sha1 = element.Attributes["sha1"].Value;
			result.Metadata = element.Attributes["metadata"].Value;
			result.Mime = element.Attributes["mime"].Value;
			result.Bitdepth = Int32.Parse(element.Attributes["bitdepth"].Value);
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
}