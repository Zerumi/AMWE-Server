// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System.Collections.Generic;

namespace AMWE_RealTime_Server.Models
{
    public class Report
    {
        public Client Client { get; set; }

        public float OverallRating { get; set; }
        public int KeyBoardRating { get; set; }
        public int MouseRating { get; set; }
        public int ProcessRating { get; set; }

        public ICollection<KeyPressedInfo> KeyPressedInfo { get; set; }
        public int PressingCount { get; set; }
        public bool IsMouseCoordChanged { get; set; }

        public int ProcessChangedCount { get; set; }
        public ICollection<string> OldProcesses { get; set; }
        public ICollection<string> LastProcesses { get; set; }
    }
}
