using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Cookie AuthCookie;
        public static string ServerAddress { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

        }
    }
}