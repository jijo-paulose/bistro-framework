using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace Bistro.Designer.Explorer
{
    internal class ProjectFileManager : FileManager, IVsTrackProjectDocumentsEvents2
    {

        private IVsTrackProjectDocuments2 vsTPD;
        private uint tpdCookie;

        internal ProjectFileManager(IVsProject project,string codeFileExt,object UIControl)
            : base(project, codeFileExt,UIControl)
        {
            AdviseEvents();
        }

        internal override void AdviseEvents()
        {
            // 1. subcribe of inside VS events
            //vsRDT = (IVsRunningDocumentTable)proxy.GetService(typeof(SVsRunningDocumentTable));
            //ErrorHandler.ThrowOnFailure(vsRDT.AdviseRunningDocTableEvents(this, out rdtCookie));

            vsTPD = (IVsTrackProjectDocuments2)Package.GetGlobalService(typeof(SVsTrackProjectDocuments));
            ErrorHandler.ThrowOnFailure(vsTPD.AdviseTrackProjectDocumentsEvents(this, out tpdCookie));

            base.AdviseEvents();

        }

        #region IVsTrackProjectDocumentsEvents2 Members (handler of adding/removing documents inside VS)

        public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            for (int i = 0; i < rgpProjects.Length; i++)
                if (rgpProjects[i] == Project)
                {
                    int upBound = (rgFirstIndices.Length > i + 1) ? rgFirstIndices[i + 1] : rgpszMkDocuments.Length;
                    for (int j = rgFirstIndices[i]; j < upBound; j++)
                        FileAdded(rgpszMkDocuments[j]);
                }

            return VSConstants.S_OK;
        }

        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            for (int i = 0; i < rgpProjects.Length; i++)
                if (rgpProjects[i] == Project)
                {
                    int upBound = (rgFirstIndices.Length > i + 1) ? rgFirstIndices[i + 1] : rgpszMkDocuments.Length;
                    for (int j = rgFirstIndices[i]; j < upBound; j++)
                        FileRemoved(rgpszMkDocuments[j]);
                }

            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            OnAfterRemoveFiles(cProjects, cFiles, rgpProjects, rgFirstIndices, rgszMkOldNames, null);
            OnAfterAddFilesEx(cProjects, cFiles, rgpProjects, rgFirstIndices, rgszMkNewNames, null);
            return VSConstants.S_OK;
        }

        public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        #endregion

        protected override List<string> BuildCodeFileList()
        {
            return BuildCodeFileList(VSConstants.VSITEMID_ROOT);
        }

        private List<string> BuildCodeFileList(uint item)
        {
            List<string> result = new List<string>();

            object obj;
            if (ErrorHandler.Failed(((IVsHierarchy)Project).GetProperty(item, (int)__VSHPROPID.VSHPROPID_FirstChild, out obj)))
                return result;

            item = (uint)(int)obj;
            while (item != VSConstants.VSITEMID_NIL)
            {
                Guid typeGuid;
                string fileName;
                if (
                    // we succesfully retrieved the property
                   ErrorHandler.Succeeded(((IVsHierarchy)Project).GetGuidProperty(item, (int)__VSHPROPID.VSHPROPID_TypeGuid, out typeGuid))
                    // this is a real file
                   && VSConstants.GUID_ItemType_PhysicalFile == typeGuid
                    // will be compiled when the project is built
                   && isCompilable(item)
                    // we succesfully retrieved the filename
                   && ErrorHandler.Succeeded(Project.GetMkDocument(item, out fileName)))
                {
                    result.Add(fileName);
                }

                result.AddRange(BuildCodeFileList(item));

                ((IVsHierarchy)Project).GetProperty(item, (int)__VSHPROPID.VSHPROPID_NextSibling, out obj);
                item = (uint)(int)obj;
            }

            return result;
        }

        protected override bool ToBeParsed(string path)
        {
            uint itemid;

            if (ErrorHandler.Failed(((IVsHierarchy)Project).ParseCanonicalName(path, out itemid)))
                return false;

            return isCompilable(itemid);
        }

        private bool isCompilable(uint itemid)
        {
            object browseObject;
            if (ErrorHandler.Succeeded(((IVsHierarchy)Project).GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_BrowseObject, out browseObject)))
            {
                object action = browseObject.GetType().InvokeMember("BuildAction", System.Reflection.BindingFlags.GetProperty, null, browseObject, new object[] { });
                return (int)action == 1;
            }
            return false;
        }

        protected override void DisposeManaged()
        {

            if (vsTPD != null)
                    vsTPD.UnadviseTrackProjectDocumentsEvents(tpdCookie);
            vsTPD = null;

            base.DisposeManaged();
        }

    }
}
