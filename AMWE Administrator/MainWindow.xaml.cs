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
        public static readonly List<ClientState> clientStates = new List<ClientState>();
        static readonly List<Notification> notifications = new List<Notification>();
        static readonly List<Client> сurrentclients = new List<Client>();
        static readonly List<Client> allclients = new List<Client>();
        static readonly List<Chat> chats = new List<Chat>();
        static readonly Stopwatch LastConnectStopwatch = new Stopwatch();
        static readonly List<TextBlock> tbpClientList = new List<TextBlock>();

        bool isWorkdayStarted = false;

        public static readonly HubConnection ClientHandlerConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}listen/clients", options => {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        public static readonly HubConnection ReportHandleConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}report", options => {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        public static readonly HubConnection ChatSystemConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}chat", options => {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        public static readonly HubConnection ScreenSystemConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}screen", options => {
            options.UseDefaultCredentials = true;
            if (bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                options.Transports = HttpTransportType.WebSockets;
                options.SkipNegotiation = true;
            }
            options.Headers.Add("User-Agent", "Mozilla/5.0");
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        private static readonly System.Windows.Forms.NotifyIcon notifyIcon1 = new System.Windows.Forms.NotifyIcon()
        {
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location)
        };

        public static event Action<Screen, Client> OnNewScreen;

        public static event Action<Screen, Client> OnNewWebcam;

        public MainWindow()
        {
            try
            {
                #region Configure ClientListener Connection
                ClientHandlerConnection.ServerTimeout = TimeSpan.FromDays(2);
                ClientHandlerConnection.On<List<ClientState>>("GetAllClients", UpdateClients);
                ClientHandlerConnection.On<ClientState>("OnUserAuth", AddClient);
                ClientHandlerConnection.On<ClientState>("OnUserLeft", DeleteClient);
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
                Task.Run(async () =>
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
                ReportHandleConnection.On<Report>("CreateReport", CreateReport);
                ReportHandleConnection.On("SetWorkday", new Action<bool>(async(x) => {
                    await Task.Run(() => ChangeWorkdayValue(x));
                }));
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
                Task.Run(async () =>
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
                ChatSystemConnection.On<uint, string, string, DateTime>("ReceiveMessage", RecieveMessage);
                ChatSystemConnection.On<uint>("AcceptChatID", AcceptChat);
                ChatSystemConnection.On<uint>("CloseDeleteChat", DeleteChat);
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
                Task.Run(async () =>
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
                ScreenSystemConnection.On<Screen, Client, ScreenType>("NewScreen", rmNewScreen);
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
                Task.Run(async () =>
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

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    notifyIcon1.Visible = true;
                    notifyIcon1.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
                    notifyIcon1.ContextMenuStrip.Items.Add("Exit", null, ExitItem_Click);
                    notifyIcon1.DoubleClick += NotifyIcon1_MouseDoubleClick;
                }));

                InitializeComponent();

                mConnect.Header = $"Подключено к {App.ServerAddress}";

                foreach (var obj in WinHelper.FindVisualChildren<Label>(Grid))
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

        private async void DeleteChat(uint id)
        {
            var chat = chats.Find(x => x.ChatID == id);
            await Dispatcher.BeginInvoke((Action)(() => { chat.chatClosed = true; chat.Close(); }));
            MessageBox.Show($"Чат {id} был закрыт и удален, так как одна из сторон закрыла соединение. Вы можете заново открыть чат (пользователь: ID {chat.Client.Id} / {chat.Client.Nameofpc})");
            chats.Remove(chat);
        }

        private void RecieveMessage(uint id, string message, string user, DateTime timestamp)
        {
            var chat = chats.Find(x => x.ChatID == id);
            chat.Dispatcher.BeginInvoke((Action)(() => chat.AddMessage(timestamp, user, message)));
        }

        private void AcceptChat(uint id)
        {
            var chat = chats.Find(x => x.ChatID == id);
            chat.Dispatcher.BeginInvoke((Action)(() => {
                chat.Show();
                chat.Activate();
            }));
            RemoveNotification(notifications.Find(x => x.Name == $"ChatWait{chat.Client.Id}"));
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, EventArgs e)
        {
            App.Current.Windows[0].Show();
            App.Current.Windows[0].Activate();
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
                    MessageBox.Show("(17.2) Получен пустой отчет. Мы даже не знаем, от кого он :/\nВозможно на API совершена атака.");
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

                    var tb = WinHelper.FindChild<TextBlock>(ClientList, $"ID{report.Client.Id}");
                    if (tb != null)
                    {
                        tb.Foreground = report.OverallRating > 0.5 ? App.RedColor : App.GreenColor;
                    } // optimize for disconnected users
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
        private async void UpdateClients(List<ClientState> _clientStates)
        {
            try
            {
                LastConnectStopwatch.Stop();
                await Dispatcher.BeginInvoke(new Action(async() => {
                    mLastConnectTime.Header = $"Последнее подключение длилось {LastConnectStopwatch.ElapsedMilliseconds} мс";
                    ClientList.Children.Clear();
                    var temp = mReports.Items[0];
                    mReports.Items.Clear();
                    mReports.Items.Add(temp);
                    сurrentclients.Clear();
                    allclients.Clear();
                    foreach (var client in _clientStates)
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
                    var client = clientState.Client;
                    TextBlock temptextblock = new TextBlock()
                    {
                        Name = $"ID{client.Id}",
                        Text = $"ID {client.Id} - {client.Nameofpc}",
                        Foreground = App.FontColor
                    };
                    temptextblock.MouseEnter += TextBlock_GotMouseCapture;
                    сurrentclients.Add(client);
                    allclients.Add(client);
                    clientStates.Add(clientState);
                    lClientList.Content = $"Список пользователей ({сurrentclients.Count}):";
                    ClientList.Children.Add(temptextblock);
                    tbpClientList.Add(temptextblock);

                    MenuItem tempmenuitem = new MenuItem()
                    {
                        Name = temptextblock.Name,
                        Header = temptextblock.Text,
                        Foreground = App.FontColor
                    };
                    tempmenuitem.Click += rmUniversalUser_Click;
                    mReports.Items.Add(tempmenuitem);
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
                var client = clientState.Client;
                if (client != null)
                {
                    OnUserDisconnected?.Invoke(client);
                    await Dispatcher.BeginInvoke((Action)(() =>
                    {
                        var a = сurrentclients.Find(x => x.Id == client.Id);
                        сurrentclients.Remove(a);
                        lClientList.Content = $"Список пользователей ({сurrentclients.Count}):";
                        var temptextblock = tbpClientList.Find(x => x.Text == $"ID {client.Id} - {client.Nameofpc}");
                        ClientList.Children.Remove(temptextblock);
                        tbpClientList.Remove(temptextblock);
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
                int id = Convert.ToInt32((e.Source as TextBlock).Text.GetUntilOrEmpty(" -").Substring(3));
                Button button = new Button()
                {
                    Content = $"({id}) Начать чат",
                    Margin = (e.Source as FrameworkElement).Margin
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
                var id = uint.Parse((e.Source as Button).Content.ToString().GetUntilOrEmpty(")").Remove(0,1));
                var client = сurrentclients.Find(x => x.Id == id);
                await Dispatcher.BeginInvoke((Action)(async() => {
                    Chat chat = new Chat(ChatSystemConnection, await ChatSystemConnection.InvokeAsync<uint>("OpenChat", id), client);
                    chats.Add(chat);
                    TextBlock textBlock = new TextBlock()
                    {
                        Text = $"({DateTime.Now.ToShortTimeString()}) Мы ожидаем ответа на открытие чата от {id} / {client.Nameofpc}",
                        Foreground = App.FontColor
                    };
                    textBlock.MouseEnter += Notification_GotMouseCapture;
                    textBlock.MouseLeave += Notification_LostMouseCapture;
                    Notification notification = new TextActionNotification()
                    {
                        Name = $"ChatWait{id}",
                        NotifyBlock = textBlock
                    };
                    AddNotification(notification);
                }));
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

                Dispatcher.BeginInvoke(new Action(delegate {
                    ReportWindow reportWindow = new ReportWindow(App.reports[notification.NotifyReportIndex]);
                    reportWindow.Show();
                    reportWindow.Activate();
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
                _ = MessageBox.Show($"Client Listener: {ClientHandlerConnection.State} [{await ClientHandlerConnection.InvokeAsync<string>("GetTransportType")}]\nReport Listener: {ReportHandleConnection.State} [{await ReportHandleConnection.InvokeAsync<string>("GetTransportType")}]\nChat System: {ChatSystemConnection.State} [{await ChatSystemConnection.InvokeAsync<string>("GetTransportType")}]\nScreen Transfer: {ScreenSystemConnection.State} [{await ScreenSystemConnection.InvokeAsync<string>("GetTransportType")}]");
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex, false);
                _ = MessageBox.Show($"Client Listener: {ClientHandlerConnection.State}\nReport Listener: {ReportHandleConnection.State}\nChat System: {ChatSystemConnection.State}\n Screen Transfer: {ScreenSystemConnection.State}");
            }
        }

        private void MenuManage_Settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings settings = new Settings();
                settings.Show();
                settings.Activate();
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
                _ = MessageBox.Show($"Assistant in Monitoring the Work of Employees Administrator\nVersion 1.4.2022.1401 beta 7\nAMWE RealTime server version 1.3.2022.1401\nMade by Zerumi (Discord: Zerumi#4666)\nGitHub: https://github.com/Zerumi");
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

        private async void mConnect_Click(object sender, RoutedEventArgs e)
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
                DiagnosticExceptionWindow diagnostic1 = new DiagnosticExceptionWindow();
                diagnostic1.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void DiagnoseServer_Click(object sender, RoutedEventArgs e)
        {
            DiagnoseServer wserver = new DiagnoseServer();
            wserver.Show();
            wserver.Activate();
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
            var ps = new ProcessStartInfo("https://amwe.glitch.me/")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        private void rmUniversalUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = uint.Parse((e.Source as FrameworkElement).Name.Remove(0, 2));
                var client = allclients.Find(x => x.Id == id);

                UserReports userReports = new(client, !(сurrentclients.IndexOf(client) == -1));
                userReports.Show();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void rmNewScreen(Screen screen, Client client, ScreenType type)
        {
            switch (type)
            {
                case ScreenType.ScreenImage:
                    OnNewScreen?.Invoke(screen, client);
                    break;
                case ScreenType.WebcamImage:
                    OnNewWebcam?.Invoke(screen, client);
                    break;
                default:
                    _ = MessageBox.Show("(17.6) С сервера получено неизвестное значение типа изображения.");
                    break;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}