using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Bistro.Designer.Projects.FSharp
{
    [CLSCompliant(false), ComVisible(true), Guid("D43926CD-8001-42fd-999E-F5B4BA050107")]
    public class BuildOrderPage : PropertyTabContainer
    {
        public BuildOrderPage()
        {
            Name = "Compilation Order";
        }

        protected override Type TabType
        {
            get { return typeof(BuildOrderViewer); }
        }
    }
}
