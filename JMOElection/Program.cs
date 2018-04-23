using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace JMOElection
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Config.LoadConfig())
                Application.Run(new frmVote());
            else
            {
                MessageBox.Show("Unable to load config file");
                Application.Exit();
            }
        }

        public static ConfigKeyValue SetupConfig { get; set; }
        public static ConfigCandidates CandidatesConfig { get; set; } 





    }
}
