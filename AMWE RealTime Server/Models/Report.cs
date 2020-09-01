using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Models
{
    public class Report
    {
        public Client Client { get; set; }
        public AlarmList DangerLevel { get; set; }
        public ReportDetails ReportDetails { get; set; }
        public string ReportSummary { get; set; }
    }
}