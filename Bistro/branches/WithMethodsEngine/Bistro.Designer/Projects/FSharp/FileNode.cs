using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.ProjectBase;

namespace Bistro.Designer.Projects.FSharp
{
    class FSharpFileNode : FileNode
    {
        internal FSharpFileNode(ProjectNode root, ProjectElement e)
			: base(root, e)
		{
            // TODO: It looks like the selectionChanged is introduced in py to support codebehind code generation
            //selectionChangedListener = new SelectionElementValueChangedListener(new ServiceProvider((IOleServiceProvider)root.GetService(typeof(IOleServiceProvider))), root);
            //selectionChangedListener.Init();
		}


    }
}
