using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TestDate;


namespace BistroDesignerMockUp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.SetUpDataBinding();
        }

        public void SetUpDataBinding()
        {
            //temporary lines, this lines should be another place
            TestHelper th = new TestHelper();
            control1.CreateRootNode(th.GetTestDescriptor);
            control1.CreateTreeNodes(th.GetTestBinding);
        }
    }
}
