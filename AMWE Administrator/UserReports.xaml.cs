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
        private readonly Timer timer;

        public UserReports(Client client)
        {
            try
            {
                InitializeComponent();

                Grid.Background = App.MainColor;
                gScreenViewer.Background = App.MainColor;
                chartCanvas.Background = App.MainColor;

                Resources["ButtonSelectedBrush"] = App.ButtonHighlightColor;

                foreach (Label obj in WinHelper.FindVisualChildren<Label>(gScreenViewer))
                {
                    obj.Foreground = App.FontColor;
                }

                foreach (Label obj in WinHelper.FindVisualChildren<Label>(Grid))
                {
                    obj.Foreground = App.FontColor;
                }

                foreach (Button obj in WinHelper.FindVisualChildren<Button>(gScreenViewer))
                {
                    obj.Background = App.ButtonColor;
                    obj.Foreground = App.FontColor;
                }

                foreach (Button obj in WinHelper.FindVisualChildren<Button>(Grid))
                {
                    obj.Background = App.ButtonColor;
                    obj.Foreground = App.FontColor;
                }

                clientState = MainWindow.clientStates.Find(x => x.Client.Id == client.Id);

                UserInWindow = client;
                userReports = App.reports.FindAll(x => x.Client.Id == UserInWindow.Id); // long time parsing??

                double avgmark = 0;

                ReportDrawer = new(this, userReports);

                if (userReports.Count != 0)
                {
                    avgmark = userReports.Select(x => x.OverallRating).Average();
                }

                Title = $"Сводка по сотруднику {UserInWindow.Nameofpc} (ID: {UserInWindow.Id})";
                lUserInfo.Content = $"ID {UserInWindow.Id} / {UserInWindow.Nameofpc}";
                lRepCount.Content = $"Количество отчетов: {userReports.Count}";
                lAvgMark.Content = $"Средняя оценка: {Math.Round(avgmark, 2)} ({Math.Round(avgmark, 5)})";

                if (clientState.IsOnline)
                {
                    DateTime a = default;
                    Task.Run(async () =>
                    {
                        a = (await ApiRequest.GetProductAsync<DateTime>("/time")).ToLocalTime();
                    }).Wait();
                    timer = new()
                    {
                        Interval = TimeSpan.FromMinutes(clientState.LastLoginDateTime.AddHours(a.Subtract(clientState.LastLoginDateTime).Hours + 1).Subtract(a).Minutes + 1).TotalMilliseconds,
                        AutoReset = true
                    };
                    timer.Elapsed += UpdateTimestamp;
                    timer.Start();
                    TimeSpan b = a.Subtract(clientState.LastLoginDateTime);
                    lOnlineStatus.Content = $"В сети уже {b.Hours} {Parser.GetDeclension(b.Hours, "час", "часа", "часов")} (с {clientState.LastLoginDateTime.ToLongTimeString()})"; // add connect & disconnect time (server utc only!)
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
                        Content = $"Отчет {a} ({report.Timestamp.ToLocalTime().ToLongTimeString()}) ({report.OverallRating})", // report timestamp (server utc only!)
                        Foreground = App.FontColor,
                        Background = App.ButtonColor
                    };

                    tempbutton.Click += Reportbutton_Click;

                    _ = spReports.Children.Add(tempbutton);
                }

                MainWindow.OnNewReport += MainWindow_OnNewReport;
                MainWindow.OnUserDisconnected += MainWindow_OnUserDisconnected;
                MainWindow.OnNewImage += UpdateScreen;
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
                    lOnlineStatus.Content = $"В сети уже {a.Hours} {Parser.GetDeclension(a.Hours, "час", "часа", "часов")} (с {clientState.LastLoginDateTime.ToLongTimeString()})"; // add connect & disconnect time (server utc only!)
                }));
                (sender as Timer).Interval = TimeSpan.FromMinutes(clientState.LastLoginDateTime.AddHours(e.SignalTime.Subtract(clientState.LastLoginDateTime).Hours + 1).Subtract(e.SignalTime).Minutes + 1).TotalMilliseconds;
                // Calculate next hour: 
                // window opened: 01/01/1970 09:47:22
                // client logged: 01/01/1970 09:10:00
                // next hour: 01/01/1970 10:10:00
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
                    bSaveScr.IsEnabled = true;
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
                        timer?.Stop();
                        lUserInfo.Content += " (Не в сети)";
                        lOnlineStatus.Content = $"Был в сети с {clientState.LastLoginDateTime.ToLongTimeString()} по {clientState.LastLogoutDateTime.ToLongTimeString()}";
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
            if (obj.Client.Id == UserInWindow.Id)
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
                            Content = $"Отчет {a} ({obj.Timestamp.ToLocalTime().ToLongTimeString()}) ({obj.OverallRating})", // report timestamp (server utc only!)
                        Foreground = App.FontColor,
                            Background = App.ButtonColor
                        };

                        tempbutton.Click += Reportbutton_Click;

                        _ = spReports.Children.Add(tempbutton);

                        lRepCount.Content = $"Количество отчетов: {userReports.Count}";

                        double avgmark = userReports.Select(x => x.OverallRating).Average();
                        lAvgMark.Content = $"Средняя оценка: {Math.Round(avgmark, 2)} ({Math.Round(avgmark, 5)})";
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.RegisterNew(ex);
                    }
                }));
            }
        }

        private async void BScreen_Click(object sender, RoutedEventArgs e)
        {
            await MainWindow.ScreenSystemConnection.InvokeAsync("RequestScreen", UserInWindow, ScreenType.ScreenImage);
        }

        private async void BWebCam_Click(object sender, RoutedEventArgs e)
        {
            await MainWindow.ScreenSystemConnection.InvokeAsync("RequestScreen", UserInWindow, ScreenType.WebcamImage);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.OnNewReport -= MainWindow_OnNewReport;
            MainWindow.OnUserDisconnected -= MainWindow_OnUserDisconnected;
            MainWindow.OnNewImage -= UpdateScreen;
            GC.Collect();
        }

        private void bSaveScr_Click(object sender, RoutedEventArgs e)
        {
            if (iScreen.Source == null)
            {
                _ = MessageBox.Show("(19.1) Отсутсвует изображение для сохранения");
                return;
            }
        linkcreatepng:
            try
            {
                PngBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)iScreen.Source));
                using FileStream stream = new($"{Directory.GetCurrentDirectory()}\\Screens\\Screen-{UserInWindow.Nameofpc}-{DateTime.Now.Ticks}.png", FileMode.Create);
                encoder.Save(stream);
                bSaveScr.IsEnabled = false;
            }
            catch (DirectoryNotFoundException)
            {
                _ = Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\Screens");
                goto linkcreatepng;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void LastReport(object sender, RoutedEventArgs e)
        {
            ReportWindow reportWindow = new ReportWindow(userReports?.Last());
            reportWindow?.Show();
        }

        private void Ignore(object sender, RoutedEventArgs e)
        {

        }

        private void Warnings(object sender, RoutedEventArgs e)
        {

        }
    }
}
