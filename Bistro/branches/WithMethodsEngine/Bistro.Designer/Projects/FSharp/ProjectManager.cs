using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.ProjectBase;
using System.Drawing;
using System.Windows.Forms;
using Bistro.Designer.ProjectBase.Automation;
using System.IO;
using Microsoft.VisualStudio.Editors.PropertyPages;
using Bistro.Designer.Projects.FSharp.Properties;

namespace Bistro.Designer.Projects.FSharp
{
    public class ProjectManager : ProjectNode
    {
        private static ImageList imageList;
        private OAVSProject vsProject;

        static ProjectManager()
        {
            imageList = Utilities.GetImageList(
                typeof(ProjectNode).Assembly.GetManifestResourceStream("Bistro.Designer.Resources.Root.bmp"));
        }

        int imageIndex;
        public ProjectManager(DesignerPackage package)
        {
            Package = package;

            // I would rather override the property to always return true
            this.SupportsProjectDesigner = true;
            imageIndex = this.ImageHandler.ImageList.Images.Count;
            foreach (Image img in imageList.Images)
            {
                this.ImageHandler.AddImage(img);
            }

            InitializeCATIDs();
        }

        /// <summary>
        /// Returns the project type guid AKA the guid of the project factory type
        /// </summary>
        public override Guid ProjectGuid
        {
            get { return typeof(Factory).GUID; }
        }

        /// <summary>
        /// Evaluates if a file is an FSharp code file based on is extension
        /// </summary>
        /// <param name="strFileName">The filename to be evaluated</param>
        /// <returns>true if is a code file</returns>
        public override string ProjectType
        {
            get { return "Bistro"; }
        }

        /// <summary>
        /// Evaluates file name to determine whether this is a code file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override bool IsCodeFile(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                return false;
            }
            return (String.Compare(Path.GetExtension(fileName), ".fs", StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Overriden here to provide notification to the projecy property page
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected override ProjectElement AddFileToMsBuild(string file)
        {
            // TODO: rename; delete
            ProjectElement result = base.AddFileToMsBuild(file);
            RaiseProjectModified();
            return result;
        }

        public event EventHandler OnProjectModified;

        public void RaiseProjectModified()
        {
            if (OnProjectModified != null)
                OnProjectModified(this, EventArgs.Empty);
        }

        protected override NodeProperties CreatePropertiesObject()
        {
            return new OAProjectProperties(this);
        }

        protected override Guid[] GetConfigurationIndependentPropertyPages()
        {
            // Application, BuildEvents, ReferencePaths
            return new Guid[]
            {
                typeof(ApplicationPage).GUID,
                typeof(ApplicationPropPageComClass).GUID,
                typeof(CompileOrderPage).GUID,
                typeof(BuildEventsPropPageComClass).GUID,
                typeof(ReferencePathsPropPageComClass).GUID
            };
        }

        protected override Guid[] GetConfigurationDependentPropertyPages()
        {
            // Build Project property page
            return new Guid[] { typeof(BuildPropPageComClass).GUID };
        }

        /// <summary>
        /// Provide mapping from our browse objects and automation objects to our CATIDs
        /// </summary>
        private void InitializeCATIDs()
        {
            // TODO: figure out CATIDs - they have to do with property browsing
            //// The following properties classes are specific to python so we can use their GUIDs directly
            //this.AddCATIDMapping(typeof(PythonProjectNodeProperties), typeof(PythonProjectNodeProperties).GUID);
            //this.AddCATIDMapping(typeof(PythonFileNodeProperties), typeof(PythonFileNodeProperties).GUID);
            //this.AddCATIDMapping(typeof(OAIronPythonFileItem), typeof(OAIronPythonFileItem).GUID);
            //// The following are not specific to python and as such we need a separate GUID (we simply used guidgen.exe to create new guids)
            //this.AddCATIDMapping(typeof(FolderNodeProperties), new Guid("A3273B8E-FDF8-4ea8-901B-0D66889F645F"));
            //// This one we use the same as python file nodes since both refer to files
            //this.AddCATIDMapping(typeof(FileNodeProperties), typeof(PythonFileNodeProperties).GUID);
            //// Because our property page pass itself as the object to display in its grid, we need to make it have the same CATID
            //// as the browse object of the project node so that filtering is possible.
            //this.AddCATIDMapping(typeof(GeneralPropertyPage), typeof(PythonProjectNodeProperties).GUID);
            
            //AddCATIDMapping(typeof(FSharpProjectNodeProperties), typeof(FSharpProjectNodeProperties).GUID);
            //FSharpProjectNode local11 = @this.@this.contents;
            //int num11 = local11.GetHashCode;
            //local11.AddCATIDMapping(typeof(FSharpFileNodeProperties), typeof(FSharpFileNodeProperties).GUID);
            //FSharpProjectNode local12 = @this.@this.contents;
            //int num12 = local12.GetHashCode;
            //local12.AddCATIDMapping(typeof(FileNodeProperties), typeof(FSharpFileNodeProperties).GUID);
            //FSharpProjectNode local13 = @this.@this.contents;
            //int num13 = local13.GetHashCode;
            // AddCATIDMapping(typeof(GeneralPropertyPage), typeof(GeneralPropertyPage).GUID);
 
            // We could also provide CATIDs for references and the references container node, if we wanted to.
        }

        public override int ImageIndex
        {
            get
            {
                return imageIndex;
            }
        }

        /// <summary>
        /// Creates the services exposed by this project.
        /// </summary>
        private object CreateServices(Type serviceType)
        {
            // TODO: figure out services
            object service = null;
            //if (typeof(SVSMDCodeDomProvider) == serviceType)
            //{
            //    service = this.CodeDomProvider;
            //}
            //else if (typeof(System.CodeDom.Compiler.CodeDomProvider) == serviceType)
            //{
            //    service = this.CodeDomProvider.CodeDomProvider;
            //}
            //else if (typeof(DesignerContext) == serviceType)
            //{
            //    service = this.DesignerContext;
            //}
            /*else*/ 
            if (typeof(VSLangProj.VSProject) == serviceType)
            {
                service = this.VSProject;
            }
            else if (typeof(EnvDTE.Project) == serviceType)
            {
                service = this.GetAutomationObject();
            }
            return service;
        }

        /// <summary>
        /// Get the VSProject corresponding to this project
        /// </summary>
        protected internal VSLangProj.VSProject VSProject
        {
            get
            {
                if (vsProject == null)
                    vsProject = new OAVSProject(this);
                return vsProject;
            }
        }

    }
}
