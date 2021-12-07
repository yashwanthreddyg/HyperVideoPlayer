using Microsoft.Extensions.Logging;
using Project.Core;
using System.Collections.Generic;
using System.Windows;



namespace Project.Wpf.Author
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ILogger<MainWindow> logger, MediaManager mediaManager)
        {
            InitializeComponent();

            this._projectVideo.MediaManager = mediaManager;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this._projectVideo.LoadVideo(dialog.SelectedPath);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PLACEHOLDERBUTTON_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window
            {
                Title = "My User Control Dialog",
                Content = new MediaLinkCreatorPanel()
            };

            window.ShowDialog();
        }

        private void _linkBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_linkBox.SelectedItem != null)
            {
                _projectVideo.SetFrame((_linkBox.SelectedItem as Core.Models.MediaLink).FromFrame);
            }   
        }
    }
}
