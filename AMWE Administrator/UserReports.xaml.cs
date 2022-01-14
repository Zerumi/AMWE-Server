// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using m3md2;
using Microsoft.AspNetCore.SignalR.Client;
using ReportHandler;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для UserReports.xaml
    /// </summary>
    public partial class UserReports : Window
    {
        public Client UserInWindow { get; set; }
        private readonly ClientState clientState;
        private readonly ReportLineChartDrawer ReportDrawer;
        private readonly List<Report> userReports;

        public UserReports(Client client, bool IsUserConnected)
        {
            try
            {
                InitializeComponent();

                Grid.Background = App.MainColor;
                gScreenViewer.Background = App.MainColor;
                chartCanvas.Background = App.MainColor;

                foreach (Label obj in WinHelper.FindVisualChildren<Label>(gScreenViewer))
                {
                    obj.Foreground = App.FontColor;
                }

                foreach (Label obj in WinHelper.FindVisualChildren<Label>(Grid))
                {
                    obj.Foreground = App.FontColor;
                }

                clientState = MainWindow.clientStates.Find(x => x.Client == client);

                UserInWindow = client;
                userReports = App.reports.FindAll(x => x.Client.Id == UserInWindow.Id); // long time parsing??

                double avgmark = 0;

                if (userReports.Count != 0)
                {
                    ReportDrawer = new(this, userReports);
                    avgmark = userReports.Select(x => x.OverallRating).Average();
                }

                lUserInfo.Content = $"ID {UserInWindow.Id} / {UserInWindow.Nameofpc}";
                lRepCount.Content = $"Количество отчетов: {userReports.Count}";
                lAvgMark.Content = $"Средняя оценка: {Math.Round(avgmark, 2)} ({Math.Round(avgmark, 5)})";

                if (IsUserConnected)
                {
                    DateTime a = default;
                    Task.Run(async () =>
                    {
                        a = (await ApiRequest.GetProductAsync<DateTime>("/time")).ToLocalTime();
                    }).Wait();
                    Timer timer = new()
                    {
                        Interval = TimeSpan.FromHours(1).TotalMilliseconds,
                        AutoReset = true
                    };
                    timer.Elapsed += UpdateTimestamp;
                    TimeSpan b = a.Subtract(clientState.LastLoginDateTime);
                    lOnlineStatus.Content = $"В сети уже {b.Hours} часов (с {clientState.LastLoginDateTime.ToLongTimeString()})"; // add connect & disconnect time (server utc only!)
                }
                else
                {
                    lUserInfo.Content += " (Не в сети)";
                    lOnlineStatus.Content = $"Был в сети с {clientState.LastLoginDateTime.ToLongTimeString()} по {clientState.LastLogoutDateTime.ToLongTimeString()}"; // add connect & disconnect time (server utc only!)
                    bScreen.IsEnabled = false;
                }

                foreach (Report report in userReports)
                {
                    int a = App.reports.IndexOf(report);
                    Button tempbutton = new()
                    {
                        Content = $"Отчет {a} ()" // report timestamp (server utc only!)
                    };

                    tempbutton.Click += Reportbutton_Click;

                    _ = spReports.Children.Add(tempbutton);
                }

                MainWindow.OnNewReport += MainWindow_OnNewReport;
                MainWindow.OnUserDisconnected += MainWindow_OnUserDisconnected;
                MainWindow.OnNewScreen += UpdateScreen;
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async void UpdateTimestamp(object sender, ElapsedEventArgs e)
        {
            try
            {
                TimeSpan a = e.SignalTime.Subtract(clientState.LastLoginDateTime);
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    lOnlineStatus.Content = $"В сети уже {a.Hours} часов (с {clientState.LastLoginDateTime.ToLongTimeString()})"; // add connect & disconnect time (server utc only!)
                }));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void UpdateScreen(Screen screen, Client client)
        {
            if (UserInWindow.Id == client.Id)
            {
                _ = Dispatcher.BeginInvoke(new Action(() =>
                {
                    iScreen.Source = LoadImage(screen.Bytes);
                }));
            }
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            BitmapImage image = new();
            using (MemoryStream mem = new(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
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
                        lOnlineStatus.Content = $"Был в сети с {clientState.LastLoginDateTime.ToLongTimeString()} по {clientState.LastLogoutDateTime.ToLongTimeString()}"; // add connect & disconnect time (server utc only!)
                        bScreen.IsEnabled = false;
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
            ReportWindow reportWindow = new(userReports[spReports.Children.IndexOf(e.Source as UIElement)]);
            reportWindow.Show();
        }

        private async void MainWindow_OnNewReport(Report obj)
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    userReports.Add(obj);
                    ReportDrawer.Values.Add(new Value(ReportDrawer.Values.Count, obj.OverallRating * 100));

                    int a = App.reports.IndexOf(obj);
                    Button tempbutton = new()
                    {
                        Content = $"Отчет {a} ()" // report timestamp (server utc only!)
                    };

                    tempbutton.Click += Reportbutton_Click;

                    _ = spReports.Children.Add(tempbutton);

                    lRepCount.Content = $"Количество отчетов: {userReports.Count}";

                    double avgmark = userReports.Select(x => x.OverallRating).Average(); // collection might be empty!
                    lAvgMark.Content = $"Средняя оценка: {Math.Round(avgmark, 2)} ({Math.Round(avgmark, 5)})";
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex);
                }
            }));
        }

        private async void BScreen_Click(object sender, RoutedEventArgs e)
        {
            await MainWindow.ScreenSystemConnection.InvokeAsync("RequestScreen", UserInWindow, ScreenType.ScreenImage);
        }
    }
}
