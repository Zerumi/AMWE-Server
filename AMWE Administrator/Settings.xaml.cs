// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;

namespace AMWE_Administrator
{
    public class MyList<T> : List<T>
    {
        public event EventHandler OnAdd;
        public event EventHandler OnRemove;

        public new void Add(T item) // "new" to avoid compiler-warnings, because we're hiding a method from base-class
        {
            OnAdd?.Invoke(this, null);
            base.Add(item);
        }

        public new void Remove(T item)
        {
            OnRemove?.Invoke(this, null);
            base.Remove(item);
        }
    }
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public MyList<bool> bitArray = new MyList<bool>();

        public int Colorindex;

        public Settings()
        {
            InitializeComponent();
            comboxColorTheme.ItemsSource = m3md2.ColorThemes.GetColorNames();
            Colorindex = Array.IndexOf(comboxColorTheme.ItemsSource.OfType<string>().ToArray(), Array.Find(comboxColorTheme.ItemsSource.OfType<string>().ToArray(), x => x == ConfigurationRequest.GetValueByKey("ColorTheme")));
            comboxColorTheme.SelectedIndex = Colorindex;

            bitArray.OnAdd += BitArray_OnAdd;
            bitArray.OnRemove += BitArray_OnRemove;

            cbAllowSocketsOnly.IsChecked = bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly"));

            cbExpect100Continue.IsChecked = !bool.Parse(ConfigurationRequest.GetValueByKey("Expect100Continue"));

            cbMinimizeToTray.IsChecked = bool.Parse(ConfigurationRequest.GetValueByKey("MinimizeToTray"));
        }

        private void BitArray_OnAdd(object sender, EventArgs e)
        {
            lbRestartRequired.Visibility = Visibility.Visible;
        }

        private void BitArray_OnRemove(object sender, EventArgs e)
        {
            if (bitArray.Count - 1 == 0)
            {
                lbRestartRequired.Visibility = Visibility.Hidden;
            }
        }

        private void cbAllowSocketsOnly_Checked(object sender, RoutedEventArgs e)
        {
            if (cbAllowSocketsOnly.IsChecked.GetValueOrDefault(true) != bool.Parse(ConfigurationRequest.GetValueByKey("WebSocketsOnly")))
            {
                bitArray.Add(true);
            }
            else
            {
                bitArray.Remove(true);
            }
        }

        bool isColorChanged = false;

        private void comboxColorTheme_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboxColorTheme.SelectedIndex != Colorindex && !isColorChanged)
            {
                isColorChanged = true;
                bitArray.Add(true);
            }
            else if (comboxColorTheme.SelectedIndex == Colorindex)
            {
                isColorChanged = false;
                bitArray.Remove(true);
            }
        }

        private void Apply(object sender = default, RoutedEventArgs e = default)
        {
            ConfigurationRequest.WriteValueByKey("WebSocketsOnly", Convert.ToString(cbAllowSocketsOnly.IsChecked.GetValueOrDefault(true)));

            var e100c = !cbExpect100Continue.IsChecked.GetValueOrDefault(false);
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
    }
}
