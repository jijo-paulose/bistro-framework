using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Bistro.Designer.Core.Projects;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace Bistro.Designer.Explorer
{
    /// <summary>
    /// Summary description for MyControl.
    /// </summary>
    public partial class Control : UserControl
    {
        IProjectManager projectManager;
        public Control()
        {
            InitializeComponent();
            projectManager = (IProjectManager)Package.GetGlobalService(typeof(SProjectManager));
            foreach (Project project in projectManager.Projects)
                CreateNodeForProject(project);
            projectManager.ProjectRemoved += new ProjectDelegate(projectManager_ProjectRemoved);
            projectManager.ProjectAdded += new ProjectDelegate(projectManager_ProjectAdded);
        }

        void CreateNodeForProject(Project project)
        {
            if (!project.IsConverted)
                return;

            MemoryStream stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, project.Icon.ToBitmap());
            string iconKey = "";
            foreach (byte b in new System.Security.Cryptography.SHA1Managed().ComputeHash(stream.ToArray()))
                iconKey += Convert.ToString(b, 16);

            if (!TreeIcons.Images.ContainsKey(iconKey))
                TreeIcons.Images.Add(iconKey, project.Icon);

            TreeNode result = new TreeNode();
            result.Text = project.Name;
            result.Name = project.Name;
            result.Tag = project;
            result.ImageKey = iconKey;
            ControllerView.Nodes.Add(result);
        }

        void projectManager_ProjectAdded(Project project)
        {
            CreateNodeForProject(project);
        }

        void projectManager_ProjectRemoved(Project project)
        {
            ControllerView.Nodes.RemoveByKey(project.Name);
        }
    }
}
