namespace WikiTools.Access
{
    /// <summary>
    /// Is thrown when you haven't got permissions to do some actions
    /// </summary>
    public class WikiPermissionsExpection : WikiException
    {
        /// <summary>
        /// Initializes new instance of WikiPermissionsExpection object
        /// </summary>
        public WikiPermissionsExpection()
        {
        }

        /// <summary>
        /// Initializes new instance of WikiPermissionsExpection object
        /// </summary>
        /// <param name="message">Message of exception</param>
        public WikiPermissionsExpection(string message) : base(message)
        {
        }
    }
}