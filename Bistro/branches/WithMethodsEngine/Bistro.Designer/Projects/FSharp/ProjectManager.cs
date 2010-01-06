using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.ProjectBase;
using System.Drawing;
using System.Windows.Forms;

namespace Bistro.Designer.Projects.FSharp
{
    public class ProjectManager : ProjectNode
    {
        private static ImageList imageList;

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

        protected override ProjectBase.ConfigProvider CreateConfigProvider()
        {
            return new ConfigProvider(this);
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
            //else if (typeof(VSLangProj.VSProject) == serviceType)
            //{
            //    service = this.VSProject;
            //}
            //else if (typeof(EnvDTE.Project) == serviceType)
            //{
            //    service = this.GetAutomationObject();
            //}
            return service;
        }

        protected override Guid[] GetConfigurationIndependentPropertyPages()
        {
            Guid[] result = new Guid[2];
            result[0] = typeof(BuildPropertyPage).GUID;
            result[1] = typeof(CompilerPropertyPage).GUID;
            return result;
        }

    }
}
