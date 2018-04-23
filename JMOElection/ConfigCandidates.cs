using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMOElection
{
    class ConfigCandidates : Config, IEnumerable<Candidate>
    {
        private List<Candidate> candidates = new List<Candidate>();

        public IEnumerator<Candidate> GetEnumerator()
        {
            foreach (Candidate c in candidates)
                yield return c;
        }

        public override void LoadConfigItem(string ConfigItem)
        {
            string[] parts = ConfigItem.Split(',');
            if (parts.Length != 3 && parts.Length != 4)
                throw new Exception("Invalid candidate information");

            Candidate c = new Candidate();
            c.Code = parts[0].Trim();
            c.Name = parts[1].Trim();
            c.Description = parts[2].Trim();
            if (parts.Length > 3)
                c.PicFile = parts[3].Trim();

            candidates.Add(c);

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
