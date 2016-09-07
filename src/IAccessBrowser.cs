using System;
using System.Net.Http;
using WikiTools.Web;

namespace WikiTools.Access
{
    public interface IAccessBrowser
    {
        void ClearCookies();

        IQuery CreateGetQuery(string page);
        IQuery CreateGetQueryFullUrl(string uri);
        IQuery CreatePostQuery(string page);
        IQuery CreatePostQueryFullUrl(string uri);
        HttpClient HttpClient { get; }
    }
}