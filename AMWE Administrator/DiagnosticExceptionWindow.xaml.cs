// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для DiagnosticExceptionWindow.xaml
    /// </summary>
    public partial class DiagnosticExceptionWindow : Window
    {
        public DiagnosticExceptionWindow()
        {
            Background = App.MainColor;
            InitializeComponent();
        }

        private readonly List<Button> buttons = new();

        private void ExsLoaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < m3md2.StaticVariables.ExceptionCount; i++)
            {
                buttons.Add(new Button()
                {
                    Name = "Button" + buttons.Count,
                    Content = $"{m3md2.StaticVariables.exceptions[buttons.Count].Message}"
                });
                buttons[i].Click += Ex_Click;
                _ = Exceptions.Children.Add(buttons[i]);
            }
        }

        private void Ex_Click(object sender, EventArgs e)
        {
            _ = MessageBox.Show(m3md2.StaticVariables.exceptions[Convert.ToInt32((sender as FrameworkElement).Name.Replace("Button", ""))].ToString());
        }
    }
}
