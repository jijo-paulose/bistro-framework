using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.Explorer;
using TestDate;

namespace Controls.Nodes
{
    public class ApplicationNode : ExplorerNode
    {

        #region Constructors

        public ApplicationNode(TestDescriptor td)
            : base((ExplorerNode)null, td.Name, APPLICATION_ICON)
        {
        }

        #endregion

    }
}
