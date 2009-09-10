using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;


namespace TreeViewSerialization
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
    {
        private System.Windows.Forms.ImageList imageList1;
        private ContextMenuStrip contextMenuStripSource;
        private TreeView treeView2;
        private TreeView treeView1;
        private SplitContainer splitContainer1;
        private System.ComponentModel.IContainer components;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripSource = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ResourceProvider.bmp");
            this.imageList1.Images.SetKeyName(1, "Action.bmp");
            this.imageList1.Images.SetKeyName(2, "Activities.bmp");
            this.imageList1.Images.SetKeyName(3, "Activity.bmp");
            this.imageList1.Images.SetKeyName(4, "Application.bmp");
            this.imageList1.Images.SetKeyName(5, "Binder.bmp");
            this.imageList1.Images.SetKeyName(6, "Binders.bmp");
            this.imageList1.Images.SetKeyName(7, "Document.bmp");
            this.imageList1.Images.SetKeyName(8, "Documents.bmp");
            this.imageList1.Images.SetKeyName(9, "Error.bmp");
            this.imageList1.Images.SetKeyName(10, "source.bmp");
            this.imageList1.Images.SetKeyName(11, "FieldSelected.bmp");
            this.imageList1.Images.SetKeyName(12, "Folder.bmp");
            this.imageList1.Images.SetKeyName(13, "Link.bmp");
            this.imageList1.Images.SetKeyName(14, "ModelExplorerToolWindowBitmaps.bmp");
            this.imageList1.Images.SetKeyName(15, "OpenFolder.bmp");
            this.imageList1.Images.SetKeyName(16, "OpenFolderYellow.bmp");
            this.imageList1.Images.SetKeyName(17, "Resource.bmp");
            this.imageList1.Images.SetKeyName(18, "ResourceConsumer.bmp");
            this.imageList1.Images.SetKeyName(19, "method.bmp");
            this.imageList1.Images.SetKeyName(20, "Field.bmp");
            this.imageList1.Images.SetKeyName(21, "session.ico");
            this.imageList1.Images.SetKeyName(22, "url.png");
            this.imageList1.Images.SetKeyName(23, "controller.bmp");
            this.imageList1.Images.SetKeyName(24, "form1.bmp");
            this.imageList1.Images.SetKeyName(25, "cookies.bmp");
            this.imageList1.Images.SetKeyName(26, "controllerOut.bmp");
            this.imageList1.Images.SetKeyName(27, "controllerIn.bmp");
            this.imageList1.Images.SetKeyName(28, "resource.bmp");
            // 
            // contextMenuStripSource
            // 
            this.contextMenuStripSource.Name = "contextMenuStripSource";
            this.contextMenuStripSource.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStripSource.Text = "Go to Source";
            // 
            // treeView2
            // 
            this.treeView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView2.ImageIndex = 0;
            this.treeView2.ImageList = this.imageList1;
            this.treeView2.Location = new System.Drawing.Point(0, 0);
            this.treeView2.Name = "treeView2";
            this.treeView2.SelectedImageIndex = 0;
            this.treeView2.Size = new System.Drawing.Size(363, 483);
            this.treeView2.TabIndex = 4;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 18;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 7;
            this.treeView1.Size = new System.Drawing.Size(367, 483);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(0, 1);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView2);
            this.splitContainer1.Size = new System.Drawing.Size(734, 483);
            this.splitContainer1.SplitterDistance = 367;
            this.splitContainer1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(733, 486);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Bistro Designer Mockup";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
            this.LoadData();
		}

        private void LoadData()
        {
            this.treeView1.Nodes.Clear();
            TreeViewDeSerializer serializer = new TreeViewDeSerializer();
            serializer.DeserializeTreeView(this.treeView1, GetAppPath(Application.StartupPath) + "MainTreeView.xml"); 
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeView2.Nodes.Clear();
            string item = String.Empty;
            if (e.Node.Text == "DataAccessControl") {
                item = "DataAccessControl.xml";
            }

            if (e.Node.Text == "AdDisplay")
            {
                item = "AdDisplay.xml";
            }

            if (e.Node.Text == "AdUpdate")
            {
                item = "AdUpdate.xml";
            }

            if (e.Node.Text.Contains("Resource1"))
            {
                item = "Resource1.xml";
            }
            
            if (e.Node.Text.Contains("Resource2"))
            {
                item = "Resource2.xml";
            }

            if (e.Node.Text.Contains("Resource3"))
            {
                item = "Resource3.xml";
            }

            if (e.Node.Text.Contains ("Resource5"))
            {
                item = "Resource5.xml";
            }

            if ((string)e.Node.Tag != "Binding")
            {
                if (e.Node.Text.Contains("postingId"))
                {
                    item = "postingId.xml";
                }
                if (e.Node.Text.Contains("shortName"))
                {
                    item = "shortName.xml";
                }
            }

            if (e.Node.Text.Contains("DefaultController"))
            {
                item = "DefaultController.xml";
            }
            
            if (!string.IsNullOrEmpty(item))
                ShowInfoTreeView(this.treeView2, item);
        }

        private void ShowInfoTreeView(TreeView treeView, string item)
        {
            treeView.Nodes.Clear();
            TreeViewDeSerializer serializer = new TreeViewDeSerializer();
            serializer.DeserializeTreeView(treeView, GetAppPath(Application.StartupPath) + item);
            //Expanded all Nodes on detail tree
            treeView2.ExpandAll();
        }

        private string GetAppPath(string startUpPath) {
            return Regex.Replace(startUpPath,
         @"\b(bin)\W*(Debug)",
         @"app_data\");

        }
               
	}
}
