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

namespace Bistro.Designer.Explorer
{
    /// <summary>
    /// class for placeholder reference objects
    /// </summary>
    /// <remarks>
    /// Instances of this class are never visible. They are used as children of the lazy expandable nodes, to force 
    /// the presence of the '+' sign for such objects in the tree. lazy expandable nodes are nodes which only load 
    /// their children when they are expanded
    /// </remarks>
    [Serializable]
    public class ReferenceNode : ExplorerNode
    {
        public ReferenceNode(ExplorerNode parent)
            : base(parent, "reference", ERROR_ICON)
        { }

        public override string Caption
        {
            get
            {
                return "This node is not supposed to be visible";
            }
        }

        protected override ContextMenuStrip GetContextMenu(ContextMenuStrip contextMenu)
        {
            return null;
        }
    }
}
