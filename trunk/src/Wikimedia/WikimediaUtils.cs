using System;

namespace WikiTools.Access.Wikimedia
{
    /// <summary>
    /// Provides utils for wikimedia projects
    /// </summary>
    public static class WikimediaUtils
    {
        /// <summary>
        /// Gets Wikimedia wikis capabilities
        /// </summary>
        public static WikiCapabilities Capabilities
        {
            get
            {
                var result = new WikiCapabilities();
                result.HasCheckUser =
                    result.HasExpandTemplates =
                    result.HasFilePath =
                    result.HasMakeBot =
                    result.HasMakeSysop =
                    result.HasNewUserLog =
                    result.HasOversight =
                    result.HasRenameUser =
                    true;
                result.Version = new Version(1, 11);
                return result;
            }
        }

        /// <summary>
        /// Makes URI for specified Wikimedia project
        /// </summary>
        /// <param name="proj">Project</param>
        /// <returns>Project URI</returns>
        public static string MakeUri(WikimediaProjects proj)
        {
            return MakeUri(proj, "");
        }

        /// <summary>
        /// Makes URI for specified Wikimedia project
        /// </summary>
        /// <param name="proj">Project</param>
        /// <param name="langCode">Language code</param>
        /// <returns>Project URI</returns>
        public static string MakeUri(WikimediaProjects proj, string langCode)
        {
            if (IsMultilingualProject(proj))
            {
                return "http://" + langCode + "." + proj.ToString().ToLower() + ".org/w";
            }
            switch (proj)
            {
                case WikimediaProjects.Test:
                    return "http://test.wikipedia.org/w";
                case WikimediaProjects.Foundation:
                    return "http://wikimediafoundation.org/w";
                default:
                    return "http://" + proj.ToString().ToLower() + ".wikimedia.org/w";
            }
        }

        private static bool IsMultilingualProject(WikimediaProjects proj)
        {
            var mlingprojs = new[]
                                 {
                                     WikimediaProjects.Wikipedia,
                                     WikimediaProjects.Wiktionary, WikimediaProjects.Wikibooks, WikimediaProjects.Wikiversity,
                                     WikimediaProjects.Wikinews, WikimediaProjects.Wikiquote, WikimediaProjects.Wikisource
                                 };
            return Array.IndexOf(mlingprojs, proj) != -1;
        }
    }
}