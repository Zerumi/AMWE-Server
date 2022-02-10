// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System.Windows;

namespace AMWE_Administrator
{
    /// <summary>
    /// Логика взаимодействия для CheckModelMaster.xaml
    /// </summary>
    public partial class CheckModelMaster : Window
    {
        private readonly CheckModelIndex modelIndex;

        public CheckModelMaster(CheckModelIndex modelIndex)
        {
            InitializeComponent();
            this.modelIndex = modelIndex;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbContent.Text) || string.IsNullOrWhiteSpace(tbTranscription.Text))
            {
                _ = MessageBox.Show("(9.3) Одно из значений является пустой строкой");
                return;
            }

            CheckModel newModel = new()
            {
                Content = tbContent.Text,
                Transcription = tbTranscription.Text,
                IsEnabled = cbEnable.IsChecked.GetValueOrDefault()
            };

            switch (modelIndex)
            {
                case CheckModelIndex.Apps:
                    newModel.FrameworkName = $"cbA{App.AppsToCheck.Count + 1}";
                    App.AppsToCheck.Add(newModel);
                    break;
                case CheckModelIndex.Sites:
                    newModel.FrameworkName = $"cbS{App.SitesToCheck.Count + 1}";
                    App.SitesToCheck.Add(newModel);
                    break;
                default:
                    break;
            }

            Close();
        }
    }
}
