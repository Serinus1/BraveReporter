namespace BraveIntelReporter
{
    partial class frmMain
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
            this.txtIntel = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtIntel
            // 
            this.txtIntel.Location = new System.Drawing.Point(12, 12);
            this.txtIntel.Multiline = true;
            this.txtIntel.Name = "txtIntel";
            this.txtIntel.ReadOnly = true;
            this.txtIntel.Size = new System.Drawing.Size(671, 313);
            this.txtIntel.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 441);
            this.Controls.Add(this.txtIntel);
            this.Name = "frmMain";
            this.Text = "Brave Intel Reporter";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtIntel;
    }
}

