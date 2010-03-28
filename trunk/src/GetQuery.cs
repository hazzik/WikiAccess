using System.Net;

namespace WikiTools.Web
{
    public class GetQuery : Query
    {
        public GetQuery(string uri, CookieContainer cookies)
            : base(uri, cookies)
        {
        }
    }
}