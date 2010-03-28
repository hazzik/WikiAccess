using System;

namespace WikiTools.Access
{
    public struct BlockLogEntry
    {
        public BlockAction Action;
        public string BlockedBy;
        public DateTime BlockTime;
        public string Comment;
        public string Duration;
        public string UserName;

        public override string ToString()
        {
            return string.Format("{0} {1}ed {2} for {3} at {4} (reason: {5})", BlockedBy, Action.ToString().ToLower(), UserName, Duration, BlockTime, Comment);
        }
    }
}