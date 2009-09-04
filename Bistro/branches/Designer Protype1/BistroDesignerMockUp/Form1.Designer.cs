namespace BistroDesignerMockUp
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TitlePane = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.InfoListBox = new System.Windows.Forms.ListBox();
            this.control1 = new Bistro.Designer.Explorer.Control();
            this.TitlePane.SuspendLayout();
            this.SuspendLayout();
            // 
            // TitlePane
            // 
            this.TitlePane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TitlePane.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TitlePane.Controls.Add(this.textBox1);
            this.TitlePane.Controls.Add(this.button1);
            this.TitlePane.Location = new System.Drawing.Point(11, 12);
            this.TitlePane.Name = "TitlePane";
            this.TitlePane.Size = new System.Drawing.Size(667, 38);
            this.TitlePane.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 8);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(174, 20);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(187, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Find";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // InfoListBox
            // 
            this.InfoListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.InfoListBox.FormattingEnabled = true;
            this.InfoListBox.Location = new System.Drawing.Point(502, 56);
            this.InfoListBox.MultiColumn = true;
            this.InfoListBox.Name = "InfoListBox";
            this.InfoListBox.Size = new System.Drawing.Size(174, 407);
            this.InfoListBox.TabIndex = 3;
            // 
            // control1
            // 
            this.control1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.control1.AutoSize = true;
            this.control1.BackColor = System.Drawing.SystemColors.Window;
            this.control1.Items = ((System.Collections.Generic.IList<string>)(resources.GetObject("control1.Items")));
            this.control1.Location = new System.Drawing.Point(11, 56);
            this.control1.Margin = new System.Windows.Forms.Padding(0);
            this.control1.Name = "control1";
            this.control1.Root = null;
            this.control1.Size = new System.Drawing.Size(486, 411);
            this.control1.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(685, 482);
            this.Controls.Add(this.InfoListBox);
            this.Controls.Add(this.control1);
            this.Controls.Add(this.TitlePane);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Bistro Designer MockUp";
            this.TitlePane.ResumeLayout(false);
            this.TitlePane.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel TitlePane;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private Bistro.Designer.Explorer.Control control1;
        public System.Windows.Forms.ListBox InfoListBox;
    }
}

