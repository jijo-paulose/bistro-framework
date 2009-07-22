using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Bistro.Designer.Core.Projects
{
    public class Project
    {
        public Project(IVsHierarchy hierarchy)
        {
            this.hierarchy = hierarchy;
            object nameObject;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_Name, out nameObject));
            name = (string)nameObject;

            object startupService;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_StartupServices, out startupService));
            IEnumProjectStartupServices services;
            ErrorHandler.ThrowOnFailure(((IVsProjectStartupServices)startupService).GetStartupServiceEnum(out services));
            Guid[] guids = new Guid[1];
            uint fetched;
            IsConverted = false;
            while (true)
            {
                services.Next(1, guids, out fetched);
                if (fetched == 0)
                    break;
                if (guids[0] == new Guid(Guids.guidProjectManager))
                {
                    IsConverted = true;
                    break;
                }
            }
            object iconImgListObject;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_IconImgList, out iconImgListObject));
            object iconIndexObject;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_IconIndex, out iconIndexObject));
            IntPtr iconPtr = ImageList_GetIcon(new IntPtr((int)iconImgListObject), (int)iconIndexObject, 0);
            icon = Icon.FromHandle(iconPtr);
        }

        [DllImport("comctl32.dll")]
        public static extern int ImageList_GetImageCount(IntPtr handle);

        [DllImport("comctl32.dll")]
        public static extern IntPtr ImageList_GetIcon(IntPtr handle, int idx, UInt32 flags);

        string name;
        IVsHierarchy hierarchy;
        Icon icon;
        public string Name { get { return name; } }

        public Icon Icon { get { return icon; } }

        internal void Convert()
        {
            if (IsConverted)
                return;
            object startupService;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_StartupServices, out startupService));
            Guid guid = new Guid(Guids.guidProjectManager);
            ErrorHandler.ThrowOnFailure(((IVsProjectStartupServices)startupService).AddStartupService(ref guid));
        }

        internal bool IsConverted { get; private set; }
    }
}
