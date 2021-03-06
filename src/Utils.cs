﻿/**********************************************************************************
 * Utilities of WikiAccess Library                                                *
 * Copyright (C) 2007 Vasiliev V. V.                                              *
 *                                                                                *
 * This program is free software: you can redistribute it and/or modify           *
 * it under the terms of the GNU General Public License as published by           *
 * the Free Software Foundation, either version 3 of the License, or              *
 * (at your option) any later version.                                            *
 *                                                                                *
 * This program is distributed in the hope that it will be useful,                *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of                 *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                  *
 * GNU General Public License for more details.                                   *
 *                                                                                *
 * You should have received a copy of the GNU General Public License              *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>           *
 **********************************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace WikiTools.Access
{
	/// <summary>
	/// Contains useful utils for library
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Returns version of library
		/// </summary>
		public static Version Version
		{
			get { return new Version(AssemblyConfig.Version); }
		}

		/// <summary>
		/// Removes duplicates from list
		/// </summary>
		/// <typeparam name="T">Type of list</typeparam>
		/// <param name="list">List to clean up</param>
		/// <returns>Result list</returns>
		[Obsolete("Please use Enumerable.Distinct() instead")]
        public static List<T> RemoveDuplicates<T>(List<T> list)
		{
		    return list.Distinct().ToList();
		}

		/// <summary>
		/// Removes duplicates from array
		/// </summary>
		/// <typeparam name="T">Type of array</typeparam>
		/// <param name="array">Array to clean up</param>
		/// <returns>Result list</returns>
        [Obsolete("Please use Enumerable.Distinct() instead")]
        public static T[] RemoveDuplicates<T>(T[] array)
		{
            return array.Distinct().ToArray();
		}

		/// <summary>
		/// Removes duplicates from string array
		/// </summary>
		/// <param name="array">Array to clean up</param>
		/// <returns>Result list</returns>
        [Obsolete("Please use Enumerable.Distinct() instead")]
        public static string[] RemoveDuplicates(string[] array)
		{
			return array.Distinct().ToArray();
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
		    return orig.Select(str => prefix + str).ToArray();
		}

		/// <summary>
		/// Swaps values of 2 variables
		/// </summary>
		/// <typeparam name="T">Variables type</typeparam>
		/// <param name="arg1">First variable</param>
		/// <param name="arg2">Second variable</param>
		public static void Swap<T>(ref T arg1, ref T arg2)
		{
			T temp = arg1;
			arg1 = arg2;
			arg2 = temp;
		}

		/// <summary>
		/// Adoptated version of Thread.Sleep method
		/// </summary>
		/// <param name="ts">Time to wait</param>
		public static void Wait(TimeSpan ts)
		{
			DateTime start = DateTime.Now;
			while (DateTime.Now - start < ts) ;
		}

		/// <summary>
		/// Returns the given string with first character uppercase
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string UCFirst(string s)
		{
			if(s.Length > 1)
				return Char.ToUpper(s[0]) + s.Substring(1);
			return string.IsNullOrEmpty(s) ? s : s.ToUpper();
		}

		public static string BinaryToHexString(this byte[] bytes)
		{
			var builder = new StringBuilder();
			foreach (byte b in bytes)
			{
				builder.AppendFormat("{0:X2}", b);
			}
			return builder.ToString();
		}

		public static byte[] ReadAllBytes(Stream stream)
		{
			int cbyte;
			var result = new List<byte>();
			while ((cbyte = stream.ReadByte()) != -1)
			{
				result.Add((byte) cbyte);
			}
			return result.ToArray();
		}

		public static string ReadAllText(Stream stream)
		{
			return new StreamReader(stream, Encoding.UTF8).ReadToEnd();
		}
	}
}