using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace WikiTools.Access
{
	public class BlockLog
	{
		Wiki wiki;
		AccessBrowser ab {
			get { return wiki.ab; }
		}
		List<BlockLogEntry> entries = new List<BlockLogEntry>();

		public BlockLog(Wiki wiki)
		{
			this.wiki = wiki;
		}

		public void Load(string adminname)
		{
			string pg = ab.DownloadPage("api.php?action=query&list=logevents&letype=block&leuser=" 
				+ HttpUtility.UrlEncode(adminname) + "&lelimit=500&format=xml");
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
				entries.Add(ParseBlockLogEntry((XmlElement)cnode));
			}
		}

		private BlockLogEntry ParseBlockLogEntry(XmlElement element) 
		{
			BlockLogEntry result = new BlockLogEntry();
			result.Action = StringToBlockAction(element.Attributes["action"].Value);
			result.BlockedBy = element.Attributes["user"].Value;
			result.BlockTime = DateTime.Parse(element.Attributes["timestamp"].Value).ToUniversalTime();
			if(result.Action == BlockAction.Block)
				result.Duration = element.FirstChild.FirstChild.Value;
			result.UserName = element.Attributes["title"].Value.Split(new char[] { ":"[0] }, 2)[1];
			if(element.HasAttribute("comment"))
				result.Comment = element.Attributes["comment"].Value;
			return result;
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
