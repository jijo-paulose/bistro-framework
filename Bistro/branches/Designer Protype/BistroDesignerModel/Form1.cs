using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;


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
        private Panel panel1;
        private TextBox textBox1;
        private int Count = 0;
        private List<TreeNode> findNodes = null;
        private Button button1;
        private Label label1;
        private Label label2;
        TreeViewDeSerializer serializer = new TreeViewDeSerializer();
        
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.imageList1.Images.SetKeyName(29, "ErrorBinder.bmp");
            this.imageList1.Images.SetKeyName(30, "ErrorController.bmp");
            this.imageList1.Images.SetKeyName(31, "Errorurl.PNG");
            // 
            // contextMenuStripSource
            // 
            this.contextMenuStripSource.Name = "contextMenuStripSource";
            this.contextMenuStripSource.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStripSource.Text = "Go to Source";
            // 
            // treeView2
            // 
            this.treeView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView2.ImageIndex = 0;
            this.treeView2.ImageList = this.imageList1;
            this.treeView2.Location = new System.Drawing.Point(0, 1);
            this.treeView2.Name = "treeView2";
            this.treeView2.SelectedImageIndex = 0;
            this.treeView2.Size = new System.Drawing.Size(393, 259);
            this.treeView2.TabIndex = 4;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.BackColor = System.Drawing.SystemColors.Window;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 18;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 66);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 7;
            this.treeView1.Size = new System.Drawing.Size(393, 262);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView2);
            this.splitContainer1.Size = new System.Drawing.Size(393, 582);
            this.splitContainer1.SplitterDistance = 327;
            this.splitContainer1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.MinimumSize = new System.Drawing.Size(390, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(393, 60);
            this.panel1.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Binding:";
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(42, 8);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(186, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "<search>";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Location = new System.Drawing.Point(232, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Find";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(393, 582);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(390, 500);
            this.Name = "Form1";
            this.Text = "Bistro Designer Mockup";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
            //To show the old version, you must change the name on MainTreeViewOld.xml
            serializer.DeserializeTreeView(this.treeView1, GetAppPath(Application.StartupPath) + "MainTreeViewOld.xml");
            serializer.myForm = this;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeView2.Nodes.Clear();
            string item = String.Empty;

            #region Define Controllers
            if (e.Node.Text.Contains("c1"))
            {
                item = "c1.xml";
            }
            
            if (e.Node.Text.Contains("c2"))
            {
                item = "c2.xml";
            } 
            
            if (e.Node.Text.Contains("DefaultController"))
            {
                item = "DefaultController.xml";
            }

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

            if (e.Node.Text == "UrlDataAccessControl")
            {
                item = "UrlDataAccessControl.xml";
            }

            if (e.Node.Text == "AdConverter")
            {
                item = "AdConverter.xml";
            }
            #endregion

            #region Define Resources
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

            if (e.Node.Text.Contains("url"))
            {
                item = "Resource6.xml";
            }

            if (e.Node.Text.Contains("Resource7"))
            {
                item = "Resource7.xml";
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
            #endregion

            #region Define Bindings
            
            string txtBinding = String.Empty;
            if (e.Node.Text == "[ANY]/a/*/b/c")
            {
                txtBinding = e.Node.Text;
            }

            if (e.Node.Text == "[ANY]/a/z/*/c")
            {
                txtBinding = e.Node.Text;
            }

            if (e.Node.Text == "[GET]/a/z/b/c")
            {
                txtBinding = e.Node.Text;
            }

            if (e.Node.Text == "[ANY]/?")
            {
                txtBinding = e.Node.Text;
                item = "Binding1.xml";
            }

            if (e.Node.Text == "[ANY]/?/byId/{postingId}" || e.Node.Text == "/byId/{postingId}")
            {
                txtBinding = "[ANY]/?/byId/{postingId}";
                item = "Binding2.xml";
            }

            if (e.Node.Text == "[ANY]/?/byname/{shortName}" || e.Node.Text == "/byname/{shortName}")
            {
                txtBinding = "[ANY]/?/byname/{shortName}";
                item = "Binding3.xml";
            }

            if (e.Node.Text == "[GET]/posting/ad/byname/{shortName}")
            {
                txtBinding = e.Node.Text;
                item = "Binding4.xml";
            }

            if (e.Node.Text == "[POST]/posting/ad/byname/{shortName}")
            {
                txtBinding = e.Node.Text;
                item = "Binding5.xml";
            }

            if (e.Node.Text == "[GET]/posting/adconvert//byurl/{url}")
            {
                txtBinding = e.Node.Text;
                item = "Binding6.xml";
            }

            label2.Text = txtBinding;
            #endregion

            if (!string.IsNullOrEmpty(item))
                ShowInfoTreeView(this.treeView2, item);
        }

        private void ShowInfoTreeView(TreeView treeView, string item)
        {
            treeView.Nodes.Clear();
            serializer.DeserializeTreeView(treeView, GetAppPath(Application.StartupPath) + item);
            //Expanded all Nodes on detail tree
            treeView2.ExpandAll();
        }

        private string GetAppPath(string startUpPath) {
            return Regex.Replace(startUpPath,
         @"\b(bin)\W*(Debug)",
         @"app_data\");

        }

        public void FindTreeNode(TreeNodeCollection treeNodeCollection, string searchText, ref List<TreeNode> findNodes)
        {   
            if (findNodes == null){
                findNodes = new List<TreeNode>();
            }
            foreach (TreeNode child in treeNodeCollection)
            {

                //Strict match
                //if (child.Text.ToLower().CompareTo(searchText.ToLower())== 0)
                
                //Match with Contains
                if (child.Text.ToLower().Contains(searchText.ToLower()) || IsErrorTag(child, searchText))
                {
                    findNodes.Add(child);
                }
                FindTreeNode(child.Nodes, searchText, ref findNodes);
            }
        }

        //Func. draws the erroneous nodes
        private bool IsErrorTag(TreeNode child, string searchText) {
            Regex Parser = new Regex(searchText.ToLower());
            Match match = Parser.Match(child.Tag.ToString().ToLower());
            if (match.Success)
                return true;
            return false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sender.GetType().Name == "TextBox")
            {
                button1.Select();
            }

            if (Count == 0)
            {   
                if(findNodes != null)        
                    findNodes.Clear();
                FindTreeNode(treeView1.Nodes, textBox1.Text, ref findNodes);
            }
            if (Count < findNodes.Count)
                this.treeView1.SelectedNode = findNodes[Count];
            if (Count == findNodes.Count)
            {
                Count = 0;
            }
            else
            {
                Count++;
            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (findNodes != null)
                findNodes.Clear();
            Count = 0;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                this.button1_Click(sender,new EventArgs());
            }
        }
 
	}
}
