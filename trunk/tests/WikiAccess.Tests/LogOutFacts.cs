using Moq;
using WikiTools.Access;
using Xunit;

namespace WikiAccess.Tests
{
    public class LogOutFacts
    {
        [Fact]
        public void LogOutCallsMockBrowserClearCookie()
        {
            var mockAccessBrowser = new Mock<IAccessBrowser>();

            var wiki = new Wiki(mockAccessBrowser.Object);
           
            wiki.Logout();

            mockAccessBrowser.Verify(b => b.ClearCookies());
        }
    }
}