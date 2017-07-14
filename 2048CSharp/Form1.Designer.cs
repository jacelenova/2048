namespace _2048CSharp
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
            this.stripMenu = new System.Windows.Forms.MenuStrip();
            this.tsNewGame = new System.Windows.Forms.ToolStripMenuItem();
            this.tsPause = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.tsTesting = new System.Windows.Forms.ToolStripMenuItem();
            this.stripMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // stripMenu
            // 
            this.stripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsNewGame,
            this.tsPause,
            this.tsSave,
            this.tsLoad,
            this.tsTesting});
            this.stripMenu.Location = new System.Drawing.Point(0, 0);
            this.stripMenu.Name = "stripMenu";
            this.stripMenu.Size = new System.Drawing.Size(362, 24);
            this.stripMenu.TabIndex = 0;
            this.stripMenu.Text = "menuStrip1";
            // 
            // tsNewGame
            // 
            this.tsNewGame.Name = "tsNewGame";
            this.tsNewGame.Size = new System.Drawing.Size(77, 20);
            this.tsNewGame.Text = "New Game";
            // 
            // tsPause
            // 
            this.tsPause.Name = "tsPause";
            this.tsPause.Size = new System.Drawing.Size(50, 20);
            this.tsPause.Text = "Pause";
            // 
            // tsSave
            // 
            this.tsSave.Name = "tsSave";
            this.tsSave.Size = new System.Drawing.Size(43, 20);
            this.tsSave.Text = "Save";
            // 
            // tsLoad
            // 
            this.tsLoad.Name = "tsLoad";
            this.tsLoad.Size = new System.Drawing.Size(45, 20);
            this.tsLoad.Text = "Load";
            // 
            // tsTesting
            // 
            this.tsTesting.Name = "tsTesting";
            this.tsTesting.Size = new System.Drawing.Size(78, 20);
            this.tsTesting.Text = "For Testing";
            this.tsTesting.Click += new System.EventHandler(this.tsTesting_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 304);
            this.Controls.Add(this.stripMenu);
            this.MainMenuStrip = this.stripMenu;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "2048";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.stripMenu.ResumeLayout(false);
            this.stripMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip stripMenu;
        private System.Windows.Forms.ToolStripMenuItem tsNewGame;
        private System.Windows.Forms.ToolStripMenuItem tsPause;
        private System.Windows.Forms.ToolStripMenuItem tsSave;
        private System.Windows.Forms.ToolStripMenuItem tsLoad;
        private System.Windows.Forms.ToolStripMenuItem tsTesting;
    }
}

