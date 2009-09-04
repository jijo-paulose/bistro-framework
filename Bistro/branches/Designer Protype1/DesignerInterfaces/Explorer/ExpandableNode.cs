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
using Bistro.Designer.Explorer;

namespace Bistro.Designer.Explorer
{
    [Serializable]
    public class ExpandableNode : ExplorerNode
    {
        public ExpandableNode(ExplorerNode parent, string name, string imageKey)
            : base(parent, name, imageKey)
        {
        }

        public ExpandableNode(ExplorerNode parent, string name, string imageKey, string expandedImageKey)
            : base (parent, name, imageKey, expandedImageKey)
        {
        }

        public ExpandableNode(ExplorerNode parent, ExplorerNode source, string imageKey, string expandedImageKey)
            : base(parent, source, imageKey, expandedImageKey)
        {
        }

        protected override bool Expandable() { return true; }
    }
}
