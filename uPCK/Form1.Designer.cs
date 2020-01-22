namespace uPCK
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnUnpack = new System.Windows.Forms.Button();
            this.btnCompress = new System.Windows.Forms.Button();
            this.cmbCompLvl = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblFile = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // btnUnpack
            // 
            this.btnUnpack.Location = new System.Drawing.Point(12, 12);
            this.btnUnpack.Name = "btnUnpack";
            this.btnUnpack.Size = new System.Drawing.Size(75, 23);
            this.btnUnpack.TabIndex = 0;
            this.btnUnpack.Text = "Unpack";
            this.btnUnpack.UseVisualStyleBackColor = true;
            this.btnUnpack.Click += new System.EventHandler(this.btnUnpack_Click);
            // 
            // btnCompress
            // 
            this.btnCompress.Location = new System.Drawing.Point(12, 41);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(75, 23);
            this.btnCompress.TabIndex = 1;
            this.btnCompress.Text = "Compress";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // cmbCompLvl
            // 
            this.cmbCompLvl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCompLvl.FormattingEnabled = true;
            this.cmbCompLvl.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.cmbCompLvl.Location = new System.Drawing.Point(43, 90);
            this.cmbCompLvl.Name = "cmbCompLvl";
            this.cmbCompLvl.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmbCompLvl.Size = new System.Drawing.Size(44, 21);
            this.cmbCompLvl.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Compress Level";
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(107, 12);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(29, 13);
            this.lblFile.TabIndex = 5;
            this.lblFile.Text = "File: ";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(107, 74);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 6;
            // 
            // progBar
            // 
            this.progBar.Location = new System.Drawing.Point(110, 90);
            this.progBar.Name = "progBar";
            this.progBar.Size = new System.Drawing.Size(439, 23);
            this.progBar.Step = 1;
            this.progBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progBar.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 123);
            this.Controls.Add(this.progBar);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbCompLvl);
            this.Controls.Add(this.btnCompress);
            this.Controls.Add(this.btnUnpack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "uPCK";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUnpack;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.ComboBox cmbCompLvl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progBar;
    }
}

