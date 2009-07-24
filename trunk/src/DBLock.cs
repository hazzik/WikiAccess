/**********************************************************************************
 * DB locking functions of WikiAcces Library                                      *
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
using System.Net;
using System.Text;

namespace WikiTools.Access
{
    partial class Wiki
    {
        /*/// <summary>
        /// Locks DB using Special:Lockdb
        /// </summary>
        /// <param name="reason">Reason, that will be shown when user tries to modify something</param>
        public void LockDB(string reason)
        {
            ab.PageName = "index.php?title=Special:Lockdb";
            ab.SetTextboxField("wpLockReason", reason);
            ab.SetCheckbox("wpLockConfirm", true);
            ab.ClickButton("wpLock");
        }

        /// <summary>
        /// Unlocks DB using Special:Unlockdb
        /// </summary>
        public void UnlockDB()
        {
            ab.PageName = "index.php?title=Special:Unlockdb";
            ab.SetCheckbox("wpLockConfirm", true);
            ab.ClickButton("wpLock");
        }*/
    }
}
