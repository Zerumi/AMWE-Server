﻿using Microsoft.AspNetCore.SignalR.Client;
using ReportHandler;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        private readonly HubConnection hubConnection;
        public uint ChatID { get; set; }
        public Client Client { get; set; }
        public bool ChatClosed { get; set; }

        public Chat(HubConnection chatHubConnection, uint _ChatID, Client client)
        {
            hubConnection = chatHubConnection;
            ChatID = _ChatID;
            Client = client;
            InitializeComponent();
            lbChatState.Content = $"Вы ведете чат с ID {client.Id}: {client.Nameofpc} №{ChatID}";
            Grid.Background = App.MainColor;
            ChatBox.Foreground = App.FontColor;
            tbMessage.Background = App.SecondColor;
            tbMessage.Foreground = App.FontColor;
            lbChatState.Foreground = App.FontColor;
        }

        public void AddMessage(DateTime timestamp, string user, string Message)
        {
            ChatBox.Text += $"({timestamp.ToLongTimeString()}) {user}: {Message}\n";
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ChatClosed)
            {
                await hubConnection.InvokeAsync("CloseChat", ChatID);
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await Send();
        }

        private async void Field_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await Send();
            }
        }

        private async Task Send()
        {
            await hubConnection.InvokeAsync("SendMessageToChat", ChatID, tbMessage.Text);
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                tbMessage.Text = string.Empty;
            }));
        }
    }
}
