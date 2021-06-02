// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using ReportHandler;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private Report Report { get; set; }

        public ReportWindow(Report report)
        {
            Report = report;
            InitializeComponent();

            Grid.Background = App.MainColor;
            gKeyboard.Background = App.MainColor;

            foreach(var obj in m3md2.WinHelper.FindVisualChildren<Rectangle>(gKeyboard))
            {
                obj.Stroke = App.FontColor;
            }

            foreach (var obj in m3md2.WinHelper.FindVisualChildren<Label>(gKeyboard))
            {
                obj.Foreground = App.FontColor;
            }

            foreach (var obj in m3md2.WinHelper.FindVisualChildren<Label>(Grid))
            {
                obj.Foreground = App.FontColor;
            }

            ReportOutput.Foreground = App.FontColor;
            ReportOutput.Background = App.SecondColor;
            rOverallRating.Stroke = App.FontColor;

            LinearGradientBrush ovbrush = new LinearGradientBrush()
            {
                StartPoint = new Point(1, 0),
                GradientStops = new GradientStopCollection()
                {
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 1 ? (Color)ColorConverter.ConvertFromString("#C0ff0000") : Colors.White,
                        Offset = 0.09
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.1
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.9 ? (Color)ColorConverter.ConvertFromString("#C0ff4000") : Colors.White,
                        Offset = 0.11
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.9 ? (Color)ColorConverter.ConvertFromString("#C0ff4000") : Colors.White,
                        Offset = 0.19
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.2
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.8 ? (Color)ColorConverter.ConvertFromString("#C0ff8000") : Colors.White,
                        Offset = 0.21
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.8 ? (Color)ColorConverter.ConvertFromString("#C0ff8000") : Colors.White,
                        Offset = 0.29
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.3
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.7 ? (Color)ColorConverter.ConvertFromString("#C0ffc000") : Colors.White,
                        Offset = 0.31
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.7 ? (Color)ColorConverter.ConvertFromString("#C0ffc000") : Colors.White,
                        Offset = 0.39
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.4
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.6 ? (Color)ColorConverter.ConvertFromString("#C0ffff00") : Colors.White,
                        Offset = 0.41
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.6 ? (Color)ColorConverter.ConvertFromString("#C0ffff00") : Colors.White,
                        Offset = 0.49
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.5
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.5 ? (Color)ColorConverter.ConvertFromString("#C0c0ff00") : Colors.White,
                        Offset = 0.51
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.5 ? (Color)ColorConverter.ConvertFromString("#C0c0ff00") : Colors.White,
                        Offset = 0.59
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.6
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.4 ? (Color)ColorConverter.ConvertFromString("#C0a0ff00") : Colors.White,
                        Offset = 0.61
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.4 ? (Color)ColorConverter.ConvertFromString("#C0a0ff00") : Colors.White,
                        Offset = 0.69
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.7
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.3 ? (Color)ColorConverter.ConvertFromString("#C080ff00") : Colors.White,
                        Offset = 0.71
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.3 ? (Color)ColorConverter.ConvertFromString("#C080ff00") : Colors.White,
                        Offset = 0.79
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.8
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.2 ? (Color)ColorConverter.ConvertFromString("#C060ff00") : Colors.White,
                        Offset = 0.81
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.2 ? (Color)ColorConverter.ConvertFromString("#C060ff00") : Colors.White,
                        Offset = 0.89
                    },
                    new GradientStop()
                    {
                        Color = Colors.Black,
                        Offset = 0.9
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.1 ? (Color)ColorConverter.ConvertFromString("#C020ff00") : Colors.White,
                        Offset = 0.91
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.1 ? (Color)ColorConverter.ConvertFromString("#C020ff00") : Colors.White,
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
                    var arr = item.Key.Split(", ");
                    foreach (var key in arr)
                    {
                        var rect = m3md2.WinHelper.FindChild<Rectangle>(gKeyboard, key);
                        byte alpha = (byte)Math.Ceiling((double)(255 * (double)((double)item.PressedCount / arr.Length / Report.pressingCount)));
                        byte fillalpha = (rect.Fill as SolidColorBrush)?.Color.A?? 0;
                        byte alpha2 = (byte)(0.9 * fillalpha);
                        alpha += alpha2;
                        alpha += (byte)(0.1 * (byte)(255 - alpha));
                        rect.Fill = new SolidColorBrush(Color.FromArgb(alpha, 0, 255, 0));
                    }
                }
                catch (Exception)
                {
                    // unregistred hotkey
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

            var added = report.LastProcesses.Except(report.OldProcesses).ToList();
            var removed = report.OldProcesses.Except(report.LastProcesses).ToList();

            string sChangesProcesses = "";
            foreach (var item in added)
            {
                sChangesProcesses += $"+ {item}\n";
            }
            foreach (var item in removed)
            {
                sChangesProcesses += $"- {item}\n";
            }

            gpOverallInfo.Content = $"Количество измененных процессов: {report.ProcessChangedCount}";
            tbLast.Text = sOldProcesses.Remove(0,1);
            tbCurrent.Text = sLastProcesses.Remove(0,1);
            tbChanges.Text = sChangesProcesses;

            ReportOutput.Text = $"Вердикт нейросети клиента: {report.OverallRating}\nВердикт по клавиатуре: {report.KeyBoardRating}\nВердикт по мышке: {report.MouseRating}\nВердикт по процессам: {report.ProcessRating}\n--------------------------------\nИнформация по нажатым клавишам:{sKeyPressedInfo}\n{(report.isMouseCoordChanged? "Замечено движение курсора" : "Движение курсора не было замечено")}\nИнформация по активным приложениям:{sLastProcesses}\nПо сравнению с первоначальными замерами, изменилось {report.ProcessChangedCount} процессов (список:){sOldProcesses}\nОтчет подготовлен AMWE Client'ом компьютера {report.Client.Nameofpc} и обработан AMWE Administrator для {App.Username}\n{DateTime.Now} по локальному времени";
        }
    }
}
