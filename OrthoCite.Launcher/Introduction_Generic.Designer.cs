namespace OrthoCite.Launcher
{
    partial class Introduction_Generic
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
            this.components = new System.ComponentModel.Container();
            this.playerBrowser = new System.Windows.Forms.WebBrowser();
            this.CountVideo = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // playerBrowser
            // 
            this.playerBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playerBrowser.Location = new System.Drawing.Point(0, 0);
            this.playerBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.playerBrowser.Name = "playerBrowser";
            this.playerBrowser.Size = new System.Drawing.Size(1150, 634);
            this.playerBrowser.TabIndex = 0;
            // 
            // CountVideo
            // 
            this.CountVideo.Interval = 1000;
            this.CountVideo.Tick += new System.EventHandler(this.CountVideo_Tick);
            // 
            // Introduction_Generic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(1150, 634);
            this.ControlBox = false;
            this.Controls.Add(this.playerBrowser);
            this.Name = "Introduction_Generic";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Introduction_Generic";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Introduction_Generic_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser playerBrowser;
        private System.Windows.Forms.Timer CountVideo;
    }
}