// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHandler
{
    public class Report
    {
        public Client Client { get; set; }

        public float OverallRating { get; set; }
        public int KeyBoardRating { get; set; }
        public int MouseRating { get; set; }
        public int ProcessRating { get; set; }

        public KeyPressedInfo[] KeyPressedInfo { get; set; }
        public bool isMouseCoordChanged { get; set; }

        public Process[] OldProcesses { get; set; }
        public Process[] LastProcesses { get; set; }
    }
}