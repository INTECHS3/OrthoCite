namespace OrthoCite.Launcher
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.ListDatasaves = new System.Windows.Forms.ListBox();
            this.TextName = new System.Windows.Forms.TextBox();
            this.BtnAdd = new System.Windows.Forms.Button();
            this.BtnLoad = new System.Windows.Forms.Button();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ListDatasaves
            // 
            this.ListDatasaves.FormattingEnabled = true;
            this.ListDatasaves.Location = new System.Drawing.Point(12, 41);
            this.ListDatasaves.Name = "ListDatasaves";
            this.ListDatasaves.Size = new System.Drawing.Size(260, 173);
            this.ListDatasaves.TabIndex = 0;
            // 
            // TextName
            // 
            this.TextName.BackColor = System.Drawing.Color.SandyBrown;
            this.TextName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextName.Location = new System.Drawing.Point(13, 13);
            this.TextName.Name = "TextName";
            this.TextName.Size = new System.Drawing.Size(179, 20);
            this.TextName.TabIndex = 1;
            // 
            // BtnAdd
            // 
            this.BtnAdd.FlatAppearance.BorderSize = 0;
            this.BtnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnAdd.Location = new System.Drawing.Point(198, 12);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(74, 23);
            this.BtnAdd.TabIndex = 2;
            this.BtnAdd.Text = "Ajouter";
            this.BtnAdd.UseVisualStyleBackColor = true;
            this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // BtnLoad
            // 
            this.BtnLoad.BackColor = System.Drawing.Color.ForestGreen;
            this.BtnLoad.FlatAppearance.BorderSize = 0;
            this.BtnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnLoad.ForeColor = System.Drawing.Color.White;
            this.BtnLoad.Location = new System.Drawing.Point(12, 221);
            this.BtnLoad.Name = "BtnLoad";
            this.BtnLoad.Size = new System.Drawing.Size(114, 28);
            this.BtnLoad.TabIndex = 3;
            this.BtnLoad.Text = "Charger";
            this.BtnLoad.UseVisualStyleBackColor = false;
            this.BtnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // BtnDelete
            // 
            this.BtnDelete.BackColor = System.Drawing.Color.DarkRed;
            this.BtnDelete.FlatAppearance.BorderSize = 0;
            this.BtnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnDelete.ForeColor = System.Drawing.Color.White;
            this.BtnDelete.Location = new System.Drawing.Point(158, 220);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(114, 28);
            this.BtnDelete.TabIndex = 4;
            this.BtnDelete.Text = "Supprimer";
            this.BtnDelete.UseVisualStyleBackColor = false;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.BtnDelete);
            this.Controls.Add(this.BtnLoad);
            this.Controls.Add(this.BtnAdd);
            this.Controls.Add(this.TextName);
            this.Controls.Add(this.ListDatasaves);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Launcher OrthoCité";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ListDatasaves;
        private System.Windows.Forms.TextBox TextName;
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.Button BtnLoad;
        private System.Windows.Forms.Button BtnDelete;
    }
}

