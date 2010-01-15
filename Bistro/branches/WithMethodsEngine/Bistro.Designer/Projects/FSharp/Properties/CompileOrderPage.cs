using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;

namespace Bistro.Designer.Projects.FSharp.Properties
{
    [CLSCompliant(false), ComVisible(true), Guid("D43926CD-8001-42fd-999E-F5B4BA050107")]
    public class CompileOrderPage : PropertyTabContainer
    {
        public CompileOrderPage()
        {
            Name = "Compilation Order";
        }

        protected override Control CreateControl()
        {
            return new CompileOrderViewer((ProjectManager)this.ProjectMgr);
        }

        protected override void BindProperties()
        {
            //throw new NotImplementedException();
        }

        protected override int ApplyChanges()
        {
            //throw new NotImplementedException();
            return VSConstants.S_OK;
        }
    }
}
