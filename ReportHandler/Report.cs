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