// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;

namespace AMWE_RealTime_Server.Models
{
    public class ReportHubState
    {
        public bool Id { get; set; } = true;

        public bool WorkdayValue { get; set; }

        public TimeSpan BaseRepInterval { get; set; }
    }
}