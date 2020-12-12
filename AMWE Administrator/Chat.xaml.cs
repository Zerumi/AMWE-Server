using Microsoft.AspNetCore.SignalR.Client;
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
using System.Windows.Shapes;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        HubConnection hubConnection;
        public uint ChatID;

        public Chat(HubConnection chatHubConnection, uint _ChatID)
        {
            hubConnection = chatHubConnection;
            ChatID = _ChatID;
            InitializeComponent();
        }

        public void AddMessage(string Message)
        {
            ChatBox.Text += $"{Message}\n";
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await hubConnection.InvokeAsync("CloseChat", ChatID);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await hubConnection.InvokeAsync("SendMessageToChat", ChatID, tbMessage.Text);
            tbMessage.Text = string.Empty;
        }
    }
}
