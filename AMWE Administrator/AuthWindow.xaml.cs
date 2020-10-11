using m3md2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
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
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        public static bool AuthUser((string, string) authdata, out Cookie cookie)
        {
            bool returnproduct = default;
            try
            {
                CookieContainer cookies = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = cookies;

                HttpClient client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(App.ServerAddress)
                };
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
                string json = new JavaScriptSerializer().Serialize(authdata);
                HttpResponseMessage response = client.PostAsync($"auth", new StringContent(json, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                returnproduct = response.Content.ReadAsAsync<bool>().GetAwaiter().GetResult();
                if (returnproduct)
                {
                    Uri uri = new Uri($"{App.ServerAddress}auth");
                    IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
                    CookieCollection collection = new CookieCollection();
                    responseCookies.ToList().ForEach(x =>
                    {
                        collection.Add(x);
                    });
                    cookie = collection[".AspNetCore.Cookies"];
                }
                else
                {
                    cookie = default;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                cookie = default;
            }
            return returnproduct;
        }


        public string ResponseText
        {
            get
            {
                if (cbShow.IsChecked.GetValueOrDefault())
                {
                    return sResponseTextBox.Text;
                }
                return ResponseTextBox.Password;
            }
            set
            {
                if (cbShow.IsChecked.GetValueOrDefault())
                {
                    sResponseTextBox.Text = value;
                }
                ResponseTextBox.Password = value;
            }
        }

        public string ServerText
        {
            get { return ServerTextBox.Text; }
            set { ServerTextBox.Text = value; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void Field_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Load();
            }
        }

        private async void Load()
        {
            try
            {
                ResponseTextBox.KeyDown -= Field_KeyDown;
                sResponseTextBox.KeyDown -= Field_KeyDown;
                AuthButton.IsEnabled = false;
                App.ServerAddress = ServerText;
                AuthButton.Content = "Проверка...";
                if (AuthUser((UsernameTextBox.Text, Encryption.Encrypt(ResponseText)), out App.AuthCookie))
                {
                    ConfigurationRequest.WriteValueByKey("MainUri", ServerText);
                    AuthButton.Content = "Загрузка сборок...";
                    if (await CheckVersion())
                    {
                        AuthButton.Content = "Загрузка...";
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        System.Windows.Application.Current.MainWindow.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Неправильный пароль");
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
            finally
            {
                AuthButton.Content = "ОК";
                AuthButton.IsEnabled = true;
                ResponseTextBox.KeyDown += Field_KeyDown;
                sResponseTextBox.KeyDown += Field_KeyDown;
            }
        }

        private async Task<bool> CheckVersion()
        {
            return true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ResponseTextBox.Visibility = Visibility.Collapsed;
                sResponseTextBox.Visibility = Visibility.Visible;
                sResponseTextBox.Text = ResponseTextBox.Password;
                sResponseTextBox.Focus();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void cbShow_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                sResponseTextBox.Visibility = Visibility.Collapsed;
                ResponseTextBox.Visibility = Visibility.Visible;
                ResponseTextBox.Password = sResponseTextBox.Text;

                ResponseTextBox.Focus();
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServerText = ConfigurationRequest.GetValueByKey("MainUri");
        }
    }
}
