using ReportHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AMWE_Administrator
{
    public abstract class Notification
    {
        public string Name { get; set; }
        public TextBlock NotifyBlock { get; set; }
    }

    public class ReportNotification : Notification
    {
        public int NotifyReportIndex { get; set; }
    }

    public class TextActionNotification : Notification
    {
        
    }
}
