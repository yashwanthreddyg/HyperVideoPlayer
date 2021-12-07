using Microsoft.Extensions.Logging;
using Project.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using static Project.Wpf.Author.MediaLinkCreatorPanel;

namespace Project.Wpf.Author
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaManager MediaManager { get; set; }
        string currentVideoPath = string.Empty;
        public MainWindow(ILogger<MainWindow> logger, MediaManager mediaManager)
        {
            InitializeComponent();
            this.MediaManager = mediaManager;
            this._projectVideo.MediaManager = mediaManager;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    currentVideoPath = dialog.SelectedPath;
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
            MediaLinkCreatorPanel panel = new MediaLinkCreatorPanel(currentVideoPath, _projectVideo._currentFrame, MediaManager);
            Window window = new Window
            {
                Title = "My User Control Dialog",
                Content = panel
            };

            window.ShowDialog();
            MediaLinkCreatorResult result = panel.result;

            // Do something with the result
            if(result == null)
            {
                Console.WriteLine();
            }
        }
    }

    public class LinkInfo
    {
        public string Name { get; set; }
        public int Frame { get; set; }
    }
}
