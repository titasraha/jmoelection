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

        public static bool LoadConfig()
        {
            Config currentConfig = null;

            try
            {
                using (var f = new StreamReader(ConfigFileFullName))
                {
                    string s;                    


                    while ((s = f.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(s))
                            continue;

                        if (!s.StartsWith("["))
                        {
                            try
                            {
                                if (currentConfig == null)
                                    throw new Exception("No configuration Section");

                                currentConfig.LoadConfigItem(s);
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                return false;
                            }
                        }
                        else
                            currentConfig = GetConfigLoader(s);
                    }

                }
            }
            catch
            { }

            if (Program.CandidatesConfig == null || Program.SetupConfig == null)
            {
                MessageBox.Show("Setup file is incomplete");
                return false;
            }

            // Set absolute path to candidate images
            foreach (Candidate c in Program.CandidatesConfig)
            {
                if (c.PicFile != null)
                {
                    c.PicFile = System.IO.Path.Combine(Program.SetupConfig.Candidate_Images_Path, c.PicFile);
                    if (!File.Exists(c.PicFile)) // Make sure that the image exists
                    {
                        MessageBox.Show("Image not found");
                        return false;
                    }
                }

            }

            return true;

        }


    }
}
