using System.Security.Permissions;
using System.Windows.Forms;
using System.Collections.Generic;
using Bistro.MethodsEngine;

namespace Bistro.Designer.Explorer
{
    /// <summary>
    /// Summary description for MyControl.
    /// </summary>
    public partial class Explorer : UserControl
    {
        public Dictionary<string,List<ControllerDescription>> cashPatternsCtrl;
        public Dictionary<string,Dictionary<string,Resource>> cashPatternsRes;
        public Explorer()
        {
            InitializeComponent();
            cashPatternsCtrl = new Dictionary<string,List<ControllerDescription>>();
            cashPatternsRes = new Dictionary<string,Dictionary<string,Resource>>();
        }

        /// <summary> 
        /// Let this control process the mnemonics.
        /// </summary>
        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogChar(char charCode)
        {
              // If we're the top-level form or control, we need to do the mnemonic handling
              if (charCode != ' ' && ProcessMnemonic(charCode))
              {
                    return true;
              }
              return base.ProcessDialogChar(charCode);
        }

        /// <summary>
        /// Enable the IME status handling for this control.
        /// </summary>
        protected override bool CanEnableIme
        {
            get
            {
                return true;
            }
        }
        public TreeView Tree
        {
            get
            {
                return treeView;
            }
        }

        /*[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show(this,
                            string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "Bistro Explorer");
        }*/
    }
}
