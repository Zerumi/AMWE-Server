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
        // signalr hub to get screen

        public Client UserInWindow;
        ReportLineChartDrawer ReportDrawer;
        List<Report> userReports;
        bool IsUserConnected;

        public UserReports(Client client, bool IsUserConnected)
        {
            try
            {
                InitializeComponent();

                this.IsUserConnected = IsUserConnected;
                UserInWindow = client;
                userReports = App.reports.FindAll(x => x.Client.Id == UserInWindow.Id); // long time parsing??
                ReportDrawer = new(this, userReports);
                double avgmark = userReports.Select(x => x.OverallRating).Average();

                lUserInfo.Content = $"ID {UserInWindow.Id} / {UserInWindow.Nameofpc}";
                lRepCount.Content = $"Количество отчетов: {userReports.Count}";
                lAvgMark.Content = $"Средняя оценка: {Math.Round(avgmark, 2)} ({Math.Round(avgmark, 5)})";

                if (IsUserConnected)
                {
                    lOnlineStatus.Content = "В сети уже {} часов (с {})"; // calculate from server time
                }
                else
                {
                    lUserInfo.Content += " (Не в сети)";
                    lOnlineStatus.Content = "Был в сети с {} по {}"; // add connect & disconnect time (server utc only!)
                }

                foreach (var report in userReports)
                {
                    var a = App.reports.IndexOf(report);
                    Button tempbutton = new Button()
                    {
                        Content = $"Отчет {a} ()" // report timestamp (server utc only!)
                    };

                    tempbutton.Click += Reportbutton_Click;

                    spReports.Children.Add(tempbutton);
                }

                MainWindow.OnNewReport += MainWindow_OnNewReport;
                MainWindow.OnUserDisconnected += MainWindow_OnUserDisconnected;
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async void MainWindow_OnUserDisconnected(Client obj)
        {
            try
            {
                if (obj.Id == UserInWindow.Id)
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        lUserInfo.Content += " (Не в сети)";
                        lOnlineStatus.Content = "Был в сети с {} по {}"; // add connect & disconnect time (server utc only!)
                }));
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void Reportbutton_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow reportWindow = new ReportWindow(userReports[spReports.Children.IndexOf(e.Source as UIElement)]);
            reportWindow.Show();
        }

        private async void MainWindow_OnNewReport(Report obj)
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    userReports.Add(obj);
                    ReportDrawer.values.Add(new Value(ReportDrawer.values.Count, obj.OverallRating * 100));

                    var a = App.reports.IndexOf(obj);
                    Button tempbutton = new Button()
                    {
                        Content = $"Отчет {a} ()" // report timestamp (server utc only!)
                    };

                    tempbutton.Click += Reportbutton_Click;

                    spReports.Children.Add(tempbutton);

                    lUserInfo.Content = $"ID {UserInWindow.Id} / {UserInWindow.Nameofpc}";
                    lRepCount.Content = $"Количество отчетов: {userReports.Count}";

                    double avgmark = userReports.Select(x => x.OverallRating).Average();
                    lAvgMark.Content = $"Средняя оценка: {Math.Round(avgmark, 2)} ({Math.Round(avgmark, 5)})";

                    if (IsUserConnected)
                    {
                        lOnlineStatus.Content = "В сети уже {} часов (с {})"; // calculate from server time
                    }
                    else
                    {
                        lUserInfo.Content += " (Не в сети)";
                        lOnlineStatus.Content = "Был в сети с {} по {}"; // add connect & disconnect time (server utc only!)
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex);
                }
            }));
        }

        private void bScreen_Click(object sender, RoutedEventArgs e)
        {
            // request screen from hub
        }
    }
}
