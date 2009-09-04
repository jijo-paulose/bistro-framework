/****************************************************************************
 * 
 *   Workflow Server Copyright © 2003-2006 Hill30 Inc
 *
 *   Workflow Server is free software; you can redistribute it and/or
 *   modify it under the terms of the GNU Lesser General Public
 *   License as published by the Free Software Foundation; either
 *   version 2.1 of the License.
 *
 *   This software is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *   Lesser General Public License for more details.
 *
 *   For a full text of the GNU Lesser General Public please see the License.txt
 *   file or write to the Free Software Foundation, Inc., 51 Franklin Street, 
 *   Fifth Floor, Boston, MA  02110-1301  USA
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Bistro.Designer.Explorer;
using Bistro.Designer;

namespace Bistro.Designer.Explorer
{
    public class ApplicationNode : ExplorerNode
    {

        #region Constructors

        public ApplicationNode(IBsApplicationDesigner application)
            : base(application, application.Name, APPLICATION_ICON)
        {
        }

        #endregion

    }

}
