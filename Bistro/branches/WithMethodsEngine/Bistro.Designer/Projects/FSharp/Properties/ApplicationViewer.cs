using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bistro.Designer.Projects.FSharp.Properties
{
    public partial class ApplicationViewer : UserControl
    {
        ApplicationPage page;
        public ApplicationViewer(ApplicationPage page)
        {
            this.page = page;
            InitializeComponent();
        }

        internal void BindProperties()
        {
            rootNamespace.Text = page.RootNamespace;
        }

        private void rootNamespace_TextChanged(object sender, EventArgs e)
        {
            page.RootNamespace = rootNamespace.Text;
        }
    }
}
