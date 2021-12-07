using Microsoft.Extensions.Logging;
using Project.Core;
using Project.Core.Models;
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
        public List<MediaLink> _mediaLinks = new List<MediaLink>();

        public MainWindow(ILogger<MainWindow> logger, MediaManager mediaManager)
        {
            InitializeComponent();
            
            this.MediaManager = mediaManager;
            this._projectVideo.MediaManager = mediaManager;

            this._linkBox.ItemsSource = _mediaLinks;
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

        public void OnRectangleDraw()
        {
            MediaLinkCreatorPanel panel = new MediaLinkCreatorPanel(currentVideoPath, _projectVideo._currentFrame, MediaManager);
            Window window = new Window
            {
                Title = "My User Control Dialog",
                Content = panel
            };

            window.ShowDialog();
            MediaLinkCreatorResult result = panel.result;

            if (result == null)
            {
                Console.WriteLine();
            }
            else
            {
                var mediaLink = new Core.Models.MediaLink(_projectVideo._currentFrame, result.destFrame, (uint)_projectVideo._origin.X, result.destX, (uint)_projectVideo._origin.Y, result.destY, (uint)_projectVideo.lastRectangle.Height, result.destHeight, (uint)_projectVideo.lastRectangle.Width, result.destWidth, result.linkedFile, result.linkName);
                _projectVideo._metadata.AddMediaLink(mediaLink);
                //_mediaLinks.Add(new LinkInfo() { LinkName = mediaLink.LinkName});
                _mediaLinks.Add(mediaLink);
                this._linkBox.Items.Refresh();
            }
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

            if (result == null)
            {
                Console.WriteLine();
            }
            else
            {
                var mediaLink = new Core.Models.MediaLink(_projectVideo._currentFrame, result.destFrame, (uint)_projectVideo._origin.X, result.destX, (uint)_projectVideo._origin.Y, result.destY, (uint)_projectVideo.lastRectangle.Height, result.destHeight, (uint)_projectVideo.lastRectangle.Width, result.destWidth, result.linkedFile, result.linkName);
                _projectVideo._metadata.AddMediaLink(mediaLink);
                //_mediaLinks.Add(new LinkInfo() { LinkName = mediaLink.LinkName});
                _mediaLinks.Add(mediaLink);
                this._linkBox.Items.Refresh();
            }
        }

        private void _linkBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_linkBox.SelectedItem != null)
            {
                _projectVideo.SetFrame((_linkBox.SelectedItem as Core.Models.MediaLink).FromFrame);
            }   
        }
    }

    public class LinkInfo
    {
        public string LinkName { get; set; }
    }
}
