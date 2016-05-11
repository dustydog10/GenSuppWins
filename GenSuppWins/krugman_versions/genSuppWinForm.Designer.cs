namespace GenSuppWins
{
    partial class suppWinForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.selDirDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SourceDirBtn = new System.Windows.Forms.Button();
            this.SrcDirLabel = new System.Windows.Forms.Label();
            this.genSWsBtn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // selDirDialog
            // 
            this.selDirDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // SourceDirBtn
            // 
            this.SourceDirBtn.Location = new System.Drawing.Point(23, 12);
            this.SourceDirBtn.Name = "SourceDirBtn";
            this.SourceDirBtn.Size = new System.Drawing.Size(75, 23);
            this.SourceDirBtn.TabIndex = 0;
            this.SourceDirBtn.Text = "HTML Dir";
            this.SourceDirBtn.UseVisualStyleBackColor = true;
            this.SourceDirBtn.Click += new System.EventHandler(this.SourceDirBtn_Click);
            // 
            // SrcDirLabel
            // 
            this.SrcDirLabel.AutoSize = true;
            this.SrcDirLabel.Location = new System.Drawing.Point(104, 9);
            this.SrcDirLabel.Name = "SrcDirLabel";
            this.SrcDirLabel.Size = new System.Drawing.Size(35, 13);
            this.SrcDirLabel.TabIndex = 1;
            this.SrcDirLabel.Text = "label1";
            // 
            // genSWsBtn
            // 
            this.genSWsBtn.Location = new System.Drawing.Point(23, 50);
            this.genSWsBtn.Name = "genSWsBtn";
            this.genSWsBtn.Size = new System.Drawing.Size(75, 23);
            this.genSWsBtn.TabIndex = 2;
            this.genSWsBtn.Text = "Gen SWs";
            this.genSWsBtn.UseVisualStyleBackColor = true;
            this.genSWsBtn.Click += new System.EventHandler(this.genSWsBtn_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(119, 50);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(362, 199);
            this.textBox1.TabIndex = 3;
            // 
            // suppWinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 261);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.genSWsBtn);
            this.Controls.Add(this.SrcDirLabel);
            this.Controls.Add(this.SourceDirBtn);
            this.Name = "suppWinForm";
            this.Text = "Generate Supplemental Window Files";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog selDirDialog;
        private System.Windows.Forms.Button SourceDirBtn;
        private System.Windows.Forms.Label SrcDirLabel;
        private System.Windows.Forms.Button genSWsBtn;
        private System.Windows.Forms.TextBox textBox1;
    }
}

