﻿/**********************************************************************************
 * Utils of WikiAcces Library                                                     *
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
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace WikiTools.Access
{
    /// <summary>
    /// Contains useful utils for library
    /// </summary>
    public partial class Utils
    {
        /// <summary>
        /// Returns version of library
        /// </summary>
        public static Version Version
        {
            get
            {
				return new Version(0, 1);
            }
        }

        /// <summary>
        /// Checks if node contains specified attribute
        /// </summary>
        /// <param name="elem">Element</param>
        /// <param name="attrname">Attribute name</param>
        /// <returns>Existance of attributes</returns>
        public static bool ContainsAttribure(XmlElement elem, string attrname)
        {
            foreach (XmlAttribute cattr in elem.Attributes)
            {
                if (cattr.Name.ToLower() == attrname.ToLower())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Removes duplicates from list
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="list">List to clean up</param>
        /// <returns>Result list</returns>
        public static List<T> RemoveDuplicates<T>(List<T> list)
        {
            List<T> lst_unique = new List<T>(list);
            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];
                if (!lst_unique.Contains(t))
                    lst_unique.Add(t);
            }
            return lst_unique;
        }

        /// <summary>
        /// Removes duplicates from array
        /// </summary>
        /// <typeparam name="T">Type of array</typeparam>
        /// <param name="list">Array to clean up</param>
        /// <returns>Result list</returns>
        public static T[] RemoveDuplicates<T>(T[] list)
        {
            List<T> lst = new List<T>(list);
            lst = RemoveDuplicates<T>(lst);
            return lst.ToArray();
        }

        /// <summary>
        /// Removes duplicates from string array
        /// </summary>
        /// <param name="array">Array to clean up</param>
        /// <returns>Result list</returns>
        public static string[] RemoveDuplicates(string[] array)
        {
            return RemoveDuplicates<string>(array);
        }
        
        /// <summary>
        /// Formats DateTime in API format
        /// </summary>
        /// <param name="dt">DateTime in format</param>
        /// <returns>DateTime in API format</returns>
        public static string FormatDateTimeRFC2822(DateTime dt)
        {
            return DateTime.Now.ToString(@"ddd, dd MMM yyyy HH:mm:ss G\MT", DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Checks if array contains specified element
        /// </summary>
        /// <typeparam name="T">Type of array</typeparam>
        /// <param name="array">Array to check</param>
        /// <param name="value">Value to check</param>
        /// <returns>Existance of specified element in array</returns>
        public static bool ArrayContains<T>(T[] array, T value)
        {
            return Array.IndexOf(array, value) >= 0;
        }

        /// <summary>
        /// Adds prefix to all elements in array
        /// </summary>
        /// <param name="orig">Original array</param>
        /// <param name="prefix">Prefix to add</param>
        /// <returns>New array</returns>
        public static string[] AddPrefix(string[] orig, string prefix)
        {
            List<string> result = new List<string>();
            foreach (string str in orig)
                result.Add(prefix + str);
            return result.ToArray();
        }

        /// <summary>
        /// Process events in application to avoid context deadlock
        /// </summary>
        public static void DoEvents()
        {
            System.Windows.Forms.Application.DoEvents();
        }

        public static void Swap<T>(ref T arg1, ref T arg2)
        {
            T temp = arg1;
            arg1 = arg2;
            arg2 = temp;
        }
    }
}
