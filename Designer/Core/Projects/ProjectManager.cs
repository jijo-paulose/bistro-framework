using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;

namespace Bistro.Designer.Core.Projects
{
    [Guid(Guids.guidProjectManager)]
    public interface SProjectManager
    {
    }

    public interface IProjectManager
    {
        IEnumerable<Project> Projects { get; }
        event ProjectDelegate ProjectAdded;
        event ProjectDelegate ProjectRemoved;
    }

    public delegate void ProjectDelegate(Project project);

    internal class ProjectManager : SProjectManager, IProjectManager
    {

        IVsSolution solution;
        IServiceProvider serviceProvider;

        public ProjectManager(IServiceProvider serviceProvider, ISolutionEvents solutionEvents)
        {
            this.serviceProvider = serviceProvider;
            solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            BuildProjectList();
            solutionEvents.OnAfterOpenSolution += new EventHandler(solutionEvents_OnAfterOpenSolution);
            solutionEvents.OnAfterCloseSolution += new EventHandler(solutionEvents_OnAfterCloseSolution);

            OleMenuCommandService commandService = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            // Create the command for the convert to Bistro command
            commandService.AddCommand(
                new MenuCommand(
                    convertCommand_Click,
                    new CommandID(Guids.guidCoreCmdSet, (int)PkgCmdIDList.cmdidConvertProjects)
                    )
                );
        }

        void solutionEvents_OnAfterOpenSolution(object sender, EventArgs e)
        {
            BuildProjectList();
        }

        void solutionEvents_OnAfterCloseSolution(object sender, EventArgs e)
        {
            if (ProjectRemoved != null)
                foreach (Project project in projects.Values)
                    ProjectRemoved(project);
            projects.Clear();
        }

        Dictionary<IVsHierarchy, Project> projects = new Dictionary<IVsHierarchy,Project>();

        void convertCommand_Click(object sender, EventArgs e)
        {
            IVsMonitorSelection selectionMonitor = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
            IntPtr ppHier;
            uint pitemid;
            IVsMultiItemSelect ppMIS;
            IntPtr ppSC;
            ErrorHandler.ThrowOnFailure(selectionMonitor.GetCurrentSelection(out ppHier, out pitemid, out ppMIS, out ppSC));
            IVsHierarchy hier = (IVsHierarchy)Marshal.GetObjectForIUnknown(ppHier);
            Marshal.Release(ppHier);
            projects[hier].Convert();
        }

        private void BuildProjectList()
        {
            Guid dummy = Guid.Empty;    // method signature requires it, but in our case it is ignored
            IEnumHierarchies projects;
            solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref dummy, out projects);
            IVsHierarchy[] projectArray = new IVsHierarchy[1];
            uint fetched;
            while (true)
            {
                projects.Next(1, projectArray, out fetched);
                if (fetched == 0)
                    break;
                Project project = new Project(projectArray[0]);
                this.projects.Add(projectArray[0], project);
                if (ProjectAdded != null)
                    ProjectAdded(project);
            }
        }


        #region IProjectManager Members

        public event ProjectDelegate ProjectAdded;

        public event ProjectDelegate ProjectRemoved;

        public IEnumerable<Project> Projects
        {
            get { return projects.Values; }
        }

        #endregion
    }
}
