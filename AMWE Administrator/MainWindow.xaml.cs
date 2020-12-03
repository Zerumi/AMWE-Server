﻿// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using m3md2;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using ReportHandler;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly List<Notification> notifications = new List<Notification>();
        bool isWorkdayStarted = false;

        readonly HubConnection ClientHandlerConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}listen/clients", options => {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.SkipNegotiation = true;
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        readonly HubConnection ReportHandleConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}report", options => {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.SkipNegotiation = true;
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        private readonly System.Windows.Forms.NotifyIcon notifyIcon1 = new System.Windows.Forms.NotifyIcon()
        {
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location)
        };

        public MainWindow()
        {
            try
            {
                #region Configure ClientListener Connection
                ClientHandlerConnection.ServerTimeout = TimeSpan.FromDays(2);
                ClientHandlerConnection.On<List<Client>>("GetAllClients", UpdateClients);
                ClientHandlerConnection.On<Client>("OnUserAuth", AddClient);
                ClientHandlerConnection.On<Client>("OnUserLeft", DeleteClient);
                ClientHandlerConnection.Closed += async (error) =>
                {
                    ExceptionHandler.RegisterNew(error);
                    await ClientHandlerConnection.StartAsync();
                };
                Task.Run(async () =>
                {
                    try
                    {
                        await ClientHandlerConnection.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.RegisterNew(ex);
                    }
                });
                #endregion

                #region Configure ReportHandler Connection
                ReportHandleConnection.ServerTimeout = TimeSpan.FromDays(2);
                ReportHandleConnection.On<Report>("CreateReport", CreateReport);
                ReportHandleConnection.On("SetWorkday", new Action<bool>(async(x) => {
                    await Task.Run(() => ChangeWorkdayValue(x));
                }));
                ReportHandleConnection.Closed += async (error) =>
                {
                    ExceptionHandler.RegisterNew(error);
                    await ReportHandleConnection.StartAsync();
                };
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        await ReportHandleConnection.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.RegisterNew(ex);
                    }
                });
                #endregion

                notifyIcon1.Visible = true;
                notifyIcon1.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
                notifyIcon1.ContextMenuStrip.Items.Add("Exit", null, ExitItem_Click);
                notifyIcon1.DoubleClick += NotifyIcon1_MouseDoubleClick;

                InitializeComponent();

                foreach(var obj in WinHelper.FindVisualChildren<Label>(Grid))
                {
                    obj.Foreground = App.FontColor;
                }

                Grid.Background = App.MainColor;

                WelcomeLabel.Content = $"{Parser.GetWelcomeLabel(Parser.GetTimeDescription(App.ServerDateTime))}, {App.Username}";
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, EventArgs e)
        {
            App.Current.Windows[0].Show();
            App.Current.Windows[0].Activate();
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private async void CreateReport(Report report)
        {
            try
            {
                if (report == null)
                {
                    // cancel this report
                    MessageBox.Show("(17.2) Получен пустой отчет");
                    return;
                }

                await Dispatcher.BeginInvoke((Action)(async() =>
                {
                    TextBlock textBlock = new TextBlock()
                    {
                        Text = $"({DateTime.Now.ToShortTimeString()}) ID {report.Client.Id}: Отправлен отчет ({report.OverallRating})",
                        Foreground = App.FontColor
                    };
                    textBlock.MouseEnter += Notification_GotMouseCapture;
                    textBlock.MouseLeave += Notification_LostMouseCapture;
                    textBlock.MouseDown += ReportNotification_MouseDown;

                    WinHelper.FindChild<TextBlock>(ClientList, $"ID{report.Client.Id}").Foreground = report.OverallRating > 0.5? App.RedColor : App.GreenColor;

                    App.reports.Add(report);

                    Notification notification = new ReportNotification()
                    {
                        NotifyBlock = textBlock,
                        NotifyReportIndex = App.reports.Count - 1
                    };

                    await Task.Run(() => AddNotification(notification));
                }));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

#nullable enable
        [STAThread]
        public async void AddNotification(Notification? notification)
        {
            try
            {
                notifications.Add(notification);
                await Dispatcher.BeginInvoke(new ThreadStart(() => spNotifications.Children.Add(notification?.NotifyBlock)));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        [STAThread]
        public async void RemoveNotification(Notification? notification)
        {
            try
            {
                notifications.Remove(notification);
                await Dispatcher.BeginInvoke(new ThreadStart(() => spNotifications.Children.Remove(notification?.NotifyBlock)));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }
#nullable disable

        [STAThread]
        private async void UpdateClients(List<Client> clients)
        {
            try
            {
                await Dispatcher.BeginInvoke(new Action(async() => { 
                    ClientList.Children.Clear();
                    foreach (var client in clients)
                    {
                        await ClientList.Dispatcher.BeginInvoke(new Action(() => AddClient(client)));
                    }
                }));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async void AddClient(Client client)
        {
            try
            {
                await Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlock temptextblock = new TextBlock()
                    {
                        Name = $"ID{client.Id}",
                        Text = $"ID {client.Id} - {client.Nameofpc}",
                        Foreground = App.FontColor
                    };
                    temptextblock.MouseEnter += TextBlock_GotMouseCapture;
                    ClientList.Children.Add(temptextblock);
                }));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void DeleteClient(Client client)
        {
            try
            {
                ClientList.Children.Remove(new TextBlock()
                {
                    Text = $"ID {client.Id} - {client.Nameofpc}",
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void TextBlock_GotMouseCapture(object sender, MouseEventArgs e)
        {
            try
            {
                (e.Source as UIElement).Visibility = Visibility.Collapsed;
                int id = ClientList.Children.IndexOf(e.Source as UIElement);
                Button button = new Button()
                {
                    Content = $"Управление {(e.Source as TextBlock).Text.Remove(0, id.ToString().Length + 6)}",
                    Margin = (e.Source as FrameworkElement).Margin
                };
                button.Click += ManageUser;
                button.MouseLeave += TextBlock_LostMouseCapture;
                ClientList.Children.Insert(id, button);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void ManageUser(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show((e.Source as Button).Content.ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void TextBlock_LostMouseCapture(object sender, MouseEventArgs e)
        {
            try
            {
                ClientList.Children[ClientList.Children.IndexOf(e.Source as UIElement) + 1].Visibility = Visibility.Visible;
                ClientList.Children.Remove(e.Source as UIElement);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void Notification_GotMouseCapture(object sender, MouseEventArgs e)
        {
            try
            {
                (e.Source as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 238));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void Notification_LostMouseCapture(object sender, MouseEventArgs e)
        {
            try
            {
                (e.Source as TextBlock).Foreground = App.FontColor;
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void ReportNotification_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ReportNotification notification = notifications.Find(x => x.NotifyBlock == e.Source as TextBlock) as ReportNotification;

                ReportWindow reportWindow = new ReportWindow(App.reports[notification.NotifyReportIndex])
                {
                    ShowActivated = true
                };
                reportWindow.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async void MenuDiagnostics_CheckConnectionState(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show($"Client Listener: {ClientHandlerConnection.State} [{await ClientHandlerConnection.InvokeAsync<string>("GetTransportType")}]\nReport Listener: {ReportHandleConnection.State} [{await ReportHandleConnection.InvokeAsync<string>("GetTransportType")}]\nBotNet System: -");
            }
            catch (Exception)
            {
                MessageBox.Show($"Client Listener: {ClientHandlerConnection.State}\nReport Listener: {ReportHandleConnection.State}\nBotNet System: -");
            }
        }

        private void MenuManage_Settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings settings = new Settings
                {
                    ShowActivated = true
                };
                settings.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void MenuManage_OpenAtExplorer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory);
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void MenuManage_ChangeServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите изменить сервер? (Программа будет перезагружена)", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    System.Windows.Forms.Application.Restart();

                    Environment.Exit(0);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void PrintInfo(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show($"Assistant in Monitoring the Work of Employees Administrator\nVersion 0.9.2020.0112\nAMWE RealTime server version 0.9.2020.3011\nMade by Zerumi (Discord: Zerumi#4666)");
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (bool.Parse(ConfigurationRequest.GetValueByKey("MinimizeToTray")))
                {
                    Array.ForEach(App.Current.Windows.OfType<Window>().ToArray(), (x) => x.Hide());
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void Main_Closed(object sender, EventArgs e)
        {
            try
            {
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        [STAThread]
        private async void ChangeWorkdayValue(bool x)
        {
            try
            {
                isWorkdayStarted = x;
                if (!isWorkdayStarted)
                {
                    TextBlock textBlock;
                    await Dispatcher.BeginInvoke((Action)(async() =>
                    {
                        textBlock = new TextBlock()
                        {
                            Text = "Всем клиентам сейчас сообщено что собирать и отправлять отчеты бесполезно и не нужно. Запустите рабочий день, чтобы начать сбор отчетов.",
                            Foreground = App.FontColor
                        };
                        textBlock.MouseEnter += Notification_GotMouseCapture;
                        textBlock.MouseLeave += Notification_LostMouseCapture;
                        textBlock.MouseDown += OnWorkdayNotifyTextBlock_MouseDown;
                        Notification notification = new TextActionNotification()
                        {
                            Name = "WorkdayNotStartedOnConnected",
                            NotifyBlock = textBlock
                        };

                        await Task.Run(() => AddNotification(notification));
                    }));
                }
                else
                {
                    await Task.Run(() => RemoveNotification(notifications.Find(x => x.Name == "WorkdayNotStartedOnConnected")));
                }
                UpdateMenuHeader();
                await Dispatcher.BeginInvoke((Action)(() => mWorkday.IsEnabled = true));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async void OnWorkdayNotifyTextBlock_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                await ReportHandleConnection.InvokeAsync("SetWorkdayValue", true);
                UpdateMenuHeader();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async void MWorkday_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ReportHandleConnection.InvokeAsync("SetWorkdayValue", !isWorkdayStarted);
                UpdateMenuHeader();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async void UpdateMenuHeader()
        {
            try
            {
                await Dispatcher.BeginInvoke((Action)(() => mWorkday.Header = isWorkdayStarted ? "Остановить рабочий день" : "Начать рабочий день"));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex); 
            }
        }
    }
}