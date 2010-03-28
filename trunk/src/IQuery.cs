using System;
using System.IO;

namespace WikiTools.Web
{
    public interface IQuery
    {
        Uri Uri { get; }
        IQuery Add(string key, string value);
        string DownloadText();
        byte[] DownloadBinary();
        Stream GetResponseStream();
        TextReader GetTextReader();
    }
}