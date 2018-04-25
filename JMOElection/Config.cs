using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace JMOElection
{
    public abstract class Config
    {
        public abstract void LoadConfigItem(string ConfigItem);

        public static string ConfigFileFullName
        {

            get
            {
                string DataPath = Application.StartupPath;
                string file = "setup.config";
                return System.IO.Path.Combine(DataPath, file);
            }
        }

        public static Config GetConfigLoader(string section)
        {
            if (section.ToLower() == "[config]")
            {
                Program.SetupConfig = new ConfigKeyValue();
                return Program.SetupConfig;
            } else if (section.ToLower() == "[cadidates]")
            {
                Program.CandidatesConfig = new ConfigCandidates();
                return Program.CandidatesConfig;
            }

            return null;
        }

        public static void LoadConfig()
        {
            Config currentConfig = null;

            using (var f = new StreamReader(ConfigFileFullName))
            {
                string s;                    


                while ((s = f.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(s))
                        continue;

                    if (!s.StartsWith("["))
                    {
                        if (currentConfig == null)
                            throw new Exception("No configuration Section");

                        currentConfig.LoadConfigItem(s);
                    }
                    else
                        currentConfig = GetConfigLoader(s);
                }

            }

            if (Program.CandidatesConfig == null || Program.SetupConfig == null)
                throw new Exception("Setup file is incomplete");

            if (Program.SetupConfig.Booth == 0)
                throw new Exception("Booth number not defined");

            if (string.IsNullOrEmpty(Program.SetupConfig.VoteResultPath))
                throw new Exception("Vote path not set");

            if (string.IsNullOrEmpty(Program.SetupConfig.VoteResultPathAlt))
                throw new Exception("Alternate Vote path not set");

            Directory.CreateDirectory(Program.SetupConfig.VoteResultPath);
            
            Directory.CreateDirectory(Program.SetupConfig.VoteResultPathAlt);


            // Set absolute path to candidate images
            foreach (Candidate c in Program.CandidatesConfig)
            {
                if (c.PicFile != null)
                {
                    c.PicFile = Path.Combine(Program.SetupConfig.Candidate_Images_Path, c.PicFile);
                    if (!File.Exists(c.PicFile)) // Make sure that the image exists
                        throw new Exception("Image not found");

                }

            }

        }


    }
}
