using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WikiTools.Access
{
    partial class Wiki
    {
        /* TODO: doesn't work
         * public string ExpandTemplates(string text, string pagetitle, bool removeComments)
        {
            ab.PageName = "index.php?title=Special:Expandtemplates";
            ab.SetTextboxField("contexttitle", pagetitle);
            ab.SetTextboxField("input", text);
            ab.SetCheckbox("removecomments", removeComments);
            HtmlElementCollection hec = ab.WebBrowser.Document.GetElementsByTagName("input");
            foreach (HtmlElement helem in hec)
            {
                if (helem.GetAttribute("type") == "submit")
                {
                    helem.InvokeMember("click");
                    ab.Wait();
                    break;
                }
            }
            return ab.GetTextboxField("output");
         */
    }
}
