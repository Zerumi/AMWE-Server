// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHandler
{
    public class Report
    {
        public Client Client { get; set; }
        public AlarmList DangerLevel { get; set; }
        public ReportDetails ReportDetails { get; set; }
        public string ReportSummary { get; set; }
    }
}