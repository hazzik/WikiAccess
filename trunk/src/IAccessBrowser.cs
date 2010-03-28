using System;
using System.Collections.Generic;
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
        IQuery CreatePostQuery(string page, IDictionary<string, string> data);
        IQuery CreatePostQueryFullUrl(string uri, IDictionary<string, string> data);
    }
}