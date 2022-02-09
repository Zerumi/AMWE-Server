// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Linq;
using System.Net;
using System.Windows;
using m3md2;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public UpgList<bool> BitArray = new();

        public int Colorindex;

        public Settings()
        {
            InitializeComponent();
            comboxColorTheme.ItemsSource = ColorThemes.GetColorNames();
            Colorindex = Array.IndexOf(comboxColorTheme.ItemsSource.OfType<string>().ToArray(), Array.Find(comboxColorTheme.ItemsSource.OfType<string>().ToArray(), x => x == ConfigurationRequest.GetValueByKey("ColorTheme")));
            comboxColorTheme.SelectedIndex = Colorindex;

            BitArray.OnAdd += BitArray_OnAdd;
            BitArray.OnRemove += BitArray_OnRemove;

            cbAllowSocketsOnly.IsChecked = bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly"));

            cbExpect100Continue.IsChecked = !bool.Parse(ConfigurationRequest.GetValueByKey("Expect100Continue"));

            cbMinimizeToTray.IsChecked = bool.Parse(ConfigurationRequest.GetValueByKey("MinimizeToTray"));

            cbCheckReports.IsChecked = bool.Parse(ConfigurationRequest.GetValueByKey("CheckReports"));

            cbCheckApps.IsChecked = bool.Parse(ConfigurationRequest.GetValueByKey("CheckApps"));

            cbCheckSites.IsChecked = bool.Parse(ConfigurationRequest.GetValueByKey("CheckSites"));
        }

        private void BitArray_OnAdd(object sender, EventArgs e)
        {
            lbRestartRequired.Visibility = Visibility.Visible;
        }

        private void BitArray_OnRemove(object sender, EventArgs e)
        {
            if (BitArray.Count - 1 == 0)
            {
                lbRestartRequired.Visibility = Visibility.Hidden;
            }
        }

        private void CbAllowSocketsOnly_Checked(object sender, RoutedEventArgs e)
        {
            if (cbAllowSocketsOnly.IsChecked.GetValueOrDefault(true) != bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                BitArray.Add(true);
            }
            else
            {
                BitArray.Remove(true);
            }
        }

        private bool isColorChanged;

        private void ComboxColorTheme_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboxColorTheme.SelectedIndex != Colorindex && !isColorChanged)
            {
                isColorChanged = true;
                BitArray.Add(true);
            }
            else if (comboxColorTheme.SelectedIndex == Colorindex)
            {
                isColorChanged = false;
                BitArray.Remove(true);
            }
        }

        private void Apply(object sender = default, RoutedEventArgs e = default)
        {
            ConfigurationRequest.WriteValueByKey("WebSocketsOnly", Convert.ToString(cbAllowSocketsOnly.IsChecked.GetValueOrDefault(true)));

            bool e100c = !cbExpect100Continue.IsChecked.GetValueOrDefault(false);
            ConfigurationRequest.WriteValueByKey("Expect100Continue", Convert.ToString(e100c));
            ServicePointManager.Expect100Continue = e100c;

            ConfigurationRequest.WriteValueByKey("ColorTheme", comboxColorTheme.Text);

            ConfigurationRequest.WriteValueByKey("MinimizeToTray", Convert.ToString(cbMinimizeToTray.IsChecked.GetValueOrDefault(true)));

            if (lbRestartRequired.Visibility == Visibility.Visible)
            {
                System.Windows.Forms.Application.Restart();

                Environment.Exit(0);
            }

            Close();
        }

        private void BAddSite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BAddApp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CbCheckReports_Checked(object sender, RoutedEventArgs e)
        {
            gCheckReports.IsEnabled = true;
        }

        private void CbCheckReports_Unchecked(object sender, RoutedEventArgs e)
        {
            gCheckReports.IsEnabled = false;
        }

        private void CbCheckApps_Checked(object sender, RoutedEventArgs e)
        {
            lbAppsToCheck.IsEnabled = true;
        }

        private void CbCheckApps_Unchecked(object sender, RoutedEventArgs e)
        {
            lbAppsToCheck.IsEnabled = false;
        }

        private void CbCheckSites_Checked(object sender, RoutedEventArgs e)
        {
            lbSitesToCheck.IsEnabled = true;
        }

        private void CbCheckSites_Unchecked(object sender, RoutedEventArgs e)
        {
            lbSitesToCheck.IsEnabled = false;
        }
    }
}