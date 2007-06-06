﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WikiTools.Access
{
    #region Capabilities

	/// <summary>
	/// Capabilities of wiki
	/// </summary>
    public struct WikiCapabilities
    {
        /// <summary>
        /// Version of MediaWiki
        /// </summary>
        public Version Version;
        /// <summary>
        /// Requierd to check users
        /// </summary>
        public bool HasCheckUser;
        /// <summary>
        /// Required for full template substitution
        /// </summary>
        public bool HasExpandTemplates;
        /// <summary>
        /// Required to get images
        /// </summary>
        public bool HasFilePath;
        /// <summary>
        /// Requiered to make bot via MakeBot interface and have access to MakeBot log
        /// </summary>
        public bool HasMakeBot;
        /// <summary>
        /// Required to determine what permission bureaucrat have
        /// </summary>
        public bool HasMakeSysop;
        /// <summary>
        /// Required to get new users log
        /// </summary>
        public bool HasNewUserLog;
        /// <summary>
        /// Required to hide revisions
        /// </summary>
        public bool HasOversight;
        /// <summary>
        /// Required to rename users
        /// </summary>
        public bool HasRenameUser;

		/// <summary>
		/// Casts capabilities to string
		/// </summary>
		/// <returns>String</returns>
        public override string ToString()
        {
            List<string> str = new List<string>();
            str.Add("!Wiki-capa");
            str.Add("version = " + Version.ToString());
            str.Add("ext:checkuser = " + HasCheckUser);
            str.Add("ext:exptl = " + HasExpandTemplates);
            str.Add("ext:fpath = " + HasFilePath);
            str.Add("ext:mkbot = " + HasMakeBot);
            str.Add("ext:mksysop = " + HasMakeSysop);
            str.Add("ext:newusers = " + HasNewUserLog);
            str.Add("ext:oversight = " + HasOversight);
            str.Add("ext:renameuser = " + HasRenameUser);
            return String.Join("\n", str.ToArray());
        }

		/// <summary>
		/// Loads capabilities from string
		/// </summary>
		/// <param name="s">String to parse</param>
		/// <returns>Succes of parsing</returns>
        public bool FromString(string s)
        {
            StringReader sr = new StringReader(s);
            if (sr.ReadLine().Trim() != "!Wiki-capa")
                return false;
            string cstr;
            while (!String.IsNullOrEmpty(cstr = sr.ReadLine()))
            {
                string[] kv = cstr.Split(new char[] { '=' }, 2);
                kv[0] = kv[0].Trim(); kv[1] = kv[1].Trim();
                switch (kv[0])
                {
                    case "version":
                        Version = new Version(kv[1]);
                        break;
                    case "ext:checkuser":
                        HasCheckUser = Boolean.Parse(kv[1]);
                        break;
                    case "ext:exptl":
                        HasExpandTemplates = Boolean.Parse(kv[1]);
                        break;
                    case "ext:fpath":
                        HasFilePath = Boolean.Parse(kv[1]);
                        break;
                    case "ext:mkbot":
                        HasMakeBot = Boolean.Parse(kv[1]);
                        break;
                    case "ext:mksysop":
                        HasMakeSysop = Boolean.Parse(kv[1]);
                        break;
                    case "ext:newusers":
                        HasNewUserLog = Boolean.Parse(kv[1]);
                        break;
                    case "ext:oversight":
                        HasOversight = Boolean.Parse(kv[1]);
                        break;
                    case "ext:renameuser":
                        HasRenameUser = Boolean.Parse(kv[1]);
                        break;
                }
            }
            return true;
        }
    }

    #endregion

	/// <summary>
	/// Statistics of wiki
	/// </summary>
    public struct Statistics
    {
		/// <summary>
		/// Count of total pages
		/// </summary>
        public int TotalPages;
		/// <summary>
		/// Count of articles
		/// </summary>
        public int GoodPages;
		/// <summary>
		/// Count of views
		/// </summary>
        public int Views;
		/// <summary>
		/// Count of revisions
		/// </summary>
        public int Edits;
		/// <summary>
		/// Count of users
		/// </summary>
        public int Users;
		/// <summary>
		/// Count of sysops
		/// </summary>
        public int Admins;
		/// <summary>
		/// Count of images
		/// </summary>
        public int Images;
		/// <summary>
		/// Size of job queue
		/// </summary>
        public int Jobs;
    }

    public enum ProtectionLevel
    {
        None,
        Autoconfirmed,
        Sysop,
    }

    public enum LogAction
    {
        Block,
        Unblock,
        Protect,
        Unprotect,
        Rights,
        Delete,
        Restore,
        DeleteRevision,
        Upload,
        Move,
        Import,
        RenameUser,
        MakeBot,
        RevokeBot
    }

    public enum LogType
    {
        Block = 1,
        Protect = 2,
        Rights = 4,
        Delete = 8,
        Upload = 16,
        Move = 32,
        Import = 64,
        Renameuser = 128,
        Newusers = 256,
        Makebot = 512
    }
}
