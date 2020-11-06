// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        HubConnection ClientHandlerConnection = new HubConnectionBuilder().WithUrl($"{App.ServerAddress}listen/clients", options => {
            options.UseDefaultCredentials = true;
            options.Cookies.Add(App.AuthCookie);
        }).Build();

        public MainWindow()
        {
            #region Configure ClientListener Connection
            ClientHandlerConnection.ServerTimeout = TimeSpan.FromDays(2);
            ClientHandlerConnection.On<List<Client>>("GetAllClients", UpdateClients);
            ClientHandlerConnection.On<Client>("OnUserAuth", AddClient);
            ClientHandlerConnection.On<Client>("OnUserLeft", DeleteClient);
            ClientHandlerConnection.Closed += async (error) =>
            {
                await ClientHandlerConnection.StartAsync();
            };
            ClientHandlerConnection.StartAsync();
            #endregion

            InitializeComponent();
        }

        private void UpdateClients(List<Client> clients)
        {
            ClientList.Children.Clear();
            foreach (var client in clients)
            {
                TextBlock temptextblock = new TextBlock()
                {
                    Text = $"ID {client.Id} - {client.Nameofpc}"
                };
                temptextblock.MouseEnter += TextBlock_GotMouseCapture;
                ClientList.Children.Add(temptextblock);
            }
        }
        
        private void AddClient(Client client)
        {
            TextBlock temptextblock = new TextBlock()
            {
                Text = $"ID {client.Id} - {client.Nameofpc}"
            };
            temptextblock.MouseEnter += TextBlock_GotMouseCapture;
            ClientList.Children.Add(temptextblock);
        }

        private void DeleteClient(Client client)
        {
            ClientList.Children.Remove(new TextBlock()
            {
                Text = $"ID {client.Id} - {client.Nameofpc}"
            });
        }

        private void TextBlock_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (e.Source as UIElement).Visibility = Visibility.Collapsed;
            int id = ClientList.Children.IndexOf(e.Source as UIElement);
            Button button = new Button()
            {
                Content = $"Управление {(e.Source as TextBlock).Text.Remove(0, id.ToString().Length + 6)}",
                Margin = (e.Source as FrameworkElement).Margin
            };
            button.MouseLeave += TextBlock_LostMouseCapture;
            ClientList.Children.Insert(id, button);
            e.Handled = true;
        }

        private void TextBlock_LostMouseCapture(object sender, MouseEventArgs e)
        {
            ClientList.Children[ClientList.Children.IndexOf(e.Source as UIElement) + 1].Visibility = Visibility.Visible;
            ClientList.Children.Remove(e.Source as UIElement);
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

        private void MenuDiagnostics_CheckConnectionState(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Client Listener: {ClientHandlerConnection.State}");
        }

        private void MenuManage_Settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void MenuManage_OpenAtExplorer_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void MenuManage_ChangeServer_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите изменить сервер? (Программа будет перезагружена)", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Forms.Application.Restart();

                Application.Current.Shutdown();
            }
            else
            {
                return;
            }
        }
    }
}