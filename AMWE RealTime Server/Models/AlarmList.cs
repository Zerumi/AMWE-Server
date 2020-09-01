using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMWE_RealTime_Server.Models
{
    public class AlarmList
    {
        public decimal OverallRating { get; set; }
        public decimal MouseRating { get; set; }
        public decimal KeyboardRating { get; set; }
        public decimal ProcessRating { get; set; }
    }
}