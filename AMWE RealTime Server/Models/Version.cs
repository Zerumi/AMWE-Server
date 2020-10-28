// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Models
{
    public class Version
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string version { get; set; }

        public virtual ICollection<VersionFile> versionfiles { get; set; }
    }
}