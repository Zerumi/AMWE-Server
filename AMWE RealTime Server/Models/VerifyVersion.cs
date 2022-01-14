// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System.Collections.Generic;

namespace AMWE_RealTime_Server.Models
{
    public class VerifyVersion
    {
        public string Version { get; internal set; }
        public bool IsNotSupported { get; internal set; }
        public bool IsLatest { get; internal set; }
        public bool IsUpdateNeeded { get; internal set; }
        public List<string> Rephandler { get; internal set; }
        public List<string> M3md2 { get; internal set; }
        public List<string> M3md2_startup { get; internal set; }
        public string Custommessage { get; internal set; }
    }
}