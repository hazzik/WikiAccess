using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;

namespace WikiTools.Access
{
    /// <summary>
    /// Class to detect changes to wiki
    /// </summary>
    public class RecentChangesWatcher
    {
        private int lastrc;
        private readonly Thread t;
        private int updateTime = 5;
        internal Wiki w;

        public RecentChangesWatcher(Wiki w)
        {
            this.w = w;
            t = new Thread(ThreadProc);
        }

        public int UpdateTime
        {
            get { return updateTime; }
            set
            {
                if (value > 0)
                    updateTime = value;
            }
        }

        public void Start()
        {
            if (t.ThreadState != ThreadState.Running)
                t.Start();
        }

        public void Stop()
        {
            t.Abort();
        }

        private void ThreadProc()
        {
            lastrc = GetRecentChanges()[0].RCID;
            while (true)
            {
                RecentChange[] changes = GetRecentChanges();
                Array.Reverse(changes);
                foreach (RecentChange change in changes)
                {
                    if (change.RCID <= lastrc)
                        continue;
                    lastrc = change.RCID;
                    var eea = new EditEventArgs(change);
                    if (OnEdit != null)
                        OnEdit((object) this, eea);
                }
                Thread.Sleep(new TimeSpan(0, 0, updateTime));
            }
        }

        public RecentChange[] GetRecentChanges()
        {
            const string page = Web.Query.RecentChanges;
            var doc = new XmlDocument();
            doc.Load(w.ab.HttpClient.GetStreamAsync(page).Result);
            XmlNodeList nodes = doc.GetElementsByTagName("rc");

            var changes = new List<RecentChange>();
            foreach (XmlNode node in nodes)
            {
                changes.Add(ParseReventChange((XmlElement) node));
            }
            return changes.ToArray();
        }

        private static RecentChange ParseReventChange(XmlElement element)
        {
            var result = new RecentChange();
            result.Type = (RecentChangeType) Enum.Parse(typeof (RecentChangeType), element.GetAttribute("type"), true);
            result.Page = element.Attributes["title"].Value;
            result.RCID = int.Parse(element.GetAttribute("rcid"));
            result.OldRevisionID = int.Parse(element.GetAttribute("old_revid"));
            result.RevisionID = int.Parse(element.GetAttribute("revid"));
            result.Bot = element.HasAttribute("bot");
            result.Minor = element.HasAttribute("minor");
            result.New = element.HasAttribute("new");
            result.OldSize = int.Parse(element.GetAttribute("oldlen"));
            result.NewSize = int.Parse(element.GetAttribute("newlen"));
            result.User = element.GetAttribute("user");
            result.Time = DateTime.Parse(element.GetAttribute("timestamp")).ToUniversalTime();
            result.Comment = element.HasAttribute("comment") ? element.GetAttribute("comment") : "";
            return result;
        }

        public event EditEventHandler OnEdit;
    }
}