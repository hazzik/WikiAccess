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
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Xml;
using WikiTools.Web;

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
		private string redirectsOn;

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

		public string RedirectsOn
		{
			get
			{
				if (!infoLoaded)
					LoadInfo();
				return redirectsOn;
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
			string pgname = string.Format(Query.ImageInfo, HttpUtility.UrlEncode(name));
			var doc = new XmlDocument();
			doc.LoadXml(wiki.ab.CreateGetQuery(pgname).DownloadText());
			var pageelem = (XmlElement) doc.GetElementsByTagName("page")[0];
			existsLocaly = !pageelem.HasAttribute("missing");
			repotype = ParseRepoType(pageelem.Attributes["imagerepository"].Value);

			XmlNodeList revs_ii = pageelem.GetElementsByTagName("ii");
			revs = (from XmlNode cnode in revs_ii
					select ParseImageRevision(cnode)).ToArray();

			var redirect = doc.CreateNavigator().SelectSingleNode("//api/query/redirects/r/@to");
			if(redirect != null)
				redirectsOn = redirect.Value;
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
			// Metadata can be an attribute of ii node
			// but also a childnode (e.g. for gif Files):
			//<metadata>
			//    <metadata name="frameCount" value="1" /
			//    <metadata name="looped" value="" />
			//    <metadata name="duration" value="0" />
			//</metadata>
			if(element.HasChildNodes)
			{
				//var md = element.CreateNavigator().SelectSingleNode("metadata");
				List<string> list = new List<string>();
				foreach (var node in element.ChildNodes.OfType<XmlNode>().Where(x => x.Name == "metadata"))
				{
					list.AddRange(from XmlNode cnode in node.ChildNodes
					              select string.Format("{0}:{1}", cnode.Attributes["name"].Value, cnode.Attributes["value"].Value));
				}
				result.Metadata = string.Join("; ", list);
			}
			else
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

		public void Upload(Stream stream, string contentType, string comment = null)
		{
			/* Note from http://www.mediawiki.org/wiki/API:Import:
			 * To import pages, an import token is required. This token is equal to the edit token and
			 * the same for all pages, but changes at every login. */
			string token = wiki.GetPage("Main Page").GetToken("edit");

			using (stream)
			{
				var qry = ((PostQuery) wiki.ab.CreatePostQuery("api.php?format=xml"))
					.AddFile("file", name, contentType, stream)
					.Add("action", "upload")
					.Add("token", token)
					.Add("filename", name)
					.Add("comment", comment);
				string s = qry.DownloadText();
			}
		}
	}
}