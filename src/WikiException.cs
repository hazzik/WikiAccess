﻿/**********************************************************************************
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
		public WikiException()
		{
		}

		/// <summary>
		/// Initializes new instance of WikiException object
		/// </summary>
		/// <param name="message">Message of exception</param>
		public WikiException(string message) : base(message)
		{
		}
	}
}