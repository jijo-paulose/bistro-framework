using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
namespace Bistro.Designer.Explorer
{
    public class SelectionEventsHandler : IVsSelectionEvents, IVsSolutionEvents
    {
        public SelectionEventsHandler()
        {
            //if (null == serviceProvider) throw new System.ArgumentNullException("host");
            Guid solutionExists = VSConstants.UICONTEXT_SolutionExists;
            monitorSelectionService = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            monitorSelectionService.GetCmdUIContextCookie(ref solutionExists, out SolutionExistsCookie);
        }
        IVsMonitorSelection monitorSelectionService;
        IVsSolution solution; 
        public readonly ServiceProvider _ServiceProvider;
        public readonly uint SolutionExistsCookie;
        protected uint SolutionSubscriptionCookie = 0;
        int IVsSelectionEvents.OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            int active = 0;
            if (monitorSelectionService != null) monitorSelectionService.IsCmdUIContextActive(SolutionExistsCookie, out active);
            Trace.WriteLine("Solution Exists: " + active);

            if (active != 0 && SolutionSubscriptionCookie == 0)
            {
                // The solution is loaded. Now we can get hold of our selection events. 
                solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
                Debug.Assert(solution != null);
                solution.AdviseSolutionEvents(this, out SolutionSubscriptionCookie);
            }

            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        #region IVsSolutionEvents Members

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            throw new NotImplementedException();
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            throw new NotImplementedException();
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
           /* Guid projectGuid;
            ErrorHandler.ThrowOnFailure(
                pHierarchy.GetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out projectGuid)
            );*/
            uint pdwCookie;
            pHierarchy.AdviseHierarchyEvents(new MetadataExtractor("c#", String.Empty), out pdwCookie);
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            throw new NotImplementedException();
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            throw new NotImplementedException();
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            throw new NotImplementedException();
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            throw new NotImplementedException();
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            throw new NotImplementedException();
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            throw new NotImplementedException();
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVsSelectionEvents Members


        int IVsSelectionEvents.OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            throw new NotImplementedException();
        }

        int IVsSelectionEvents.OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
