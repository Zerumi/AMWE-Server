﻿// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using Microsoft.AspNetCore.SignalR.Client;
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
        private string sWarnedApps;
        private string sWarnedSites;

        private readonly Client client;
        private readonly ClientState state;

        public ReportWindow(Report report)
        {
            try
            {
                InitializeComponent();

                client = report.Client;
                state = MainWindow.clientStates.Find(x => x.Client.Id == client.Id);

                Grid.Background = App.MainColor;
                gKeyboard.Background = App.SecondColor;

                TextOutputTab.Background = App.ControlColor;
                KeyboardTab.Background = App.ControlColor;
                ProccessesTab.Background = App.ControlColor;
                SitesTab.Background = App.ControlColor;
                WarnActivityTab.Background = App.ControlColor;
                TextOutputTab.Foreground = App.FontColor;
                KeyboardTab.Foreground = App.FontColor;
                ProccessesTab.Foreground = App.FontColor;
                SitesTab.Foreground = App.FontColor;
                WarnActivityTab.Foreground = App.FontColor;

                Resources["TabControlBrush"] = App.ControlColor;
                Resources["TabSelectedBrush"] = App.ExtraColor;
                Resources["ButtonSelectedBrush"] = App.ButtonHighlightColor;

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

                foreach (Label obj in m3md2.WinHelper.FindVisualChildren<Label>(gActivity))
                {
                    obj.Foreground = App.FontColor;
                }

                foreach (Button obj in m3md2.WinHelper.FindVisualChildren<Button>(gActivity))
                {
                    obj.Background = App.ButtonColor;
                    obj.Foreground = App.FontColor;
                }

                tcReportVisualiser.Background = App.SecondColor;

                ReportOutput.Foreground = App.FontColor;
                ReportOutput.Background = App.SecondColor;
                ReportOutput.Text = string.Empty;

                tbWarnActivityOutput.Foreground = App.FontColor;

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
                tbSites.Foreground = App.FontColor;

                rOverallRating.Stroke = App.BorderColor;

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
                    await LoadWarnOutput(report);
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
                        Color = App.BorderColor.Color,
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
                        Color = App.BorderColor.Color,
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
                        Color = App.BorderColor.Color,
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
                        Color = App.BorderColor.Color,
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
                        Color = App.BorderColor.Color,
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
                        Color = App.BorderColor.Color,
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
                        Color = App.BorderColor.Color,
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
                        Color = App.BorderColor.Color,
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
                        Color = App.BorderColor.Color,
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
                    ReportOutput.Text += $"Вердикт нейросети клиента: {report.OverallRating}\nВердикт по клавиатуре: {report.KeyBoardRating}\nВердикт по мышке: {report.MouseRating}\nВердикт по процессам: {report.ProcessRating}\n--------------------------------\nИнформация по нажатым клавишам:{sKeyPressedInfo}\n{(report.IsMouseCoordChanged ? "Замечено движение курсора" : "Движение курсора не было замечено")}\nИнформация по активным приложениям:{sLastProcesses}\nПо сравнению с первоначальными замерами, изменились следующие процессы: {sOldProcesses}\nИнформация по текущим сайтам:{sCurrentSites}\nХост: {report.Server.DnsSafeHost} ({report.Server.AbsolutePath})\nОтчет подготовлен AMWE Client'ом компьютера {report.Client.Nameofpc} и обработан AMWE Administrator для {App.Username}\n{DateTime.Now} по локальному времени";
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

                    gpOverallInfo.Content = $"Текущий открытый процесс: {report.CurrentProccess}";
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
                    foreach (Site item in report.CurrentSites)
                    {
                        sCurrentSites += $"\n({item.Browser}): {item?.SiteUri?.DnsSafeHost ?? "Ссылку получить невозможно"} // \"{item.Header}\" * {item?.SiteUri?.AbsoluteUri ?? string.Empty}";
                    }

                    lBrowser.Content = $"Основной используемый браузер: {report.MainBrowser}";
                    tbSites.Text = sCurrentSites.Remove(0, 1);
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

        private async Task LoadWarnOutput(Report report)
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if ((report.SiteIntersection?.Count ?? 0) == 0 && (report.ProcIntersection?.Count ?? 0) == 0)
                    {
                        WarnActivityTab.Visibility = Visibility.Collapsed;
                        return;
                    }
                    if (state.IsEnhanced)
                    {
                        bEnhanceControl.Content = "Ослабить контроль";
                    }
                    else
                    {
                        bEnhanceControl.Content = "Усилить контроль";
                    }
                    foreach (CheckModel item in report.SiteIntersection ?? new List<CheckModel>())
                    {
                        sWarnedSites += $"{item.Transcription} - {item.Content}\n";
                    }
                    foreach (CheckModel item in report.ProcIntersection ?? new List<CheckModel>())
                    {
                        sWarnedApps += $"{item.Transcription} - {item.Content}\n";
                    }
                    tbWarnActivityOutput.Text = $"{((report.SiteIntersection?.Count ?? 0) != 0 ? $"Обнаруженные сайты: \n{sWarnedSites}" : "")}{((report.ProcIntersection?.Count ?? 0) != 0 ? $"Обнаруженные программы: \n{sWarnedApps}" : "")}";
                    lReportTime.Content = $"{report.Timestamp.ToLocalTime().ToShortDateString()} {report.Timestamp.ToLocalTime().ToLongTimeString()}";
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

        private void bMoreAboutUser_Click(object sender, RoutedEventArgs e)
        {
            UserReports userReports = new(client);
            userReports.Show();
        }

        private async void bEnhanceControl_Click(object sender, RoutedEventArgs e)
        {
            if (!state.IsEnhanced)
            {
                await MainWindow.ReportHandleConnection.InvokeAsync("EnhanceControl", client.Id);
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    bEnhanceControl.Content = "Ослабить контроль";
                }));
            }
            else
            {
                await MainWindow.ReportHandleConnection.InvokeAsync("LoosenControl", client.Id);
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    bEnhanceControl.Content = "Усилить контроль";
                }));
            }
        }

        private async void bOpenChat_Click(object sender, RoutedEventArgs e)
        {
            await (App.Current.Windows[0] as MainWindow).OpenChat(client.Id);
        }

        private void bIgnore_Click(object sender, RoutedEventArgs e)
        {
            state.IgnoreWarning = !state.IgnoreWarning;
            bIgnore.Content = state.IgnoreWarning ? "Оповещать" : "Игнорировать";
        }

        private void bSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new();
            settings.Show();
            _ = settings.Activate();
        }
    }
}
