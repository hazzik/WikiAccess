using System;

namespace WikiTools.Access
{
    public class EditEventArgs : EventArgs
    {
        private readonly RecentChange rc;

        public EditEventArgs(RecentChange rc)
        {
            this.rc = rc;
        }

        public RecentChange Change
        {
            get { return rc; }
        }
    }
}