// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
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
