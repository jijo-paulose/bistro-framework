using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Designer.ProjectBase;

namespace Bistro.Designer.Projects.FSharp
{
    public class ProjectManager : ProjectNode
    {
        private DesignerPackage package;

        public ProjectManager(DesignerPackage package)
        {
            this.package = package;

            // I would rather override the property to always return true
            this.SupportsProjectDesigner = true;

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

        public override Guid ProjectGuid
        {
            get { return typeof(Factory).GUID; }
        }

        public override string ProjectType
        {
            get { return "Bistro"; }
        }

        protected override ConfigProvider CreateConfigProvider()
        {
            return base.CreateConfigProvider();
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
    }
}
