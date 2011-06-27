using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace WikiTools.Access
{
	public class BlockLog
	{
		private List<BlockLogEntry> entries = new List<BlockLogEntry>();
		private readonly Wiki wiki;

		public BlockLog(Wiki wiki)
		{
			this.wiki = wiki;
		}

		public BlockLogEntry[] Entries
		{
			get { return entries.ToArray(); }
		}

		public void Load(string adminname)
		{
			string pgname = string.Format(Web.Query.BlockLog, HttpUtility.UrlEncode(adminname));
			var doc = new XmlDocument();
			doc.Load(wiki.ab.CreateGetQuery(pgname).GetResponseStream());
			var root = (XmlElement) doc.GetElementsByTagName("logevents")[0];
			foreach (XmlNode cnode in root.ChildNodes)
			{
				if ((cnode.NodeType == XmlNodeType.Element && cnode.Name == "item"))
				{
					entries.Add(ParseBlockLogEntry((XmlElement) cnode));
				}
			}
			for (;;)
			{
				if (entries.Count%500 != 0) break;
				List<BlockLogEntry> tmp_entries = entries;
				entries = new List<BlockLogEntry>();
				//
			}
		}

		private static BlockLogEntry ParseBlockLogEntry(XmlElement element)
		{
			var result = new BlockLogEntry();
			result.Action = StringToBlockAction(element.Attributes["action"].Value);
			result.BlockedBy = element.Attributes["user"].Value;
			result.BlockTime = DateTime.Parse(element.Attributes["timestamp"].Value).ToUniversalTime();
			if (result.Action == BlockAction.Block)
				result.Duration = element.FirstChild.FirstChild.Value;
			result.UserName = element.Attributes["title"].Value.Split(new[] {':'}, 2)[1];
			if (element.HasAttribute("comment"))
				result.Comment = element.Attributes["comment"].Value;
			return result;
		}

		private static BlockAction StringToBlockAction(String str)
		{
			switch (str.ToLower())
			{
				case "block":
					return BlockAction.Block;
				case "unblock":
					return BlockAction.Unblock;
				default:
					throw new FormatException();
			}
		}
	}
}