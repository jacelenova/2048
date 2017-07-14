using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _2048CSharp
{
    public partial class frmMain : Form
    {
        private PlayPanel PP;

        public frmMain()
        {
            InitializeComponent();
        }

        private void NewGame()
        {
            PP.Restart();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            PP = new PlayPanel(4,4);
            PP.Location = new Point(0, stripMenu.Height);
            PP.PlayState_Changed += new PlayPanel.PlayState_Changed_Del(PP_PlayState_Changed);

            this.Controls.Add(PP);
            //this.ActiveControl = PP;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = PP.Size;
            this.Height = PP.Height + stripMenu.Height;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
        }

        void PP_PlayState_Changed(PlayPanel sender, PlayStates playstate)
        {
            if (playstate == PlayStates.GAMEOVER)
            {
                tsPause.Enabled = false;
            }
            else if (playstate == PlayStates.PLAYING)
            {
                tsPause.Text = "Pause";
                tsPause.Enabled = true;
            }
            else if (playstate == PlayStates.PAUSED)
            {
                tsPause.Text = "Continue";
                tsPause.Enabled = true;
            }
            else if (playstate == PlayStates.WIN)
            {
                tsPause.Text = "Pause";
                tsPause.Enabled = false;
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            PP.Start();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down) { PP.AddMove(Directions.DOWN); }
            else if (e.KeyCode == Keys.Up) { PP.AddMove(Directions.UP); }
            else if (e.KeyCode == Keys.Right) { PP.AddMove(Directions.RIGHT); }
            else if (e.KeyCode == Keys.Left) { PP.AddMove(Directions.LEFT); }
            else if (e.KeyCode == Keys.Space) { Pause(); }
            else if (e.KeyCode == Keys.F1) { NewGame(); }
        }

        private void Pause()
        {
            if (PP.PlayState == PlayStates.PLAYING) { PP.SetPlayState(PlayStates.PAUSED); }
            else if (PP.PlayState == PlayStates.PAUSED) { PP.SetPlayState(PlayStates.PLAYING); }
        }

        private void tsTesting_Click(object sender, EventArgs e)
        {
            MessageBox.Show(MySquare.TotalSquareCreated.ToString());
        }
    }
}
