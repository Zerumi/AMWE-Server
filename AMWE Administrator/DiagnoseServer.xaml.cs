using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для DiagnoseServer.xaml
    /// </summary>
    public partial class DiagnoseServer : Window
    {
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
                await Task.Run(() =>
                {
                    PingReply reply = pingSender.Send(host);
                    if (reply.Status == IPStatus.Success)
                    {
                        ping = reply.RoundtripTime;
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
                    sandboxHubConnection.On("Connected", new Action<double>(async (x) =>
                    {
                        stopwatch.Stop();
                        double ConnectedAndDeliverTime = DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                        double delivery = ConnectedAndDeliverTime - x;
                        WriteLine($"({DateTime.Now.ToLongTimeString()}) [Sandbox]: Connected");
                        WriteLine($"({DateTime.Now.ToLongTimeString()}) [Sandbox]: Measured connection creation time: {stopwatch.ElapsedMilliseconds}ms");
                        WriteLine($"({DateTime.Now.ToLongTimeString()}) [Sandbox]: Server response delivery time: {delivery}ms");
                        string type = await sandboxHubConnection.InvokeAsync<string>("GetTransportType");
                        WriteLine($"({DateTime.Now.ToLongTimeString()}) [Sandbox]: Connection estabilished with {type} protocol");
                        await Dispatcher.BeginInvoke(new Action(() => {
                            if ((delivery < 5538) && (type == "WebSockets"))
                            {
                                lbStatus.Content = "Server status: OK";
                            }
                            else if (type == "ServerSentEvents")
                            {
                                lbStatus.Content = "Server status: Something isn't ok";
                                MessageBox.Show("Check your internet connection configuration, because we can't estabilish WebSockets connection.");
                            }
                            else if (stopwatch.ElapsedMilliseconds - delivery < 14000)
                            {
                                lbStatus.Content = "Server status: Slow configuration";
                            }
                            else if (delivery < 7000 || ping > 400)
                            {
                                lbStatus.Content = "Server status: Slow connection";
                            }
                            else
                            {
                                lbStatus.Content = "Server status: Bad";
                            }
                        }));
                        await sandboxHubConnection.DisposeAsync();
                    }));
                    await sandboxHubConnection.StartAsync();
                });
            }
            catch(Exception ex)
            {
                WriteLine($"({DateTime.Now.ToLongTimeString()}) []: Диагностика остановлена из-за возникшего исключения {ex.Message}");
                ExceptionHandler.RegisterNew(ex);
            }
        }
    }
}
