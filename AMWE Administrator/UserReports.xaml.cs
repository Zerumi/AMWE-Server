using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ReportHandler;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для UserReports.xaml
    /// </summary>
    public partial class UserReports : Window
    {
        public Client UserInWindow;
        ReportLineChartDrawer ReportDrawer;

        public UserReports(Client client)
        {
            InitializeComponent();

            UserInWindow = client;
            ReportDrawer = new(this);

            MainWindow.OnNewReport += MainWindow_OnNewReport;
        }

        private void MainWindow_OnNewReport(Report obj)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                ReportDrawer.values.Add(new Value(ReportDrawer.values.Count, obj.OverallRating * 100)); 
            }));
        }
    }
}
