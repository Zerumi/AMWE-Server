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
    /// Логика взаимодействия для TimeSetter.xaml
    /// </summary>
    public partial class TimeSetter : Window
    {
        public TimeSetter(TimeSpan current)
        {
            InitializeComponent();
            tbHour.Text = current.Hours.ToString();
            tbMinute.Text = current.Minutes.ToString();
        }

        private void tbHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool isApprove = int.TryParse(tbHour.Text, out int hour);
                if (isApprove)
                {
                    double angle = 30 * (hour + (double)(int.Parse(tbMinute?.Text ?? "10") / 60.0d)); // 360 / 12 * (hour + minute)
                    HourRotate.Angle = angle;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void tbMinute_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool isApprove = int.TryParse(tbMinute.Text, out int minute);
                if (isApprove)
                {
                    double hangle = 30 * (int.Parse(tbHour.Text) + (double)(minute / 60.0d)); // 360 / 12 * (hour + minute)
                    double mangle = 6 * minute; // 360 / 60 * minute
                    HourRotate.Angle = hangle;
                    MinRotate.Angle = mangle;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isApproveH = int.TryParse(tbHour.Text, out int hour),
                 isAproveM = int.TryParse(tbMinute.Text, out int minute);
            if (isApproveH && isAproveM)
            {
                TimeSpan interval = new TimeSpan(hour, minute, 00);
                _ = MainWindow.ReportHandleConnection.InvokeAsync("UpdateReportPollingTime", interval);
                Close();
            }
        }
    }
}
