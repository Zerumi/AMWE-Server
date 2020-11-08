using ReportHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AMWE_Administrator
{
    abstract class Notification
    {

    }

    class ReportNotification : Notification
    {
        public TextBlock NotifyBlock { get; set; }
        public int NotifyReportIndex { get; set; }
    }
}
