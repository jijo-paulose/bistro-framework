using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.ProjectBase;
using System.Drawing;
using System.Windows.Forms;
using Bistro.Designer.ProjectBase.Automation;

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
        /// Provide mapping from our browse objects and automation objects to our CATIDs
        /// </summary>
        private void InitializeCATIDs()
        {
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
            //AddCATIDMapping(typeof(Microsoft.VisualStudio.FSharp.ProjectSystem.GeneralPropertyPage), typeof(ProjectNodeProperties).GUID);
            //Notifier sourcesAndFlagsNotifier = @this.sourcesAndFlagsNotifier;
            //FSharpProjectNode local14 = @this.@this.contents;
            //int num14 = local14.GetHashCode;
 
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
        /// Returns the project type guid AKA the guid of the project factory type
        /// </summary>
        public override Guid ProjectGuid
        {
            get { return typeof(Factory).GUID; } 
        }

        public override string ProjectType
        {
            get { return "Bistro"; }
        }

        /// <summary>
        /// Creates the services exposed by this project.
        /// </summary>
        private object CreateServices(Type serviceType)
        {
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

        protected override Guid[] GetConfigurationIndependentPropertyPages()
        {
            Guid[] result = new Guid[1];
            //result[0] = typeof(BuildPropertyPage).GUID;
            result[0] = typeof(BuildOrderPage).GUID;
//            result[2] = new Guid("6D2D9B56-2691-4624-A1BF-D07A14594748");
            return result;
        }

        public List<string> Files = new List<string>();

        public override FileNode CreateFileNode(ProjectElement item)
        {
            Files.Add("Name = " + item.Item.FinalItemSpec);
            return base.CreateFileNode(item);
        }

    }
}
