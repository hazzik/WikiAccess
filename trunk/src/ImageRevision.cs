using System;

namespace WikiTools.Access
{
    /// <summary>
    /// Image revison
    /// </summary>
    public struct ImageRevision
    {
        /// <summary>
        /// Uploader
        /// </summary>
        public string Author;

        /// <summary>
        /// Revision comment
        /// </summary>
        public string Comment;

        /// <summary>
        /// Image height (0 for non-images)
        /// </summary>
        public int Height;

        /// <summary>
        /// Image name
        /// </summary>
        public string Image;

        /// <summary>
        /// SHA1 hash of image
        /// </summary>
        public string Sha1;

        /// <summary>
        /// Image size
        /// </summary>
        public long Size;

        /// <summary>
        /// Version upload Time
        /// </summary>
        public DateTime Time;

        /// <summary>
        /// Image raw url
        /// </summary>
        public string Url;

        /// <summary>
        /// Image width (0 for non-images)
        /// </summary>
        public int Width;

        /// <summary>
        /// Wiki that contains image
        /// </summary>
        public Wiki Wiki;

        public byte[] Download()
        {
            //return Wiki.ab.DownloadBinaryFullUrl(Url);
            throw new NotImplementedException();
        }
    }
}