using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Models
{
    public class VerifyVersion
    {
        public string version { get; internal set; }
        public bool isNotSupported { get; internal set; }
        public bool isNotLatest { get; internal set; }
        public bool isUpdateNeeded { get; internal set; }
        public List<string> rephandler { get; internal set; }
        public List<string> m3md2 { get; internal set; }
        public List<string> m3md2_startup { get; internal set; }
        public string custommessage { get; internal set; }
    }
}