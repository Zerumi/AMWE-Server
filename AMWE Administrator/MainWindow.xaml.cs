// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
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
    public partial class MainWindow : Window, IDisposable
    {
        public static readonly List<ClientState> clientStates = new();
        private static readonly List<Client> сurrentclients = new();
        private static readonly List<Notification> notifications = new();
        private static readonly List<Client> allclients = new();
        private static readonly List<Chat> chats = new();
        private static readonly Stopwatch LastConnectStopwatch = new();
        private static readonly List<TextBlock> tbpClientList = new();

        private TimeSpan ReportPollingInterval;
        private bool isWorkdayStarted;

        public static readonly HubConnection ClientHandlerConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}listen/clients", options =>
        {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        public static readonly HubConnection ReportHandleConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}report", options =>
        {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        public static readonly HubConnection ChatSystemConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}chat", options =>
        {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        public static readonly HubConnection ScreenSystemConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}screen", options =>
        {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        public static readonly HubConnection AdminSystemConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}admin", options =>
        {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        private static readonly System.Windows.Forms.NotifyIcon notifyIcon1 = new()
        {
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location)
        };

        public static event Action<Screen, Client> OnNewImage;

        public MainWindow()
        {
            try
            {
                #region Configure ClientListener Connection
                ClientHandlerConnection.ServerTimeout = TimeSpan.FromDays(2);
                _ = ClientHandlerConnection.On<List<ClientState>>("GetAllClients", UpdateClients);
                _ = ClientHandlerConnection.On<ClientState>("OnUserAuth", AddClient);
                _ = ClientHandlerConnection.On<ClientState>("OnUserLeft", DeleteClient);
                _ = ClientHandlerConnection.On<uint>("EnhanceControlForUser", EnhanceClientControl);
                _ = ClientHandlerConnection.On<uint>("LoosenControlForUser", LoosenClientControl);
                ClientHandlerConnection.Closed += async (error) =>
                {
                    ExceptionHandler.RegisterNew(error, false);
                    try
                    {
                        await ClientHandlerConnection.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.RegisterNew(ex);
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                            mConnect.IsEnabled = true;
                        }));
                    }
                };
                _ = Task.Run(async () =>
                  {
                      try
                      {
                          LastConnectStopwatch.Start();
                          await ClientHandlerConnection.StartAsync();
                      }
                      catch (Exception ex)
                      {
                          ExceptionHandler.RegisterNew(ex);
                          await Dispatcher.BeginInvoke(new Action(() =>
                          {
                              mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                              mConnect.IsEnabled = true;
                          }));
                      }
                  });
                #endregion

                #region Configure ReportHandler Connection
                ReportHandleConnection.ServerTimeout = TimeSpan.FromDays(2);
                _ = ReportHandleConnection.On<Report>("CreateReport", CreateReport);
                _ = ReportHandleConnection.On("SetWorkday", new Action<bool>(async (x) =>
                {
                    await Task.Run(() => ChangeWorkdayValue(x));
                }));
                _ = ReportHandleConnection.On<TimeSpan>("SetBaseSendingTime", ChangeReportPollingTime);
                ReportHandleConnection.Closed += async (error) =>
                {
                    ExceptionHandler.RegisterNew(error, false);
                    try
                    {
                        await ReportHandleConnection.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.RegisterNew(ex);
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                            mConnect.IsEnabled = true;
                        }));
                    }
                };
                _ = Task.Run(async () =>
                  {
                      try
                      {
                          await ReportHandleConnection.StartAsync();
                      }
                      catch (Exception ex)
                      {
                          ExceptionHandler.RegisterNew(ex);
                          await Dispatcher.BeginInvoke(new Action(() =>
                          {
                              mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                              mConnect.IsEnabled = true;
                          }));
                      }
                  });
                #endregion

                #region Configure ChatSystem Connection
                ChatSystemConnection.ServerTimeout = TimeSpan.FromDays(2);
                _ = ChatSystemConnection.On<uint, string, string, DateTime>("ReceiveMessage", RecieveMessage);
                _ = ChatSystemConnection.On<uint>("AcceptChatID", AcceptChat);
                _ = ChatSystemConnection.On<uint>("CloseDeleteChat", DeleteChat);
                ChatSystemConnection.Closed += async (error) =>
                {
                    ExceptionHandler.RegisterNew(error, false);
                    try
                    {
                        await ChatSystemConnection.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.RegisterNew(ex);
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                            mConnect.IsEnabled = true;
                        }));
                    }
                };
                _ = Task.Run(async () =>
                  {
                      try
                      {
                          await ChatSystemConnection.StartAsync();
                      }
                      catch (Exception ex)
                      {
                          ExceptionHandler.RegisterNew(ex);
                          await Dispatcher.BeginInvoke(new Action(() =>
                          {
                              mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                              mConnect.IsEnabled = true;
                          }));
                      }
                  });
                #endregion

                #region Configure ScreenSystem Connection
                ScreenSystemConnection.ServerTimeout = TimeSpan.FromDays(2);
                _ = ScreenSystemConnection.On<Screen, Client, ScreenType>("NewScreen", RmNewScreen);
                ScreenSystemConnection.Closed += async (error) =>
                {
                    ExceptionHandler.RegisterNew(error, false);
                    try
                    {
                        await ScreenSystemConnection.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.RegisterNew(ex);
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                            mConnect.IsEnabled = true;
                        }));
                    }
                };
                _ = Task.Run(async () =>
                  {
                      try
                      {
                          await ScreenSystemConnection.StartAsync();
                      }
                      catch (Exception ex)
                      {
                          ExceptionHandler.RegisterNew(ex);
                          await Dispatcher.BeginInvoke(new Action(() =>
                          {
                              mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                              mConnect.IsEnabled = true;
                          }));
                      }
                  });
                #endregion

                #region Configure AdminSystem Connection
                AdminSystemConnection.ServerTimeout = TimeSpan.FromDays(2);
                _ = AdminSystemConnection.On<string>("Log", ServerLogInfo);
                AdminSystemConnection.Closed += async (error) =>
                {
                    ExceptionHandler.RegisterNew(error, false);
                    try
                    {
                        await AdminSystemConnection.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.RegisterNew(ex);
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                            mConnect.IsEnabled = true;
                        }));
                    }
                };
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await AdminSystemConnection.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.RegisterNew(ex);
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                            mConnect.IsEnabled = true;
                        }));
                    }
                });
                #endregion

                _ = Dispatcher.BeginInvoke(new Action(() =>
                  {
                      notifyIcon1.Visible = true;
                      notifyIcon1.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
                      _ = notifyIcon1.ContextMenuStrip.Items.Add("Exit", null, ExitItem_Click);
                      notifyIcon1.DoubleClick += NotifyIcon1_MouseDoubleClick;
                  }));

                InitializeComponent();

                mConnect.Header = $"Подключено к {App.ServerAddress}";

                foreach (Label obj in WinHelper.FindVisualChildren<Label>(Grid))
                {
                    obj.Foreground = App.FontColor;
                }

                Grid.Background = App.MainColor;

                Resources["ButtonSelectedBrush"] = App.ButtonHighlightColor;

                mwMenu.Background = App.ControlColor;
                mwMenu.Foreground = App.FontColor;
                foreach (MenuItem mitem in mwMenu.Items.OfType<MenuItem>().SelectMany(item => item.Items.OfType<MenuItem>()))
                {
                    mitem.Foreground = SystemColors.ActiveCaptionTextBrush;
                }

                WelcomeLabel.Content = $"{Parser.GetWelcomeLabel(Parser.GetTimeDescription(App.ServerDateTime))}, {App.Username}";
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async void ChangeReportPollingTime(TimeSpan interval)
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                ReportPollingInterval = interval;
                mReportPolling.Header = $"Опрос отчетов происходит {Parser.GetDeclension(Convert.ToInt32(interval.TotalMinutes), "каждую", "каждые", "каждые")} {interval.TotalMinutes} {Parser.GetDeclension(Convert.ToInt32(interval.TotalMinutes), "минуту", "минуты", "минут")}";
            }));
        }

        private async void ServerLogInfo(string message)
        {
            await Dispatcher.BeginInvoke(new Action(async () =>
            {
                TextBlock textBlock = new()
                {
                    Text = $"({DateTime.Now.ToShortTimeString()}) (Server): {message}",
                    Foreground = App.FontColor
                };
                textBlock.MouseEnter += Notification_GotMouseCapture;
                textBlock.MouseLeave += Notification_LostMouseCapture;
                Notification notification = new LogNotification()
                {
                    Name = $"AdmLog{DateTime.Now.Ticks}",
                    NotifyBlock = textBlock
                };
                await Task.Run(() => AddNotification(notification));
            }));
        }

        private void EnhanceClientControl(uint clientId)
        {
            var a = clientStates.Find(x => x.Client.Id == clientId);
            a.IsEnhanced = true;
        }

        private void LoosenClientControl(uint clientId)
        {
            var a = clientStates.Find(x => x.Client.Id == clientId);
            a.IsEnhanced = false;
        }

        private async void DeleteChat(uint id)
        {
            Chat chat = chats.Find(x => x.ChatID == id);
            await Dispatcher.BeginInvoke((Action)(() => { chat.ChatClosed = true; chat.Close(); }));
            _ = MessageBox.Show($"Чат {id} был закрыт и удален, так как одна из сторон закрыла соединение. Вы можете заново открыть чат (пользователь: ID {chat.Client.Id} / {chat.Client.Nameofpc})");
            _ = chats.Remove(chat);
        }

        private void RecieveMessage(uint id, string message, string user, DateTime timestamp)
        {
            Chat chat = chats.Find(x => x.ChatID == id);
            _ = chat.Dispatcher.BeginInvoke((Action)(() => chat.AddMessage(timestamp, user, message)));
        }

        private void AcceptChat(uint id)
        {
            Chat chat = chats.Find(x => x.ChatID == id);
            _ = chat.Dispatcher.BeginInvoke((Action)(() =>
            {
                chat.Show();
                _ = chat.Activate();
            }));
            RemoveNotification(notifications.Find(x => x.Name == $"ChatWait{chat.Client.Id}"));
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, EventArgs e)
        {
            App.Current.Windows[0].Show();
            _ = App.Current.Windows[0].Activate();
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Dispose();
            Environment.Exit(0);
        }

        public static event Action<Report> OnNewReport;

        private async void CreateReport(Report report)
        {
            try
            {
                if (report == null)
                {
                    // cancel this report
                    _ = MessageBox.Show("(17.2) Получен пустой отчет. Мы даже не знаем, от кого он :/\nВозможно на API совершена атака.");
                    return;
                }

                if (App.CheckReports && !clientStates.Find(x => x.Client.Id == report.Client.Id).IgnoreWarning)
                {
                    if (App.CheckApps)
                    {
                        report.ProcIntersection = App.AppsToCheck.Select(x => { return x.IsEnabled ? x.Transcription : string.Empty; }).Intersect(report?.LastProcesses?.ToList().FindAll(x => x != string.Empty) ?? new List<string>()).Select(f => App.AppsToCheck.Find(z => z.Transcription == f)).ToList();
                    }
                    if (App.CheckSites)
                    {
                        report.SiteIntersection = App.SitesToCheck.Select(x => { return x.IsEnabled ? x.Transcription : string.Empty; }).Intersect(report?.CurrentSites?.Select(x => x.SiteUri?.DnsSafeHost ?? string.Empty) ?? new List<string>()).Select(f => App.SitesToCheck.Find(z => z.Transcription == f)).ToArray();
                    }
                }

                await Dispatcher.BeginInvoke((Action)(async () =>
                {
                    bool flag = false;

                    string message = string.Empty;

                    if ((report.ProcIntersection?.Count ?? 0) != 0)
                    {
                        message = $"({DateTime.Now.ToShortTimeString()}) / ! \\ ID {report.Client.Id}: Обнаружена запрещенная программа ({report.OverallRating})";
                        flag = true;
                    }
                    else if ((report.SiteIntersection?.Count ?? 0) != 0)
                    {
                        message = $"({DateTime.Now.ToShortTimeString()}) / ! \\ ID {report.Client.Id}: Обнаружен запрещенный сайт ({report.OverallRating})";
                        flag = true;
                    }
                    else
                    {
                        message = $"({DateTime.Now.ToShortTimeString()}) ID {report.Client.Id}: Отправлен отчет ({report.OverallRating})";
                    }
                    TextBlock textBlock = new()
                    {
                        Text = message,
                        Foreground = App.FontColor
                    };
                    textBlock.MouseEnter += Notification_GotMouseCapture;
                    textBlock.MouseLeave += Notification_LostMouseCapture;
                    textBlock.MouseDown += ReportNotification_MouseDown;

                    TextBlock tb = WinHelper.FindChild<TextBlock>(ClientList, $"ID{report.Client.Id}");
                    if (tb != null)
                    {
                        if (flag)
                        {
                            tb.Foreground = App.RedColor;
                        }
                        else
                        {
                            tb.Foreground = report.OverallRating > 0.5 ? App.RedColor : App.GreenColor;
                        }
                    }
                    else
                    {
                        _ = MessageBox.Show("(17.3) Получен отчет от удаленного из сети пользователя.\nВозможно на API совершена атака.");
                    }

                    App.reports.Add(report);

                    Notification notification = new ReportNotification()
                    {
                        NotifyBlock = textBlock,
                        NotifyReportIndex = App.reports.Count - 1
                    };

                    await Task.Run(() => AddNotification(notification));
                }));
                OnNewReport?.Invoke(report);
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
                _ = notifications.Remove(notification);
                await Dispatcher.BeginInvoke(new ThreadStart(() => spNotifications.Children.Remove(notification?.NotifyBlock)));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }
#nullable disable

        [STAThread]
        private async void UpdateClients(List<ClientState> _clientStates)
        {
            try
            {
                LastConnectStopwatch.Stop();
                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    mLastConnectTime.Header = $"Последнее подключение длилось {LastConnectStopwatch.ElapsedMilliseconds} мс";
                    ClientList.Children.Clear();
                    object temp = mReports.Items[0];
                    mReports.Items.Clear();
                    _ = mReports.Items.Add(temp);
                    сurrentclients.Clear();
                    allclients.Clear();
                    lClientList.Content = $"Список пользователей (0):";
                    foreach (ClientState client in _clientStates)
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

        private async void AddClient(ClientState clientState)
        {
            try
            {
                await Dispatcher.BeginInvoke((Action)(() =>
                {
                    Client client = clientState.Client;
                    clientStates.Add(clientState);
                    allclients.Add(client);
                    TextBlock temptextblock = new()
                    {
                        Name = $"ID{client.Id}",
                        Text = $"ID {client.Id} - {client.Nameofpc}",
                        Foreground = App.FontColor
                    };
                    if (clientState.IsOnline)
                    {
                        temptextblock.MouseEnter += TextBlock_GotMouseCapture;
                        сurrentclients.Add(client);
                        lClientList.Content = $"Список пользователей ({сurrentclients.Count}):";
                        _ = ClientList.Children.Add(temptextblock);
                        tbpClientList.Add(temptextblock);
                    }
                    MenuItem tempmenuitem = new()
                    {
                        Name = temptextblock.Name,
                        Header = temptextblock.Text,
                        Foreground = SystemColors.ActiveCaptionTextBrush
                    };
                    tempmenuitem.Click += RmUniversalUser_Click;
                    _ = mReports.Items.Add(tempmenuitem); // save reports on server
                    // get reports from server, save reports on HDD in spec folder, new *.amwereport
                }));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        public static event Action<Client> OnUserDisconnected;

        private async void DeleteClient(ClientState clientState)
        {
            try
            {
                Client client = clientState.Client;
                if (client != null)
                {
                    ClientState oldState = clientStates.Find(x => x.Client.Id == client.Id);
                    oldState.IsOnline = false;
                    oldState.LastLogoutDateTime = clientState.LastLogoutDateTime;
                    OnUserDisconnected?.Invoke(client);
                    await Dispatcher.BeginInvoke((Action)(() =>
                    {
                        Client a = сurrentclients.Find(x => x.Id == client.Id);
                        _ = сurrentclients.Remove(a);
                        lClientList.Content = $"Список пользователей ({сurrentclients.Count}):";
                        TextBlock temptextblock = tbpClientList.Find(x => x.Text == $"ID {client.Id} - {client.Nameofpc}");
                        ClientList.Children.Remove(temptextblock);
                        _ = tbpClientList.Remove(temptextblock);
                    }));
                }
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
                int eid = ClientList.Children.IndexOf(e.Source as UIElement);
                int id = Convert.ToInt32((e.Source as TextBlock).Text.GetUntilOrEmpty(" -")[3..]);
                Button button = new()
                {
                    Content = $"({id}) Начать чат",
                    Margin = (e.Source as FrameworkElement).Margin,
                    Background = App.ButtonColor,
                    Foreground = App.FontColor
                };
                button.Click += ManageUser;
                button.MouseLeave += TextBlock_LostMouseCapture;
                ClientList.Children.Insert(eid, button);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async void ManageUser(object sender, RoutedEventArgs e)
        {
            try
            {
                uint id = uint.Parse((e.Source as Button).Content.ToString().GetUntilOrEmpty(")").Remove(0, 1));
                await OpenChat(id);
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        public async Task OpenChat(uint id)
        {
            Client client = сurrentclients.Find(x => x.Id == id);
            await Dispatcher.BeginInvoke((Action)(async () =>
            {
                try
                {
                    Chat chat = new(ChatSystemConnection, await ChatSystemConnection.InvokeAsync<uint>("OpenChat", id), client);
                    chats.Add(chat);
                    TextBlock textBlock = new()
                    {
                        Text = $"({DateTime.Now.ToShortTimeString()}) Мы ожидаем ответа на открытие чата от {id} / {client.Nameofpc}",
                        Foreground = App.FontColor
                    };
                    textBlock.MouseEnter += Notification_GotMouseCapture;
                    textBlock.MouseLeave += Notification_LostMouseCapture;
                    Notification notification = new LogNotification()
                    {
                        Name = $"ChatWait{id}",
                        NotifyBlock = textBlock
                    };
                    AddNotification(notification);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex);
                }
            }));
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
                ReportNotification notification = notifications.Find(x => x.NotifyBlock == (e.Source as TextBlock)) as ReportNotification;

                _ = Dispatcher.BeginInvoke(new Action(delegate
                {
                    ReportWindow reportWindow = new(App.reports[notification.NotifyReportIndex]);
                    reportWindow.Show();
                    _ = reportWindow.Activate();
                }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);

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
                _ = MessageBox.Show($"Admin Logging System: {AdminSystemConnection.State} [{await AdminSystemConnection.InvokeAsync<string>("GetTransportType")}]\nClient Listener: {ClientHandlerConnection.State} [{await ClientHandlerConnection.InvokeAsync<string>("GetTransportType")}]\nReport Listener: {ReportHandleConnection.State} [{await ReportHandleConnection.InvokeAsync<string>("GetTransportType")}]\nChat System: {ChatSystemConnection.State} [{await ChatSystemConnection.InvokeAsync<string>("GetTransportType")}]\nScreen Transfer: {ScreenSystemConnection.State} [{await ScreenSystemConnection.InvokeAsync<string>("GetTransportType")}]");
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex, false);
                _ = MessageBox.Show($"Admin Logging System: {AdminSystemConnection.State}\nClient Listener: {ClientHandlerConnection.State}\nReport Listener: {ReportHandleConnection.State}\nChat System: {ChatSystemConnection.State}\n Screen Transfer: {ScreenSystemConnection.State}");
            }
        }

        private void MenuManage_Settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings settings = new();
                settings.Show();
                _ = settings.Activate();
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
                _ = Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory); // Windows integration?
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
                    notifyIcon1.Dispose();

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
                _ = MessageBox.Show($"Assistant in Monitoring the Work of Employees Administrator\nVersion 1.5.2022.2102\nAMWE RealTime server version 1.4.2022.2102\nMade by Zerumi (Discord: Zerumi#4666)\nGitHub: https://github.com/Zerumi");
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
                    Array.ForEach(Application.Current.Windows.OfType<Window>().ToArray(), (x) => x.Hide());
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
                notifyIcon1.Dispose();
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
                    await Dispatcher.BeginInvoke((Action)(async () =>
                    {
                        textBlock = new TextBlock()
                        {
                            Text = $"({DateTime.Now.ToShortTimeString()}) Всем клиентам сейчас сообщено что собирать и отправлять отчеты не нужно. Запустите рабочий день, чтобы начать сбор отчетов.",
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

        private async void MConnect_Click(object sender, RoutedEventArgs e)
        {
            mDignose.GotFocus -= Diagnose_Click;
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                mConnect.Header = $"Переподключаемся к {App.ServerAddress}";
                mConnect.IsEnabled = false;
            }));
            if (ClientHandlerConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    LastConnectStopwatch.Restart();
                    await ClientHandlerConnection.StartAsync();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex);
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                        mConnect.IsEnabled = true;
                    }));
                    return;
                }
            }
            if (ReportHandleConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await ReportHandleConnection.StartAsync();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex);
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                        mConnect.IsEnabled = true;
                    }));
                    return;
                }
            }
            if (ChatSystemConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await ChatSystemConnection.StartAsync();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex);
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                        mConnect.IsEnabled = true;
                    }));
                    return;
                }
            }
            if (ScreenSystemConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await ScreenSystemConnection.StartAsync();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex);
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                        mConnect.IsEnabled = true;
                    }));
                    return;
                }
            }
            if (AdminSystemConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await AdminSystemConnection.StartAsync();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex);
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                        mConnect.IsEnabled = true;
                    }));
                    return;
                }
            }
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                mConnect.Header = $"Подключено к {App.ServerAddress}";
                mConnect.IsEnabled = false;
            }));
            mDignose.GotFocus += Diagnose_Click;
        }

        private void GetExceptions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DiagnosticExceptionWindow diagnostic1 = new();
                diagnostic1.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void DiagnoseServer_Click(object sender, RoutedEventArgs e)
        {
            DiagnoseServer wserver = new();
            wserver.Show();
            _ = wserver.Activate();
        }

        private async void Diagnose_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ClientHandlerConnection.State == HubConnectionState.Disconnected || ReportHandleConnection.State == HubConnectionState.Disconnected || ChatSystemConnection.State == HubConnectionState.Disconnected)
                {
                    mConnect.Header = $"Отключено от {App.ServerAddress}. Нажмите для переподключения.";
                    mConnect.IsEnabled = true;
                }
            }));
        }

        private void GoToAMWESite(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo ps = new("https://amwe.glitch.me/")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            _ = Process.Start(ps);
        }

        private void RmUniversalUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                uint id = uint.Parse((e.Source as FrameworkElement).Name.Remove(0, 2));
                Client client = allclients.Find(x => x.Id == id);

                UserReports userReports = new(client);
                userReports.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void RmNewScreen(Screen screen, Client client, ScreenType type)
        {
            OnNewImage?.Invoke(screen, client);
        }

        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                notifyIcon1.Dispose();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void mReportPolling_Click(object sender, RoutedEventArgs e)
        {
            TimeSetter timeSetter = new TimeSetter(ReportPollingInterval);
            timeSetter.Show();
        }

        private async void MenuDiagnostic_GetServerInfo(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(await AdminSystemConnection.InvokeAsync<string>("GetServerInfo"));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}