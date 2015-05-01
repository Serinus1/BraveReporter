namespace BraveIntelReporter
{
    partial class frmSettings
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
            this.txtChatlogPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseForChatlogs = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAuthToken = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkRunOnStartup = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.chkDisableReporting = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtChatlogPath
            // 
            this.txtChatlogPath.Location = new System.Drawing.Point(12, 29);
            this.txtChatlogPath.Name = "txtChatlogPath";
            this.txtChatlogPath.Size = new System.Drawing.Size(350, 20);
            this.txtChatlogPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Eve Chatlog Folder";
            // 
            // btnBrowseForChatlogs
            // 
            this.btnBrowseForChatlogs.Location = new System.Drawing.Point(368, 27);
            this.btnBrowseForChatlogs.Name = "btnBrowseForChatlogs";
            this.btnBrowseForChatlogs.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseForChatlogs.TabIndex = 2;
            this.btnBrowseForChatlogs.Text = "Browse...";
            this.btnBrowseForChatlogs.UseVisualStyleBackColor = true;
            this.btnBrowseForChatlogs.Click += new System.EventHandler(this.btnBrowseForChatlogs_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Authentication Token";
            // 
            // txtAuthToken
            // 
            this.txtAuthToken.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAuthToken.Location = new System.Drawing.Point(12, 81);
            this.txtAuthToken.Name = "txtAuthToken";
            this.txtAuthToken.Size = new System.Drawing.Size(222, 20);
            this.txtAuthToken.TabIndex = 4;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(364, 136);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(283, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkRunOnStartup
            // 
            this.chkRunOnStartup.AutoSize = true;
            this.chkRunOnStartup.Location = new System.Drawing.Point(16, 120);
            this.chkRunOnStartup.Name = "chkRunOnStartup";
            this.chkRunOnStartup.Size = new System.Drawing.Size(98, 17);
            this.chkRunOnStartup.TabIndex = 7;
            this.chkRunOnStartup.Text = "Run on Startup";
            this.chkRunOnStartup.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(121, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "( from ";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(153, 65);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(45, 13);
            this.linkLabel1.TabIndex = 9;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "the map";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(194, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "click uploader.)";
            // 
            // chkDisableReporting
            // 
            this.chkDisableReporting.AutoSize = true;
            this.chkDisableReporting.Location = new System.Drawing.Point(305, 82);
            this.chkDisableReporting.Name = "chkDisableReporting";
            this.chkDisableReporting.Size = new System.Drawing.Size(110, 17);
            this.chkDisableReporting.TabIndex = 11;
            this.chkDisableReporting.Text = "Disable Reporting";
            this.chkDisableReporting.UseVisualStyleBackColor = true;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 171);
            this.Controls.Add(this.chkDisableReporting);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkRunOnStartup);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtAuthToken);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowseForChatlogs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtChatlogPath);
            this.Name = "frmSettings";
            this.Text = "Reporter Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtChatlogPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseForChatlogs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAuthToken;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkRunOnStartup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkDisableReporting;
    }
}