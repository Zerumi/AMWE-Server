// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using ReportHandler;
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
        public static string Username { get; set; }
        public static List<Report> reports = new List<Report>();
        public static DateTime ServerDateTime;
        public static IEnumerable<string> ColorThemes = m3md2.ColorThemes.GetColorNames();

        private void Application_Startup(object sender, StartupEventArgs e)
        {

        }
    }
}