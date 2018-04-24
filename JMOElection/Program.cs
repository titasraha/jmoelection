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
            bool bConfigLoaded = false;
            try
            {
                Config.LoadConfig();
                bConfigLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            if (bConfigLoaded)
                Application.Run(new frmVote());

        }

        public static ConfigKeyValue SetupConfig { get; set; }
        public static ConfigCandidates CandidatesConfig { get; set; } 





    }
}
