// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using Microsoft.AspNetCore.SignalR.Client;
using ReportHandler.Version;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для DeveloperControlPanel.xaml
    /// </summary>
    public partial class DeveloperControlPanel : Window
    {
        readonly string[] FilesPath;
        readonly HubConnection connection;

        public DeveloperControlPanel()
        {
            InitializeComponent();

            string FilesInDir = string.Empty;
            FilesPath = Directory.GetFiles(Directory.GetCurrentDirectory()).Select(System.IO.Path.GetFileName).ToArray();
            Array.ForEach(FilesPath, x => FilesInDir += $"\n{x}");
            tb_FilesInDir.Text = $"Current files in {Directory.GetCurrentDirectory()}: {FilesInDir}";

            curVer.Content = $"Current version: {Assembly.GetExecutingAssembly().GetName().Version}";


            connection = new HubConnectionBuilder().WithUrl($"{ConfigurationRequest.GetValueByKey("MainUri")}server", options =>
            {
                options.UseDefaultCredentials = true;
                options.Cookies.Add(App.AuthCookie);
            })
            .Build();

            connection.Closed += async (error) =>
            {
                serverStatus.Content = $"Server status for {connection.ConnectionId}: {connection.State} // ({error})";
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.Reconnecting += (error) =>
            {
                serverStatus.Content = $"Server status for {connection.ConnectionId}: {connection.State} // ({error})";
                return Task.CompletedTask;
            };

            connection.Reconnected += (error) =>
            {
                serverStatus.Content = $"Server status for {connection.ConnectionId}: {connection.State} // ({error})";
                return Task.CompletedTask;
            };

            connection.StartAsync();
            serverStatus.Content = $"Server status for {connection.ConnectionId}: {connection.State}";
            Task.Run(async() => {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        serverStatus.Dispatcher.BeginInvoke(new Action(() => Content = $"Server status for {connection.ConnectionId}: {connection.State}"));
                        Thread.Sleep(3000);
                    }
                });
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            List<VersionFile> version = new List<VersionFile>();
            for (int i = 0; i < FilesPath.Length; i++)
            {
                version.Add(new VersionFile() { filename = FilesPath[i], filebytes = File.ReadAllBytes(FilesPath[i]) });
            }
            bool overwritesave = false;
            if (await connection.InvokeAsync<bool>("CheckConflict", "admin", Assembly.GetExecutingAssembly().GetName().Version.ToString()))
            {
                overwritesave = MessageBox.Show("Обнаружен конфликт версий. Версия с таким же кодом уже загружена на сервере. Вы хотите перезаписать версию?", "Конфликт версий", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes;
            }
            MessageBox.Show(Convert.ToString(await connection.InvokeAsync<object>("NewLatestVersion", "admin", Assembly.GetExecutingAssembly().GetName().Version.ToString(), version, overwritesave)));
        }

        private void Moverect_Drop(object sender, DragEventArgs e)
        {
            //ZipArchive archive = new ZipArchive(File.Open(((string[])e.Data.GetData(DataFormats.FileDrop)).First(), FileMode.Open));
            //ObservableCollection<string> itemsdifsource = new ObservableCollection<string>();
            //foreach (var item in archive.Entries)
            //{

            //}
        }

        private void Moverect_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}
