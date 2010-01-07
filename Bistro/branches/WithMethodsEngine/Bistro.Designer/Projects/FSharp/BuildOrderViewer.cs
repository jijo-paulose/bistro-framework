using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bistro.Designer.ProjectBase;

namespace Bistro.Designer.Projects.FSharp
{
    public partial class BuildOrderViewer : UserControl
    {
        ProjectManager project;
        public BuildOrderViewer(ProjectManager project)
        {
            this.project = project;
            InitializeComponent();
            LoadViewer(0, project);

            foreach (string name in project.Files)
                this.Dependencies.Nodes.Add(name);
        }

        private void LoadViewer(int level, HierarchyNode node)
        {
            this.Dependencies.Nodes.Add(level.ToString() + ": " + node.Caption);
            if (node.NextSibling != null)
                LoadViewer(level, node.NextSibling);
            if (node.FirstChild != null)
                LoadViewer(level + 1, node.FirstChild);
        }
    }
}
