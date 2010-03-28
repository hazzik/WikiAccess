namespace WikiTools.Access
{
    /// <summary>
    /// Type of change
    /// </summary>
    public enum RecentChangeType
    {
        /// <summary>
        /// An edit to a page
        /// </summary>
        Edit,
        /// <summary>
        /// New page created
        /// </summary>
        New,
        /// <summary>
        /// Log action like page move of image upload
        /// </summary>
        Log,
    }
}