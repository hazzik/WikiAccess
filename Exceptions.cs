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
		public WikiException() : base() {}
		public WikiException(string message) : base(message) {}
	}

	/// <summary>
	/// Is thrown when MediaWiki doesn't support some features
	/// </summary>
	public class WikiNotSupportedException : WikiException
	{
		public WikiNotSupportedException() : base() {}
		public WikiNotSupportedException(string message) : base(message) { }
	}

	/// <summary>
	/// Is thrown when you haven't got permissions to do some actions
	/// </summary>
	public class WikiPermissionsExpection : WikiException
	{
		public WikiPermissionsExpection() : base() {}
		public WikiPermissionsExpection(string message) : base(message) {}
	}

	/// <summary>
	/// Is trown when page doesn't exists
	/// </summary>
	public class WikiPageNotFoundExcecption : WikiException
	{
		public WikiPageNotFoundExcecption() : base() {}
		public WikiPageNotFoundExcecption(string message) : base(message) { }
	}
}