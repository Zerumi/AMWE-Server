using ReportHandler;
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
using System.Windows.Forms;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private Report _Report { get; set; }

        public ReportWindow(Report report)
        {
            _Report = report;
            InitializeComponent();
            LinearGradientBrush ovbrush = new LinearGradientBrush()
            {
                StartPoint = new Point(1, 0),
                GradientStops = new GradientStopCollection()
                {
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 1 ? (Color)ColorConverter.ConvertFromString("#C1ff0000") : Colors.White,
                        Offset = 0.09
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.1
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.9 ? (Color)ColorConverter.ConvertFromString("#C1ff4000") : Colors.White,
                        Offset = 0.11
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.9 ? (Color)ColorConverter.ConvertFromString("#C1ff4000") : Colors.White,
                        Offset = 0.19
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.2
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.8 ? (Color)ColorConverter.ConvertFromString("#C1ff8000") : Colors.White,
                        Offset = 0.21
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.8 ? (Color)ColorConverter.ConvertFromString("#C1ff8000") : Colors.White,
                        Offset = 0.29
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.3
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.7 ? (Color)ColorConverter.ConvertFromString("#C1ffc000") : Colors.White,
                        Offset = 0.31
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.7 ? (Color)ColorConverter.ConvertFromString("#C1ffc000") : Colors.White,
                        Offset = 0.39
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.4
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.6 ? (Color)ColorConverter.ConvertFromString("#C1ffff00") : Colors.White,
                        Offset = 0.41
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.6 ? (Color)ColorConverter.ConvertFromString("#C1ffff00") : Colors.White,
                        Offset = 0.49
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.5
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.5 ? (Color)ColorConverter.ConvertFromString("#C1c0ff00") : Colors.White,
                        Offset = 0.51
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.5 ? (Color)ColorConverter.ConvertFromString("#C1c0ff00") : Colors.White,
                        Offset = 0.59
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.6
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.4 ? (Color)ColorConverter.ConvertFromString("#C1a0ff00") : Colors.White,
                        Offset = 0.61
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.4 ? (Color)ColorConverter.ConvertFromString("#C1a0ff00") : Colors.White,
                        Offset = 0.69
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.7
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.3 ? (Color)ColorConverter.ConvertFromString("#C180ff00") : Colors.White,
                        Offset = 0.71
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.3 ? (Color)ColorConverter.ConvertFromString("#C180ff00") : Colors.White,
                        Offset = 0.79
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.8
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.2 ? (Color)ColorConverter.ConvertFromString("#C160ff00") : Colors.White,
                        Offset = 0.81
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.2 ? (Color)ColorConverter.ConvertFromString("#C160ff00") : Colors.White,
                        Offset = 0.89
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.9
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.1 ? (Color)ColorConverter.ConvertFromString("#C120ff00") : Colors.White,
                        Offset = 0.91
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.1 ? (Color)ColorConverter.ConvertFromString("#C120ff00") : Colors.White,
                        Offset = 1
                    }
                }
            };
            rOverallRating.Fill = ovbrush;
            ReportHeader.Content = $"Отчет №{App.reports.IndexOf(report)} от {report.Client.Nameofpc} ({report.Client.Id}):";
            string sKeyPressedInfo = string.Empty;
            foreach (var item in report.KeyPressedInfo)
            {
                sKeyPressedInfo += $"\n{item.Key} - {item.PressedCount}";
                try
                {
                    var alpha = (byte)Math.Ceiling((double)(255 * (double)((double)item.PressedCount / _Report.pressingCount)));
                    alpha += (byte)(0.1 * (byte)(255 - alpha));
                    m3md2.WinHelper.FindChild<Rectangle>(gKeyboard, item.Key).Fill = new SolidColorBrush(Color.FromArgb(alpha, 0, 255, 0));
                }
                catch (NullReferenceException)
                {
                    // this is a hotkey
                }
            }
            string sOldProcesses = string.Empty;
            foreach (var item in report.OldProcesses)
            {
                sOldProcesses += $"\n{item}";
            }
            string sLastProcesses = string.Empty;
            foreach (var item in report.LastProcesses)
            {
                sLastProcesses += $"\n{item}";
            }
            ReportOutput.Text = $"Вердикт нейросети клиента: {report.OverallRating}\nВердикт по клавиатуре: {report.KeyBoardRating}\nВердикт по мышке: {report.MouseRating}\nВердикт по процессам: {report.ProcessRating}\n--------------------------------\nИнформация по нажатым клавишам:{sKeyPressedInfo}\n{(report.isMouseCoordChanged? "Замечено движение курсора" : "Движение курсора не было замечено")}\nИнформация по активным приложениям:{sLastProcesses}\nПо сравнению с первоначальными замерами, изменилось {report.ProcessChangedCount} процессов (список:){sOldProcesses}\nОтчет подготовлен AMWE Client'ом компьютера {report.Client.Nameofpc} и обработан AMWE Administrator для {App.Username}\n{DateTime.Now} по локальному времени";
        }
    }
}
