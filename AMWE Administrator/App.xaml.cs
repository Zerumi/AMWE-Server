// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using m3md2;
using ReportHandler;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

#pragma warning disable CA2211 // Поля, не являющиеся константами, не должны быть видимыми
        public static Cookie AuthCookie;
        public static string ServerAddress { get; set; }
        public static string Username { get; set; }
        public static List<Report> reports = new();
        public static DateTime ServerDateTime;

        public static List<CheckModel> AppsToCheck = new();
        public static List<CheckModel> SitesToCheck = new();

        private static readonly string ColorTheme = ConfigurationRequest.GetValueByKey("ColorTheme");
        public static Color[] colors = ColorThemes.GetColors(ColorTheme);
        public static Color[] repGradient = ColorThemes.GetGradient(ColorTheme);
        public static SolidColorBrush MainColor = new(colors[(int)ColorIndex.Main]);
        public static SolidColorBrush SecondColor = new(colors[(int)ColorIndex.Second]);
        public static SolidColorBrush FontColor = new(colors[(int)ColorIndex.Font]);
        public static SolidColorBrush ExtraColor = new(colors[(int)ColorIndex.Extra]);
        public static SolidColorBrush GreenColor = new(colors[(int)ColorIndex.Green]);
        public static SolidColorBrush RedColor = new(colors[(int)ColorIndex.Red]);
        public static SolidColorBrush ControlColor = new(colors[(int)ColorIndex.Control]);
        public static SolidColorBrush LineChartColor = new(colors[(int)ColorIndex.LineChart]);
#pragma warning restore CA2211 // Поля, не являющиеся константами, не должны быть видимыми

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ServicePointManager.Expect100Continue = bool.Parse(ConfigurationRequest.GetValueByKey("Expect100Continue"));
            ConfigurationRequest.LoadCheckModels(AppsToCheck, SitesToCheck);
        }
    }
}