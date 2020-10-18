using Microsoft.AspNetCore.SignalR.Client;
using ReportHandler.Version;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для DeveloperControlPanel.xaml
    /// </summary>
    public partial class DeveloperControlPanel : Window
    {
        string[] FilesPath;
        HubConnection connection;

        public DeveloperControlPanel()
        {
            InitializeComponent();

            string FilesInDir = string.Empty;
            FilesPath = Directory.GetFiles(Directory.GetCurrentDirectory()).Select(System.IO.Path.GetFileName).ToArray();
            Array.ForEach(FilesPath, x => FilesInDir += $"\n{x}");
            tb_FilesInDir.Text = $"Current files in {Directory.GetCurrentDirectory()}: {FilesInDir}";

            curVer.Content = $"Current version: {Assembly.GetExecutingAssembly().GetName().Version}";
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
            connection = new HubConnectionBuilder().WithUrl($"{ConfigurationRequest.GetValueByKey("MainUri")}server", options =>
            {
                options.UseDefaultCredentials = true;
                options.Cookies.Add(App.AuthCookie);
            })
            .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            await connection.StartAsync();

            MessageBox.Show(Convert.ToString(await connection.InvokeAsync<object>("NewLatestVersion", "admin", "0.6.0.0", version)));
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
