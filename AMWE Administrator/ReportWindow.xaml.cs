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
