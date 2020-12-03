// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System.Collections.Generic;

namespace AMWE_RealTime_Server.Models
{
    public class VerifyVersion
    {
        public string version { get; internal set; }
        public bool isNotSupported { get; internal set; }
        public bool isLatest { get; internal set; }
        public bool isUpdateNeeded { get; internal set; }
        public List<string> rephandler { get; internal set; }
        public List<string> m3md2 { get; internal set; }
        public List<string> m3md2_startup { get; internal set; }
        public string custommessage { get; internal set; }
    }
}