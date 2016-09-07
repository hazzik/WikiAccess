using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiTools.Access;
using Xunit;

namespace WikiAccess.Tests
{
    public class PageListTests
    {
        [Fact]
        public void X()
        {
            Wiki w = new Wiki("http://en.wikipedia.org/w/");
            var page = w.GetPage("Main_Page");
            {
                
            }
        }
    }
}
