/**********************************************************************************
 * Category class of WikiAcces Library                                            *
 * Copyright (C) 2007 Vasiliev V. V.                                              *
 *                                                                                *
 * This program is free software; you can redistribute it and/or                  *
 * modify it under the terms of the GNU General Public License                    *
 * as published by the Free Software Foundation; either version 2                 *
 * of the License, or (at your option) any later version.                         *
 *                                                                                *
 * This program is distributed in the hope that it will be useful,                *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of                 *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                  *
 * GNU General Public License for more details.                                   *
 *                                                                                *
 * You should have received a copy of the GNU General Public License              *
 * along with this program; if not, write to the Free Software                    *
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.*
 **********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiTools.Access
{
    /// <summary>
    /// Represents a category in wiki
    /// </summary>
    public class Category
    {
        Wiki wiki;
        string name;
        AccessBrowser ab;

        Regex re_subcat = new Regex
            ("<a class=\"CategoryTreeLabel  CategoryTreeLabelNs14 CategoryTreeLabelCategory\" href=\".+?\">(.+?)</a></div>");
        Regex re_catpage = new Regex
            ("<li><a href=\".+?\" title=\"(.+?)\">\\1</a></li>");

        bool loaded;
        string[] subcats;
        string[] pagesincat;

        /// <summary>
        /// Initializes new instance of category class
        /// </summary>
        /// <param name="wiki">Wiki to use</param>
        /// <param name="name">Name of the categry</param>
        public Category(Wiki wiki, string name)
        {
            this.wiki = wiki;
            this.name = name;
            this.ab = wiki.ab;
        }

        /// <summary>
        /// Loads category content
        /// </summary>
        public void Load()
        {
            ab.PageName = "index.php?title=Category:" + ab.EncodeUrl(name);
            MatchCollection subcatmatches = re_subcat.Matches(ab.PageText);
            List<String> subcatslst = new List<string>();
            foreach (Match cmatch in subcatmatches)
            {
                subcatslst.Add(cmatch.Groups[1].Value.Trim());
            }
            subcats = subcatslst.ToArray();
            Regex re_hasmore = new Regex(Regex.Escape(wiki.Messages.GetMessage("nextn").Replace("$1", "~D")).Replace("~D", "\\d+") + "\\)");
            string nextpageuri = "index.php?title=Category:" + ab.EncodeUrl(name);
            List<String> catpageslst = new List<string>();
            do
            {
                ab.PageName = nextpageuri;
                MatchCollection catpagematches = re_catpage.Matches(ab.PageText);
                foreach (Match cmatch in catpagematches)
                {
                    if (!catpageslst.Contains(cmatch.Groups[1].Value.Trim()))
                        catpageslst.Add(cmatch.Groups[1].Value.Trim());
                }
                if (catpageslst.Count < 1) break;
                nextpageuri = "index.php?title=Category:" + ab.EncodeUrl(name) + "&from=" +
                    ab.EncodeUrl(catpageslst[catpageslst.Count - 1]);
            } while (!re_hasmore.Match(ab.PageText).Success);
            pagesincat = catpageslst.ToArray();
            loaded = true;
        }

        /// <summary>
        /// Gets subcategories.  Automatically calls Load() on first usage
        /// </summary>
        public string[] Subcategories
        {
            get
            {
                if (!loaded)
                    Load();
                return subcats;
            }
        }

        /// <summary>
        /// Gets pages in it.  Automatically calls Load() on first usage
        /// </summary>
        public string[] Pages
        {
            get
            {
                if (!loaded)
                    Load();
                return pagesincat;
            }
        }

        /// <summary>
        /// Loads page in this category and all subcategories
        /// </summary>
        /// <returns>Pages</returns>
        public string[] GetPagesRecursive()
        { 
            return GetPagesRecursive(true); 
        }

        /// <summary>
        /// Loads page in this category and all subcategories
        /// </summary>
        /// <param name="removeDuplicates">Remove duplicates from list</param>
        /// <returns>Pages</returns>
        public string[] GetPagesRecursive(bool removeDuplicates)
        {
            if (removeDuplicates)
                return Utils.RemoveDuplicates(GetPagesRecursive(null));
            else
                return GetPagesRecursive(null);
        }

        private string[] GetPagesRecursive(List<string> _passed)
        {
            List<string> passed = (_passed == null ? new List<string>() : _passed);
            if (!passed.Contains(name)) passed.Add(name);
            List<string> result = new List<string>(Pages);
            foreach (string subcat in Subcategories)
            {
                if (passed.Contains(subcat)) continue;
                Category csubcat = new Category(wiki, subcat);
                result.AddRange(csubcat.GetPagesRecursive(passed));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets category page
        /// </summary>
        public Page CategoryPage
        {
            get
            {
                return new Page(wiki, "Category:" + name);
            }
        }

        /// <summary>
        /// Checks if category has its page (if no, it is a wanted category)
        /// </summary>
        public bool HasCategoryPage
        {
            get
            {
                return CategoryPage.Exists;
            }
        }
    }
}
