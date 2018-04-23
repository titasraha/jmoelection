using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JMOElection
{
    public delegate bool SelectionChanged();
    public partial class CandidateCtl : UserControl
    {
        private Color DefaultBackgroundColor;
        public bool Selected { get; set; }
        public event SelectionChanged selectionChanged;
        public Candidate Candidate { get; private set; }

        public string CandidateName
        { 

            get
            {
                return this.lbl.Text;
            }
            set
            {
                this.lbl.Text = value;
            }
        }

        public string Code
        {
            get
            {
                return this.lbl2.Text;
            }
            set
            {
                this.lbl2.Text = value;
            }
        }

        public void SetImage(string Url)
        {
            this.pic.Load(Url);
        }


        public string Description { get; set; }

        private void UpdateUI()
        {
            
            this.Selected = !this.Selected;
            
            if (selectionChanged != null)
            {
                if (!selectionChanged())
                    this.Selected = !this.Selected;
            }

            this.BackColor = this.Selected ? Color.Green : DefaultBackgroundColor;

        }

        public CandidateCtl(Candidate c)
        {

            InitializeComponent();

            Candidate = c;

            this.DefaultBackgroundColor = this.BackColor;
            this.Selected = false;

            this.CandidateName = "Name: " + c.Name;
            this.Code = "Code " + c.Code;
            if (c.PicFile != null)
                this.SetImage(c.PicFile);
            
        }

        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void pic_Click(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void lbl_Click(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void lbl2_Click(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void CandidateCtl_Load(object sender, EventArgs e)
        {

        }

        private void CandidateCtl_Click(object sender, EventArgs e)
        {
            UpdateUI();
        }
    }
}
