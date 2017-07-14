using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace _2048CSharp
{

    public class ScorePanel : Panel
    {
        private string _Text;
        private Font _TopScoreFont;
        private ScoreDT _ScoreDataTable;

        #region "Constructors"

        public ScorePanel() : this(string.Empty) { }

        public ScorePanel(string Text)
        {
            DoubleBuffered = true;
            _Text = Text;
            _TopScoreFont = new Font(this.Font.Name, 30, FontStyle.Bold);
            _ScoreDataTable = new ScoreDT();
        }

        #endregion

        public override string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        public ScoreDT ScoreDataTable
        {
            get { return _ScoreDataTable; }
            set { _ScoreDataTable = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString(_Text, _TopScoreFont, Brushes.Black, e.ClipRectangle, sf);
        }

    }
}
