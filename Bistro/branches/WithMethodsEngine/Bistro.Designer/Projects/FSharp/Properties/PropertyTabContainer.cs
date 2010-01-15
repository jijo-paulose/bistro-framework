using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Bistro.Designer.ProjectBase;
using Microsoft.VisualStudio.OLE.Interop;
using System.Windows.Forms;
using System.Drawing;

namespace Bistro.Designer.Projects.FSharp
{
	[CLSCompliant(false), ComVisible(true)]
    public abstract class PropertyTabContainer : SettingsPage
    {
        private Control control;

        public override void Activate(IntPtr parent, RECT[] pRect, int bModal)
        {
            if (this.control == null)
            {
                this.control = CreateControl();
                this.control.Size = new Size(pRect[0].right - pRect[0].left, pRect[0].bottom - pRect[0].top);
                this.control.Visible = false;
                this.control.Size = new Size(550, 300);
                this.control.CreateControl();
                NativeMethods.SetParent(this.control.Handle, parent);
                BindProperties();
            }
        }

        protected abstract Control CreateControl();

        public override void Show(uint cmd)
        {
            this.control.Visible = true; // TODO: pass SW_SHOW* flags through      
            this.control.Show();
        }

        public override void Move(RECT[] arrRect)
        {
            RECT r = arrRect[0];

            this.control.Location = new Point(r.left, r.top);
            this.control.Size = new Size(r.right - r.left, r.bottom - r.top);
        }

    }
}
