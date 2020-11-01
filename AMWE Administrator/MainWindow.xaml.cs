// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;
using ReportHandler;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection ClientHandlerConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}/client").Build();

        public MainWindow()
        {
            ClientHandlerConnection.On<List<Client>>("GetAllClients", UpdateClients);
            InitializeComponent();
        }

        public void UpdateClients(List<Client> clients)
        {

        }

        private void TextBlock_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (e.Source as UIElement).Opacity = 0;
            e.Handled = true;
        }

        private void TextBlock_LostMouseCapture(object sender, MouseEventArgs e)
        {
            (e.Source as UIElement).Opacity = 1;
            e.Handled = true;
        }

        private void Notification_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (e.Source as TextBlock).Foreground = new SolidColorBrush(Color.FromRgb(0,0,238));
        }

        private void Notification_LostMouseCapture(object sender, MouseEventArgs e)
        {
            (e.Source as TextBlock).Foreground = new SolidColorBrush(Colors.Black);
        }
    }
}
