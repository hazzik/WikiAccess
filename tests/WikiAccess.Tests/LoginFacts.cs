using Moq;
using WikiTools.Access;
using Xunit;

namespace WikiAccess.Tests
{
    public class LoginFacts
    {
        [Fact]
        public void LogInSuccess()
        {
            const string result =
                @"<api>
    <login 
        result=""Success"" 
        lguserid=""123456"" 
        lgusername=""testuser"" 
        lgtoken=""5e18270cadbb2e03e74d6cb578c468a8"" 
        cookieprefix="""" 
        sessionid=""56ced5253e58836af92fb6cfe1781118"" />
</api>";

            var wiki = new Wiki(new StubAccessBrowser("http://localost/w", result));
            var loggedIn = wiki.Login("testuser", "testpassword");
            Assert.True(loggedIn);
        }

        [Fact]
        public void LogInWithNonExistingUserReturnsFalse()
        {
            const string result = @"<api>
  <login result=""NotExists"" />
</api>";

            var wiki = new Wiki(new StubAccessBrowser("http://localost/w", result));
            var loggedIn = wiki.Login("testuser", "testpassword");
            Assert.False(loggedIn);
        }

        [Fact]
        public void LogInWithIncorretPasswordReturnsFalse()
        {
            const string result = @"<api>
  <login result=""WrongPass"" />
</api>";

            var wiki = new Wiki(new StubAccessBrowser("http://localost/w", result));
            var loggedIn = wiki.Login("HazzikBot", "testpassword");
            Assert.False(loggedIn);
        } 
    }
}