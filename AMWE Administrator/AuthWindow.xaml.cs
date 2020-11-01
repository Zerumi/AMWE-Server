﻿// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using ReportHandler.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Input;

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

        public static object AuthUser(string[] authdata, out Cookie cookie)
        {
            object returnproduct = default;
            try
            {
                CookieContainer cookies = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler
                {
                    CookieContainer = cookies
                };

                HttpClient client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(App.ServerAddress)
                };
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
                string json = new JavaScriptSerializer().Serialize(authdata);
                HttpResponseMessage response = client.PostAsync($"auth", new StringContent(json, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                returnproduct = response.Content.ReadAsAsync<object>().GetAwaiter().GetResult();
                try
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
                catch (Exception)
                {
                    cookie = default;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
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

        private void Field_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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
                object authresult = default;
                var username = UsernameTextBox.Text;
                var nocryptpass = ResponseText;
                await Task.Run(() => 
                {
                    authresult = AuthUser(new string[] { username, Encryption.Encrypt(nocryptpass), Assembly.GetExecutingAssembly().GetName().Version.ToString(), Assembly.LoadFrom("ReportHandler.dll").GetName().Version.ToString(), Assembly.LoadFrom("m3md2.dll").GetName().Version.ToString(), Assembly.LoadFrom("m3md2_startup.dll").GetName().Version.ToString() }, out App.AuthCookie);
                });
                if (authresult is List<VersionFile>)
                {
                    // update
                }
                switch (authresult)
                {
                    case bool _:
                        {
                            if ((bool)authresult)
                            {
                                ConfigurationRequest.WriteValueByKey("MainUri", ServerText);
                                AuthButton.Content = "Загрузка...";
                                MainWindow mainWindow = new MainWindow();
                                mainWindow.Show();
                                Application.Current.MainWindow.Close();
                            }
                            else
                            {
                                MessageBox.Show("Неправильный пароль");
                            }

                            break;
                        }

                    case string _:
                        {
                            if ((string)authresult == "Developer")
                            {
                                ConfigurationRequest.WriteValueByKey("MainUri", ServerText);
                                DeveloperControlPanel controlPanel = new DeveloperControlPanel();
                                controlPanel.Show();
                                Application.Current.MainWindow.Close();
                            }

                            break;
                        }

                    default:
                        {
                            MessageBox.Show($"Мы получили странный объект, с которым не знаем, что делать:{authresult}");

                            break;
                        }
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

        private void CbShow_Unchecked(object sender, RoutedEventArgs e)
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
            UsernameTextBox.Text = Environment.UserName;
            ServerText = ConfigurationRequest.GetValueByKey("MainUri");
            ResponseTextBox.Focus();
        }
    }
}
