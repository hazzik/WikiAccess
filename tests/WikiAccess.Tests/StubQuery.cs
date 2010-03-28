using System;
using System.IO;
using System.Text;
using WikiTools.Web;

namespace WikiAccess.Tests
{
    public class StubQuery : IQuery
    {
        private readonly string _uri;
        private readonly string _result;

        public StubQuery(string uri, string result)
        {
            _uri = uri;
            _result = result;
        }

        public Uri Uri
        {
            get { return new Uri(_uri); }
        }

        public IQuery Add(string key, string value)
        {
            return this;
        }

        public string DownloadText()
        {
            return _result;
        }

        public byte[] DownloadBinary()
        {
            throw new NotImplementedException();
        }

        public Stream GetResponseStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_result));
        }

        public TextReader GetTextReader()
        {
            return new StringReader(_result);
        }
    }
}