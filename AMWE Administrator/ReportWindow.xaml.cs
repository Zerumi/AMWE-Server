// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using ReportHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private string sKeyPressedInfo;
        private string sLastProcesses;
        private string sOldProcesses;
        private string sCurrentSites;
        private string sOldSites;

        public ReportWindow(Report report)
        {
            try
            {
                InitializeComponent();

                Grid.Background = App.MainColor;
                gKeyboard.Background = App.SecondColor;

                TextOutputTab.Background = App.ControlColor;
                KeyboardTab.Background = App.ControlColor;
                ProccessesTab.Background = App.ControlColor;
                SitesTab.Background = App.ControlColor;
                TextOutputTab.Foreground = App.FontColor;
                KeyboardTab.Foreground = App.FontColor;
                ProccessesTab.Foreground = App.FontColor;
                SitesTab.Foreground = App.FontColor;

                Resources["TabControlBrush"] = App.ControlColor;
                Resources["TabSelectedBrush"] = App.ExtraColor;

                foreach (Rectangle obj in m3md2.WinHelper.FindVisualChildren<Rectangle>(gKeyboard))
                {
                    obj.Stroke = App.FontColor;
                }

                foreach (Label obj in m3md2.WinHelper.FindVisualChildren<Label>(gKeyboard))
                {
                    obj.Foreground = App.FontColor;
                }

                foreach (Label obj in m3md2.WinHelper.FindVisualChildren<Label>(Grid))
                {
                    obj.Foreground = App.FontColor;
                }

                tcReportVisualiser.Background = App.SecondColor;

                ReportOutput.Foreground = App.FontColor;
                ReportOutput.Background = App.SecondColor;
                ReportOutput.Text = string.Empty;

                gpOverallInfo.Foreground = App.FontColor;
                lProcInfo.Foreground = App.FontColor;
                lLast.Foreground = App.FontColor;
                lChanges.Foreground = App.FontColor;
                lCurrent.Foreground = App.FontColor;
                tbLast.Foreground = App.FontColor;
                tbChanges.Foreground = App.FontColor;
                tbCurrent.Foreground = App.FontColor;

                lBrowser.Foreground = App.FontColor;
                lSiteInfo.Foreground = App.FontColor;
                lSlast.Foreground = App.FontColor;
                lSchanges.Foreground = App.FontColor;
                lScurrent.Foreground = App.FontColor;
                tbSiteLast.Foreground = App.FontColor;
                tbSiteChanges.Foreground = App.FontColor;
                tbSiteCurrent.Foreground = App.FontColor;

                rOverallRating.Stroke = App.SecondColor;

                _ = Task.Run(new Action(async () =>
                {
                    await LoadReportAsync(report);
                }));
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async Task LoadReportAsync(Report report)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await LoadKeyboardOutput(report);
                    await LoadProcessesOutput(report);
                    await LoadSitesOutput(report);
                });
                await Task.Run(async () =>
                {
                    await LoadReportOutput(report);
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.RegisterNew(ex);
            }
        }

        private async Task LoadReportOutput(Report report)
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    LinearGradientBrush ovbrush = new()
                    {
                        StartPoint = new Point(1, 0),
                        GradientStops = new GradientStopCollection()
                    {
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 1 ? App.repGradient[0] : App.SecondColor.Color,
                        Offset = 0.09
                    },
                    new GradientStop()
                    {
                        Color = App.SecondColor.Color,
                        Offset = 0.1
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.9 ? App.repGradient[1] : App.SecondColor.Color,
                        Offset = 0.11
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.9 ? App.repGradient[1] : App.SecondColor.Color,
                        Offset = 0.19
                    },
                    new GradientStop()
                    {
                        Color = App.SecondColor.Color,
                        Offset = 0.2
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.8 ? App.repGradient[2] : App.SecondColor.Color,
                        Offset = 0.21
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.8 ? App.repGradient[2] : App.SecondColor.Color,
                        Offset = 0.29
                    },
                    new GradientStop()
                    {
                        Color = App.SecondColor.Color,
                        Offset = 0.3
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.7 ? App.repGradient[3] : App.SecondColor.Color,
                        Offset = 0.31
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.7 ? App.repGradient[3] : App.SecondColor.Color,
                        Offset = 0.39
                    },
                    new GradientStop()
                    {
                        Color = App.SecondColor.Color,
                        Offset = 0.4
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.6 ? App.repGradient[4] : App.SecondColor.Color,
                        Offset = 0.41
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.6 ? App.repGradient[4] : App.SecondColor.Color,
                        Offset = 0.49
                    },
                    new GradientStop()
                    {
                        Color = App.SecondColor.Color,
                        Offset = 0.5
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.5 ? App.repGradient[5] : App.SecondColor.Color,
                        Offset = 0.51
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.5 ? App.repGradient[5] : App.SecondColor.Color,
                        Offset = 0.59
                    },
                    new GradientStop()
                    {
                        Color = App.SecondColor.Color,
                        Offset = 0.6
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.4 ? App.repGradient[6] : App.SecondColor.Color,
                        Offset = 0.61
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.4 ? App.repGradient[6] : App.SecondColor.Color,
                        Offset = 0.69
                    },
                    new GradientStop()
                    {
                        Color = App.SecondColor.Color,
                        Offset = 0.7
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.3 ? App.repGradient[7] : App.SecondColor.Color,
                        Offset = 0.71
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.3 ? App.repGradient[7] : App.SecondColor.Color,
                        Offset = 0.79
                    },
                    new GradientStop()
                    {
                        Color = App.SecondColor.Color,
                        Offset = 0.8
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.2 ? App.repGradient[8] : App.SecondColor.Color,
                        Offset = 0.81
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.2 ? App.repGradient[8] : App.SecondColor.Color,
                        Offset = 0.89
                    },
                    new GradientStop()
                    {
                        Color = App.SecondColor.Color,
                        Offset = 0.9
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.1 ? App.repGradient[9] : App.SecondColor.Color,
                        Offset = 0.91
                    },
                    new GradientStop()
                    {
                        Color = report.OverallRating >= 0.1 ? App.repGradient[9] : App.SecondColor.Color,
                        Offset = 1
                    }
                    }
                    };
                    rOverallRating.Fill = ovbrush;
                    ReportHeader.Content = $"Отчет №{App.reports.IndexOf(report)} от {report.Client.Nameofpc} ({report.Client.Id}):";
                    ReportOutput.Text += $"Вердикт нейросети клиента: {report.OverallRating}\nВердикт по клавиатуре: {report.KeyBoardRating}\nВердикт по мышке: {report.MouseRating}\nВердикт по процессам: {report.ProcessRating}\n--------------------------------\nИнформация по нажатым клавишам:{sKeyPressedInfo}\n{(report.IsMouseCoordChanged ? "Замечено движение курсора" : "Движение курсора не было замечено")}\nИнформация по активным приложениям:{sLastProcesses}\nПо сравнению с первоначальными замерами, изменилось {report.ProcessChangedCount} процессов (список:){sOldProcesses}\nИнформация по текущим сайтам:{sCurrentSites}\nХост: {report.Server.DnsSafeHost} ({report.Server.AbsolutePath})\nОтчет подготовлен AMWE Client'ом компьютера {report.Client.Nameofpc} и обработан AMWE Administrator для {App.Username}\n{DateTime.Now} по локальному времени";
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex, false);
                    _ = Dispatcher.BeginInvoke(new Action(() =>
                      {
                          ReportOutput.Text += $"Во время обработки отчета возникло исключение (Оно занесено в Меню -> Диагностика)\n{ex.Message}\n\n";
                      }));
                }
            }));
        }

        private async Task LoadKeyboardOutput(Report report)
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    foreach (KeyPressedInfo item in report.KeyPressedInfo)
                    {
                        sKeyPressedInfo += $"\n{item.Key} - {item.PressedCount}";
                        try
                        {
                            string[] arr = item.Key.Split(", ");
                            foreach (string key in arr)
                            {
                                Rectangle rect = m3md2.WinHelper.FindChild<Rectangle>(gKeyboard, key);
                                byte alpha = (byte)Math.Ceiling((double)(255 * (double)((double)item.PressedCount / arr.Length / report.PressingCount)));
                                byte fillalpha = (rect.Fill as SolidColorBrush)?.Color.A ?? 0;
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
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex, false);
                    _ = Dispatcher.BeginInvoke(new Action(() =>
                      {
                          ReportOutput.Text += $"Во время обработки отчета возникло исключение (Оно занесено в Меню -> Диагностика)\n{ex.Message}\n\n";
                      }));
                }
            }));
        }

        private async Task LoadProcessesOutput(Report report)
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    foreach (string item in report.OldProcesses)
                    {
                        sOldProcesses += $"\n{item}";
                    }
                    foreach (string item in report.LastProcesses)
                    {
                        sLastProcesses += $"\n{item}";
                    }

                    List<string> added = report.LastProcesses.Except(report.OldProcesses).ToList();
                    List<string> removed = report.OldProcesses.Except(report.LastProcesses).ToList();

                    string sChangesProcesses = "";
                    foreach (string item in added)
                    {
                        sChangesProcesses += $"+ {item}\n";
                    }
                    foreach (string item in removed)
                    {
                        sChangesProcesses += $"- {item}\n";
                    }

                    gpOverallInfo.Content = $"Количество измененных процессов: {report.ProcessChangedCount}";
                    if (sOldProcesses is not "")
                        tbLast.Text = sOldProcesses?.Remove(0, 1);
                    if (sLastProcesses is not "")
                        tbCurrent.Text = sLastProcesses?.Remove(0, 1);
                    tbChanges.Text = sChangesProcesses;
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex, false);
                    _ = Dispatcher.BeginInvoke(new Action(() =>
                      {
                          ReportOutput.Text += $"Во время обработки отчета возникло исключение (Оно занесено в Меню -> Диагностика)\n{ex.Message}\n\n";
                      }));
                }
            }));
        }

        private async Task LoadSitesOutput(Report report)
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    foreach (Site item in report.OldSites)
                    {
                        sOldSites += $"\n{item.SiteUri.DnsSafeHost} - {item.Header}";
                    }
                    foreach (Site item in report.CurrentSites)
                    {
                        sCurrentSites += $"\n{item.SiteUri.DnsSafeHost} - {item.Header}";
                    }

                    List<string> added = report.CurrentSites.Select(x => x.SiteUri.DnsSafeHost).Except(report.OldSites.Select(x => x.SiteUri.DnsSafeHost)).ToList();
                    List<string> removed = report.OldSites.Select(x => x.SiteUri.DnsSafeHost).Except(report.CurrentSites.Select(x => x.SiteUri.DnsSafeHost)).ToList();

                    string sChangesSites = "";
                    foreach (string item in added)
                    {
                        sChangesSites += $"+ {item}\n";
                    }
                    foreach (string item in removed)
                    {
                        sChangesSites += $"- {item}\n";
                    }

                    lBrowser.Content = $"Используемый браузер: {report.Browser}";
                    tbSiteLast.Text = sOldSites.Remove(0, 1);
                    tbSiteCurrent.Text = sCurrentSites.Remove(0, 1);
                    tbSiteChanges.Text = sChangesSites;
                }
                catch (Exception ex)
                {
                    ExceptionHandler.RegisterNew(ex, false);
                    _ = Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ReportOutput.Text += $"Во время обработки отчета возникло исключение (Оно занесено в Меню -> Диагностика)\n{ex.Message}\n\n";
                    }));
                }
            }));
        }
    }
}
