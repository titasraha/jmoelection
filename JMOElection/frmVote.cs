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
using System.Drawing.Printing;

namespace JMOElection
{
    public partial class frmVote : Form
    {

        private Font verdana10Font;
        private StreamReader reader;
        private string CurrentVoteFile;


        private List<CandidateCtl> candidates = new List<CandidateCtl>();
        private bool bAllowVoting = false;
        private bool bVoteVisible = false;
        private Random rnd = new Random();

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
                g.Visible = bVoteVisible;

                this.Controls.Add(g);

                CurX += Margin + Width;
            }
            CurY = CurY + Height + Margin;

            cmdConfirm.Left = LeftMargin;
            cmdConfirm.Top = CurY;
            cmdConfirm.Width = CanvasWidth - LeftMargin * 2;
            SelectionChanged();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllowVoting(!Program.SetupConfig.IsControllerActive, "Starting up...");
            DoFeedback();
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

        private void DoFeedback()
        {
            int TotalVotes = Directory.GetFiles(Program.SetupConfig.VoteResultPath, "*.vote", SearchOption.TopDirectoryOnly).Length;
            try
            {
                JMOServiceReference.JMOVoteServiceClient client = new JMOServiceReference.JMOVoteServiceClient();
                client.VoteFeedback(Program.SetupConfig.Booth, TotalVotes, true);
            }
            catch (Exception)
            {

            }
        }

        private void SubmitVote(List<Candidate> selections)
        {

            string RndVoteFilePath;
            string RndVoteFilePathAlt;
            int RndValue;

            do
            {
                RndValue = rnd.Next(1000, 10000);
                string RndVoteFile = RndValue.ToString() + ".vote";
                RndVoteFilePath = Path.Combine(Program.SetupConfig.VoteResultPath, RndVoteFile);
                RndVoteFilePathAlt = Path.Combine(Program.SetupConfig.VoteResultPathAlt, RndVoteFile);
            } while (File.Exists(RndVoteFilePath));


            string v = "";
            using (var f = new StreamWriter(RndVoteFilePath))
            {
                f.WriteLine("Vote Code: " + RndValue.ToString());

                foreach (Candidate c in selections)
                {
                    f.WriteLine(c.Code + ": " + c.Name);
                    v += c.Code + " " + c.Name + "\r\n";
                }

            }

            File.Copy(RndVoteFilePath, RndVoteFilePathAlt);

            lblVotes.Text = v;
            lblCode.Text = RndValue.ToString();
            CurrentVoteFile = RndVoteFilePath;
            SetState(States.Result, "Vote Captured");

            DoFeedback();

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
                SubmitVote(selections);
            }

            
        }

        private void SetState(States states, string msg)
        {
            bAllowVoting = states != States.Standby;
            bVoteVisible = states == States.Voting;
            cmdConfirm.Visible = states == States.Voting;
            lblLarge.Text = msg;
            lblLarge.Visible = states != States.Voting;
            label1.Visible = states == States.Voting;
            panFinal.Visible = states == States.Result;

            if (states == States.Standby)
                foreach (CandidateCtl g in candidates)
                    g.Selected = false;

            RefreshDisplay();
        }

        private void AllowVoting(bool allow, string msg)
        {
            if (bAllowVoting && allow)
                return;

            SetState(allow ? States.Voting : States.Standby, msg);
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Program.SetupConfig.IsControllerActive)
                try
                {
                    JMOServiceReference.JMOVoteServiceClient client = new JMOServiceReference.JMOVoteServiceClient();
                    AllowVoting(client.AllowVote(Program.SetupConfig.Booth), "Waiting...");
                }
                catch (Exception)
                {
                    AllowVoting(false, "Link Failure");
                }

        }

        private void PrintTextFileHandler(object sender, PrintPageEventArgs ppeArgs)
        {
            Graphics g = ppeArgs.Graphics;
            float linesPerPage = 0;            
            int count = 0;

            float leftMargin = ppeArgs.MarginBounds.Left;
            float topMargin = ppeArgs.MarginBounds.Top;
            string line = null;
            float yPos = topMargin;


            linesPerPage = ppeArgs.MarginBounds.Height / verdana10Font.GetHeight(g);

            Font heading = new Font("Verdana", 18);
            Font subHeading = new Font("Verdana", 14);

            g.DrawString("JM Orchid Apartment Owners Association", heading, Brushes.Black, leftMargin, yPos, new StringFormat());
            yPos += heading.GetHeight(g);

            g.DrawString("Election of BOM 2018", subHeading, Brushes.Black, leftMargin, yPos, new StringFormat());
            yPos += subHeading.GetHeight(g) * 2 ;

            while (count < linesPerPage && ((line = reader.ReadLine()) != null))
            {
                //yPos = topMargin + (count * verdana10Font.GetHeight(g));
                g.DrawString(line, verdana10Font, Brushes.Black, leftMargin, yPos, new StringFormat());
                yPos += verdana10Font.GetHeight(g);
                count++;
            }

            if (line != null)
                ppeArgs.HasMorePages = true;
            else
                ppeArgs.HasMorePages = false;
        }

        private void cmdPrint_Click(object sender, EventArgs e)
        {
            reader = new StreamReader(CurrentVoteFile);
            verdana10Font = new Font("Verdana", 10);
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(this.PrintTextFileHandler);            
            pd.Print();


            if (reader != null)
                reader.Close();
        }
    }

    public enum States
    {
        Standby, Voting, Result
    }
}
