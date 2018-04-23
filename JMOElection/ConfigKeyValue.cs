﻿using System;
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

        public ConfigKeyValue()
        {
            Candidate_Images_Path = Application.StartupPath;
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
                if (value.StartsWith("\\") || value.Contains(":"))
                    Candidate_Images_Path = value;
                else
                    Candidate_Images_Path = System.IO.Path.Combine(Application.StartupPath, value);
            }


        }
    }
}