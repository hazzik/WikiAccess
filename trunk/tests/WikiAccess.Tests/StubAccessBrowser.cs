using System;
using System.Collections.Generic;
using WikiTools.Access;
using WikiTools.Web;

namespace WikiAccess.Tests
{
    public class StubAccessBrowser : IAccessBrowser
    {
        private readonly IQuery query;

        public StubAccessBrowser(string uri, string result)
        {
            query = new StubQuery(uri, result);
        }

        public StubAccessBrowser(IQuery query)
        {
            this.query = query;
        }

        #region IAccessBrowser Members

        public bool IsLoggedIn()
        {
            throw new NotImplementedException();
        }

        public void ClearCookies()
        {
        }

        public IQuery CreateGetQuery(string page)
        {
            return query;
        }

        public IQuery CreateGetQueryFullUrl(string uri)
        {
            return query;
        }

        public IQuery CreatePostQuery(string page)
        {
            return query;
        }

        public IQuery CreatePostQueryFullUrl(string uri)
        {
            return query;
        }

        public IQuery CreatePostQuery(string page, IDictionary<string, string> data)
        {
            return query;
        }

        public IQuery CreatePostQueryFullUrl(string uri, IDictionary<string, string> data)
        {
            return query;
        }

        #endregion
    }
}