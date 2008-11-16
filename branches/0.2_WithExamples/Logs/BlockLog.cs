using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WikiTools.Access
{
	public class BlockLog
	{
		Wiki wiki;
		AccessBrowser ab;
		List<BlockLogEntry> entries;

		public BlockLog(Wiki wiki)
		{
			this.wiki = wiki;
			ab = wiki.ab;
			entries = new List<BlockLogEntry>();
		}

		public void Load(string adminname)
		{
			string pg = ab.DownloadPage("api.php?action=query&list=logevents&letype=block&leuser=" 
				+ ab.EncodeUrl(adminname) + "&lelimit=500&format=xml");
			LoadFromXML(pg);
			for (; ; )
			{
				if (entries.Count % 500 != 0) break;
				List<BlockLogEntry> tmp_entries = entries;
				entries = new List<BlockLogEntry>();
				//
			}
		}

		private void LoadFromXML(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlElement root = (XmlElement)doc.GetElementsByTagName("logevents")[0];
			foreach (XmlNode cnode in root.ChildNodes)
			{
				if (!(cnode.NodeType == XmlNodeType.Element && ((XmlElement)cnode).Name == "item")) continue;
				XmlElement celem = (XmlElement)cnode;
				BlockLogEntry centry = new BlockLogEntry();
				centry.Action = StringToBlockAction(celem.Attributes["action"].Value);
				centry.BlockedBy = celem.Attributes["user"].Value;
				centry.BlockTime = DateTime.Parse(celem.Attributes["timestamp"].Value).ToUniversalTime();
				if (centry.Action == BlockAction.Block) centry.Duration = celem.FirstChild.FirstChild.Value;
				centry.UserName = celem.Attributes["title"].Value.Split(new char[] { ":"[0] }, 2)[1];
				if (celem.HasAttribute("comment")) centry.Comment = celem.Attributes["comment"].Value;
				entries.Add(centry);
			}
		}

		private BlockAction StringToBlockAction(String str)
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

		public BlockLogEntry[] Entries
		{
			get
			{
				return entries.ToArray();
			}
		}
	}

	public struct BlockLogEntry
	{
		public string UserName;
		public BlockAction Action;
		public string BlockedBy;
		public DateTime BlockTime;
		public string Duration;
		public string Comment;

		public override string ToString()
		{
			return BlockedBy + " " + Action.ToString().ToLower() + "ed " + UserName + " for " + Duration + " at " + BlockTime.ToString()
				+ " (reason: " + Comment + ")";
		}
	}

	public enum BlockAction
	{
		Block,
		Unblock,
	}
}
