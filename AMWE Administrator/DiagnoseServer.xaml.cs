// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для DiagnoseServer.xaml
    /// </summary>
    public partial class DiagnoseServer : Window
    {
        private bool CheckEnded = false;

        public async void WriteLine(string line)
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                sowtblOutput.Text += $"{line}\n";
            }));
        }

        public DiagnoseServer()
        {
            InitializeComponent();
            Diagnose();
        }

        async void Diagnose()
        {
            try
            {
                Uri uri = new Uri(App.ServerAddress);
                WriteLine($"({DateTime.Now.ToLongTimeString()}) [Welcome]: Diagnostic connection to AMWE Server located on {uri.AbsoluteUri}...");
                WriteLine($"({DateTime.Now.ToLongTimeString()}) [Stable Hubs]: Connection state for ClientListener Hub is {(App.Current.Windows[0] as MainWindow).ClientHandlerConnection.State}");
                WriteLine($"({DateTime.Now.ToLongTimeString()}) [Stable Hubs]: Connection state for ReportListener Hub is {(App.Current.Windows[0] as MainWindow).ReportHandleConnection.State}");
                WriteLine($"({DateTime.Now.ToLongTimeString()}) [Stable Hubs]: Connection state for ChatSystem Hub is {(App.Current.Windows[0] as MainWindow).ChatSystemConnection.State}");
                // ping calculate
                Ping pingSender = new Ping();
                string host = uri.Host;
                long ping = 0;
                await Task.Run(async() =>
                {
                    PingReply reply = pingSender.Send(host);
                    if (reply.Status == IPStatus.Success)
                    {
                        ping = reply.RoundtripTime;
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lPingMarker.Content = $"Ping: {ping}ms";
                        }));
                        WriteLine($"({DateTime.Now.ToLongTimeString()}) [Ping]: Address: {reply.Address}");
                        WriteLine($"({DateTime.Now.ToLongTimeString()}) [Ping]: RoundTrip time: {ping}ms");
                        WriteLine($"({DateTime.Now.ToLongTimeString()}) [Ping]: Time to live: {reply.Options.Ttl}");
                        WriteLine($"({DateTime.Now.ToLongTimeString()}) [Ping]: Don't fragment: {reply.Options.DontFragment}");
                        WriteLine($"({DateTime.Now.ToLongTimeString()}) [Ping]: Buffer size: {reply.Buffer.Length}");
                    }
                    else
                    {
                        WriteLine($"({DateTime.Now.ToLongTimeString()}) [Ping]: Address: {reply.Status}");
                    }
                });
                // connect to sandbox hub
                await Task.Run(async () =>
                {
                    Stopwatch stopwatch = new Stopwatch();
                    WriteLine($"({DateTime.Now.ToLongTimeString()}) [Sandbox]: Starting connection building to Sandbox Hub...");
                    stopwatch.Start();
                    HubConnection sandboxHubConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}sandbox", options => {
                        options.Cookies.Add(App.AuthCookie);
                    }).Build();
                    _ = sandboxHubConnection.On("Connected", new Action<double>(async (x) =>
                      {
                          stopwatch.Stop();
                          double ConnectedAndDeliverTime = DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                          double delivery = ConnectedAndDeliverTime - x;
                          await Dispatcher.BeginInvoke(new Action(() =>
                          {
                              lTypeMarker.Content = $"Подключено! Получаем протокол...";
                              lConnectionTimeMarker.Content = $"Время стабилизации: {stopwatch.ElapsedMilliseconds}ms";
                              lDeliveryMarker.Content = $"Доставка ответа: {delivery}ms";
                          }));
                          WriteLine($"({DateTime.Now.ToLongTimeString()}) [Sandbox]: Connected");
                          WriteLine($"({DateTime.Now.ToLongTimeString()}) [Sandbox]: Measured connection creation time: {stopwatch.ElapsedMilliseconds}ms");
                          WriteLine($"({DateTime.Now.ToLongTimeString()}) [Sandbox]: Server response delivery time: {delivery}ms");
                          string type = await sandboxHubConnection.InvokeAsync<string>("GetTransportType");
                          await Dispatcher.BeginInvoke(new Action(() =>
                          {
                              lTypeMarker.Content = $"Протокол: {type}";
                          }));
                          WriteLine($"({DateTime.Now.ToLongTimeString()}) [Sandbox]: Connection estabilished with {type} protocol");
                        // Diagnose end
                        await Dispatcher.BeginInvoke(new Action(() =>
                          {
                              ConnectionType connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), type);
                              double mark = 100000000 * (int)connectionType / (0.036 * ping * delivery * stopwatch.ElapsedMilliseconds);
                              double roundmark = Math.Round(mark);
                              lMarkMarker.Content = $"Оценка: {mark}";
                              if ((delivery < 5538) && (connectionType == ConnectionType.WebSockets))
                              {
                                  lbStatus.Content = $"Статус подключения: ОК ({roundmark})";
                              }
                              else if (connectionType == ConnectionType.ServerSentEvents)
                              {
                                  lbStatus.Content = $"Статус подключения: Совсем не всё в порядке ({roundmark})";
                                  MessageBox.Show("Check your internet connection configuration, because we can't estabilish WebSockets connection.");
                              }
                              else if (stopwatch.ElapsedMilliseconds - delivery < 14000)
                              {
                                  lbStatus.Content = $"Статус подключения: Медленная конфигурация ({roundmark})";
                              }
                              else if (delivery < 7000 || ping > 400)
                              {
                                  lbStatus.Content = $"Статус подключения: Медленное соединение ({roundmark})";
                              }
                              else
                              {
                                  lbStatus.Content = "Server status: Bad";
                              }
                              CheckEnded = true;
                          }));
                          await sandboxHubConnection.DisposeAsync();
                      }));
                    await sandboxHubConnection.StartAsync();
                });
            }
            catch(Exception ex)
            {
                WriteLine($"({DateTime.Now.ToLongTimeString()}) [Error]: Диагностика остановлена из-за возникшего исключения {ex.Message}");
                ExceptionHandler.RegisterNew(ex);
                CheckEnded = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CheckEnded)
            {
                MessageBox.Show("Дождитесь окончания проверки.");
                e.Cancel = true;
            }
        }
    }
}
