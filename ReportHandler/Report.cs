// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;

namespace ReportHandler
{
    public class Report
    {
        public Client Client { get; set; }

        public float OverallRating { get; set; }
        public float KeyBoardRating { get; set; }
        public int MouseRating { get; set; }
        public int ProcessRating { get; set; }

        public ICollection<KeyPressedInfo> KeyPressedInfo { get; set; }
        public int PressingCount { get; set; }
        public bool IsMouseCoordChanged { get; set; }

        public string CurrentProccess { get; set; }
        public ICollection<string> OldProcesses { get; set; }
        public ICollection<string> LastProcesses { get; set; }
        public ICollection<CheckModel> ProcIntersection { get; set; }

        public string MainBrowser { get; set; }
        public ICollection<Site> CurrentSites { get; set; }
        public ICollection<CheckModel> SiteIntersection { get; set; }

        public Uri Server { get; set; }
        public DateTime Timestamp { get; set; }
    }
}