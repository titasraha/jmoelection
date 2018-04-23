using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JMOElection
{
    public partial class frmVote : Form
    {
        private List<CandidateCtl> candidates = new List<CandidateCtl>();

        public frmVote()
        {
            InitializeComponent();

            for (int i = 0; i < 30; i++)
            {
                CandidateCtl c = new CandidateCtl();
                c.selectionChanged += SelectionChanged;
                c.CandidateName = "Name: " + i.ToString();
                c.Code = "Code " + i.ToString();
                //c.SetImage(@"C:\Users\Admin\Desktop\Projects\JMOElection\noimage.png");
                candidates.Add(c);
            }

        }

        public bool SelectionChanged()
        {
            int ctr = 0;
            foreach(CandidateCtl c in candidates)
            {
                if (c.Selected)
                    ctr++;
            }
            if (ctr <= 10)
            {
                label1.Text = "Selected Candidates : " + ctr.ToString();
                return true;
            }
            return false;

        }

        public void RefreshDisplay()
        {
            int Width = 209, Height = 175;
            int Margin = 10;
            int TotalWidth = Width + Margin;
            int CanvasWidth = this.Width;
            int LeftMargin = (CanvasWidth - (TotalWidth * (int)(CanvasWidth / TotalWidth))) / 2, TopMargin = 50;                             
            int CurX = LeftMargin, CurY = TopMargin;

            foreach (CandidateCtl g in candidates)
            {
                if (CurX + Width > CanvasWidth)
                {
                    CurX = LeftMargin;
                    CurY = CurY + Height + Margin;
                }

                g.Left = CurX;
                g.Top = CurY;
                g.Width = Width;
                g.Height = Height;
                this.Controls.Add(g);

                CurX += Margin + Width;


            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshDisplay();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {

            RefreshDisplay();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            RefreshDisplay();
        }
    }
}
