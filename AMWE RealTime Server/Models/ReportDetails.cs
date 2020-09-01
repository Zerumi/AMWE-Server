using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Models
{
    public class ReportDetails
    {
        public KeyboardDetails KeyboardDetails { get; set; }
        public ProcessesDetails ProcessesDetails { get; set; }
        public MouseDetails MouseDetails { get; set; }
    }
}