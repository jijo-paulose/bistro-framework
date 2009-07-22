using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;

namespace Bistro.Designer.Core
{
    internal interface ISolutionEvents
    {
        event EventHandler OnAfterCloseSolution;
        event EventHandler OnAfterOpenSolution;
    }

    class SolutionEventSinks : ISolutionEvents, IVsSolutionEvents, IDisposable
    {
        uint solutionEventsCookie;
        IVsSolution solution;
        public SolutionEventSinks()
        {
            solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            if (null == solution)
                Trace.WriteLine("Can't access solution service");
            else
                solution.AdviseSolutionEvents((IVsSolutionEvents)this, out solutionEventsCookie);
        }

        protected virtual void Dispose(bool disposing)
        {
            // if we are here because of the finalizer, we cannot unadvise anyway
            if (disposing)
            {
                if (solution != null && solutionEventsCookie != VSConstants.VSCOOKIE_NIL)
                    solution.UnadviseSolutionEvents(solutionEventsCookie);
            }
        }

        // No need for this - we cannot do anything useful anyways
        //~SolutionEventSinks()
        //{
        //    Dispose(false);
        //}

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region IVsSolutionEvents Members

        int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved)
        {
            if (OnAfterCloseSolution != null)
                OnAfterCloseSolution(this, EventArgs.Empty);
            return VSConstants.S_OK;
        }
        public event EventHandler OnAfterCloseSolution;

        int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            if (OnAfterOpenSolution != null)
                OnAfterOpenSolution(this, EventArgs.Empty);
            return VSConstants.S_OK;
        }
        public event EventHandler OnAfterOpenSolution;

        int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
