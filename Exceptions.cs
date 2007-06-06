/**********************************************************************************
 * Exceptions of WikiAcces Library                                                *
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

namespace WikiTools.Access
{
	/// <summary>
	/// Generic wiki exception
	/// </summary>
    public class WikiException : Exception
    {
    }

    /// <summary>
    /// Is thrown when MediaWiki doesn't support some features
    /// </summary>
    public class WikiNotSupportedException : WikiException
    {
    }

	/// <summary>
	/// Is thrown when you haven't got permissions to do some actions
	/// </summary>
    public class WikiPermissionsExpection : WikiException
    {
    }

	/// <summary>
	/// Is trown when page doesn't exists
	/// </summary>
    public class WikiPageNotFoundExcecption : WikiException
    {
    }
}