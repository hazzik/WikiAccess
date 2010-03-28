namespace WikiTools.Access
{
    /// <summary>
    /// Is thrown when MediaWiki doesn't support some features
    /// </summary>
    public class WikiNotSupportedException : WikiException
    {
        /// <summary>
        /// Initializes new instance of WikiNotSupportedException object
        /// </summary>
        public WikiNotSupportedException()
        {
        }

        /// <summary>
        /// Initializes new instance of WikiNotSupportedException object
        /// </summary>
        /// <param name="message">Message of exception</param>
        public WikiNotSupportedException(string message) : base(message)
        {
        }
    }
}