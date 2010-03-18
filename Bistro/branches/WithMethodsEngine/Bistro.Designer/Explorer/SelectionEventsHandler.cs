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
    public class SelectionEventsHandler : IVsSelectionEvents, IVsSolutionEvents,IVsSolutionEvents4
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
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
           /* Guid projectGuid;
            ErrorHandler.ThrowOnFailure(
                pHierarchy.GetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out projectGuid)
            );*/
            uint pdwCookie;
            string projectName;
            pHierarchy.GetCanonicalName(VSConstants.VSITEMID_ROOT, out projectName);
            string lang = (projectName.EndsWith(".csproj")) ? "c#" : "f#"; 
            pHierarchy.AdviseHierarchyEvents(new ChangesTracker(lang), out pdwCookie);

            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            //previously created solution has been loaded,so call onafterOpenProject for each project 
            if (fNewSolution == 0)
            {
                IEnumHierarchies ppEnum;
                Guid tempGuid = Guid.Empty;
                solution.GetProjectEnum((uint)Microsoft.VisualStudio.Shell.Interop.__VSENUMPROJFLAGS.EPF_ALLPROJECTS, ref tempGuid, out ppEnum);
                if (ppEnum != null)
                {
                    uint actualResult = 0;
                    IVsHierarchy[] nodes = new IVsHierarchy[1];
                    while (0 == ppEnum.Next(1, nodes, out actualResult))
                    {
                        OnAfterOpenProject(nodes[0], 0);
                        //Object obj;
                        //nodes[0].GetProperty((uint)Microsoft.VisualStudio.VSConstants.VSITEMID_ROOT, (int)Microsoft.VisualStudio.Shell.Interop.__VSHPROPID.VSHPROPID_ExtObject, out obj);
                    }
                }

            }
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            //pHierarchy.UnadviseHierarchyEvents();
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
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

        #region IVsSolutionEvents4 Members

        int IVsSolutionEvents4.OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            throw new NotImplementedException();
        }

        int IVsSolutionEvents4.OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
        {
            throw new NotImplementedException();
        }

        int IVsSolutionEvents4.OnAfterRenameProject(IVsHierarchy pHierarchy)
        {
            throw new NotImplementedException();
        }

        int IVsSolutionEvents4.OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
