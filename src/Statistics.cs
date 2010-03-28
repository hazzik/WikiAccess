namespace WikiTools.Access
{
    /// <summary>
    /// Statistics of wiki
    /// </summary>
    public struct Statistics
    {
        /// <summary>
        /// Count of sysops
        /// </summary>
        public int Admins;

        /// <summary>
        /// Count of revisions
        /// </summary>
        public int Edits;

        /// <summary>
        /// Count of articles
        /// </summary>
        public int GoodPages;

        /// <summary>
        /// Count of images
        /// </summary>
        public int Images;

        /// <summary>
        /// Size of job queue
        /// </summary>
        public int Jobs;

        /// <summary>
        /// Count of total pages
        /// </summary>
        public int TotalPages;

        /// <summary>
        /// Count of users
        /// </summary>
        public int Users;

        /// <summary>
        /// Count of views
        /// </summary>
        public int Views;
    }
}