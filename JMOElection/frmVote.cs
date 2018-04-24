using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace JMOElection
{
    public partial class frmVote : Form
    {
        private List<CandidateCtl> candidates = new List<CandidateCtl>();
        private bool bAllowVoting = false;

        public frmVote()
        {
            InitializeComponent();

            foreach(Candidate candidate in Program.CandidatesConfig)
            {
                CandidateCtl c = new CandidateCtl(candidate);
                c.selectionChanged += SelectionChanged;
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
                g.Visible = bAllowVoting;

                this.Controls.Add(g);

                CurX += Margin + Width;
            }
            CurY = CurY + Height + Margin;

            cmdConfirm.Left = LeftMargin;
            cmdConfirm.Top = CurY;
            SelectionChanged();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllowVoting(false, "Starting up...");
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

        private void SubmitVote()
        {

            //using (var f = new StreamWriter(ConfigFileFullName))
            //{
            //    string s;
            //}
        }

        private void cmdConfirm_Click(object sender, EventArgs e)
        {

            string ConfirmString = "";
            List<Candidate> selections = new List<Candidate>();
            foreach (CandidateCtl c in candidates)
                if (c.Selected)
                {
                    selections.Add(c.Candidate);
                    ConfirmString += c.Candidate.Code + " " + c.Candidate.Name + "\r\n";
                }

            if (selections.Count == 0)
            {
                MessageBox.Show(this, "You have not selected any candidates", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (selections.Count > 10)
            {
                MessageBox.Show(this, "Can not select more than 10 candidates");
                return;
            }

            if (MessageBox.Show(this, "Are you sure you are voting for the following candidates?\n\n " + ConfirmString, "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SubmitVote();
            }

            
        }

        private void AllowVoting(bool allow, string msg)
        {
            bAllowVoting = allow;            
            cmdConfirm.Visible = allow;
            lblLarge.Text = msg;
            lblLarge.Visible = !allow;
            label1.Visible = allow;

            if (!allow)
                foreach (CandidateCtl g in candidates)
                    g.Selected = false;

            RefreshDisplay();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                JMOServiceReference.JMOVoteServiceClient client = new JMOServiceReference.JMOVoteServiceClient();
                AllowVoting(client.AllowVote(1), "Waiting...");
            }
            catch (Exception)
            {
                AllowVoting(false, "Link Failure");
            }

        }
    }
}
