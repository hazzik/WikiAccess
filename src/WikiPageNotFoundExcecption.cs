namespace WikiTools.Access
{
    /// <summary>
    /// Is thrown when page doesn't exists
    /// </summary>
    public class WikiPageNotFoundExcecption : WikiException
    {
        /// <summary>
        /// Initializes new instance of WikiPageNotFoundExcecption object
        /// </summary>
        public WikiPageNotFoundExcecption()
        {
        }

        /// <summary>
        /// Initializes new instance of WikiPageNotFoundExcecption object
        /// </summary>
        /// <param name="message">Message of exception</param>
        public WikiPageNotFoundExcecption(string message) : base(message)
        {
        }
    }
}