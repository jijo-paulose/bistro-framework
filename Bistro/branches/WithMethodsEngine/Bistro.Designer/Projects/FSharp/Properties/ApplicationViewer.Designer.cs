namespace Bistro.Designer.Projects.FSharp.Properties
{
    partial class ApplicationViewer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.assemblyName = new System.Windows.Forms.TextBox();
            this.rootNamespace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Assembly name:";
            // 
            // assemblyName
            // 
            this.assemblyName.Location = new System.Drawing.Point(6, 27);
            this.assemblyName.Name = "assemblyName";
            this.assemblyName.Size = new System.Drawing.Size(248, 20);
            this.assemblyName.TabIndex = 1;
            // 
            // rootNamespace
            // 
            this.rootNamespace.Location = new System.Drawing.Point(277, 27);
            this.rootNamespace.Name = "rootNamespace";
            this.rootNamespace.Size = new System.Drawing.Size(248, 20);
            this.rootNamespace.TabIndex = 3;
            this.rootNamespace.TextChanged += new System.EventHandler(this.rootNamespace_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(274, 11);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Default namespace:";
            // 
            // ApplicationViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rootNamespace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.assemblyName);
            this.Controls.Add(this.label1);
            this.Name = "ApplicationViewer";
            this.Size = new System.Drawing.Size(548, 464);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox assemblyName;
        private System.Windows.Forms.TextBox rootNamespace;
    }
}
