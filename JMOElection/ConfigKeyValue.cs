using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JMOElection
{
    public class ConfigKeyValue : Config
    {
        
        public string Candidate_Images_Path { get; private set; }
        public int Booth { get; private set; }
        public bool IsControllerActive { get; private set; }
        //public string ControllerUrl { get; private set; }
        public string VoteResultPath { get; private set; }
        public string VoteResultPathAlt { get; private set; }

        public ConfigKeyValue()
        {
            Candidate_Images_Path = Application.StartupPath;
        }

        private string GetAbsolutePath(string path)
        {
            return System.IO.Path.GetFullPath(path);
            //if (value.StartsWith("\\") || value.Contains(":"))
            //    Candidate_Images_Path = value;
            //else
            //    Candidate_Images_Path = System.IO.Path.Combine(Application.StartupPath, value);
        }

        public override void LoadConfigItem(string ConfigItem)
        {
            string[] parts = ConfigItem.Split('=');

            if (parts.Length != 2)
                throw new Exception("Invalid Setup Configuration Item");

            string key = parts[0].ToLower();
            string value = parts[1];

            if (key == "candidate_image_path")
            {
                Candidate_Images_Path = GetAbsolutePath(value);
            } else if (key =="booth")
            {
                Booth = Convert.ToInt32(value);
            } else if (key == "use_controller")
            {
                IsControllerActive = value.ToLower() != "no";
            } //else if (key == "controller")
            //{
            //    ControllerUrl = value;
            //} 
            else if (key == "vote_result_path")
            {
                VoteResultPath = GetAbsolutePath(value);
            } else if (key == "vote_result_alt_path")
            {
                VoteResultPathAlt = GetAbsolutePath(value);
            }


        }
    }
}
