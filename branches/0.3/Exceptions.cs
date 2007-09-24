/**********************************************************************************
 * Wxceptions of WikiAccess Library                                               *
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
using System.Text;

namespace WikiTools.Access
{
	/// <summary>
	/// Generic wiki exception
	/// </summary>
	public class WikiException : Exception
	{
		/// <summary>
		/// Initializes new instance of WikiException object
		/// </summary>
		public WikiException() : base() {}
		/// <summary>
		/// Initializes new instance of WikiException object
		/// </summary>
		/// <param name="message">Message of exception</param>
		public WikiException(string message) : base(message) {}
	}

	/// <summary>
	/// Is thrown when MediaWiki doesn't support some features
	/// </summary>
	public class WikiNotSupportedException : WikiException
	{
		/// <summary>
		/// Initializes new instance of WikiNotSupportedException object
		/// </summary>
		public WikiNotSupportedException() : base() {}
		/// <summary>
		/// Initializes new instance of WikiNotSupportedException object
		/// </summary>
		/// <param name="message">Message of exception</param>
		public WikiNotSupportedException(string message) : base(message) { }
	}

	/// <summary>
	/// Is thrown when you haven't got permissions to do some actions
	/// </summary>
	public class WikiPermissionsExpection : WikiException
	{
		/// <summary>
		/// Initializes new instance of WikiPermissionsExpection object
		/// </summary>
		public WikiPermissionsExpection() : base() {}
		/// <summary>
		/// Initializes new instance of WikiPermissionsExpection object
		/// </summary>
		/// <param name="message">Message of exception</param>
		public WikiPermissionsExpection(string message) : base(message) {}
	}

	/// <summary>
	/// Is trown when page doesn't exists
	/// </summary>
	public class WikiPageNotFoundExcecption : WikiException
	{
		/// <summary>
		/// Initializes new instance of WikiPageNotFoundExcecption object
		/// </summary>
		public WikiPageNotFoundExcecption() : base() {}
		/// <summary>
		/// Initializes new instance of WikiPageNotFoundExcecption object
		/// </summary>
		/// <param name="message">Message of exception</param>
		public WikiPageNotFoundExcecption(string message) : base(message) { }
	}
}