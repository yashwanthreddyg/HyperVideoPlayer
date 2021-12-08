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
        bool _projectModified = false;

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
                    _mediaLinks = new List<MediaLink>(this._projectVideo._metadata.GetMediaLinks());
                    this._linkBox.ItemsSource = _mediaLinks;
                    this._linkBox.Items.Refresh();
                    this.Title = dialog.SelectedPath;
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

        private void _linkBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_linkBox.SelectedItem != null)
            {
                var mediaLink = (_linkBox.SelectedItem as Core.Models.MediaLink);
                _projectVideo.SetFrame(mediaLink.FromFrame, new Point(mediaLink.FromX, mediaLink.FromY));
            }   
        }

        private void CreateLink_Click(object sender, RoutedEventArgs e)
        {
            MediaLinkCreatorPanel panel = new MediaLinkCreatorPanel(currentVideoPath, _projectVideo._currentFrame, MediaManager);
            Window window = new Window
            {
                Title = "Create Media Link",
                Content = panel,
                Width = panel.Width,
                Height = panel.Height,
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
                _mediaLinks.Add(mediaLink);
                this._linkBox.Items.Refresh();

                if (!_projectModified)
                {
                    this.Title += "*";
                }
                _projectModified = true;
            }
        }

        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            _projectVideo._metadata.Save();
            _projectModified = false;
            this.Title = this.Title.Trim('*');
        }

        private void DeleteLink_Click(object sender, RoutedEventArgs e)
        {
            _projectVideo._metadata.RemoveMediaLink((this._linkBox.SelectedItem as MediaLink).Id);
            _mediaLinks.Remove(_linkBox.SelectedItem as MediaLink);
            this._linkBox.Items.Refresh();
            _projectVideo.UpdateFrame();

            if (!_projectModified)
            {
                this.Title += "*";
            }
            _projectModified = true;
        }
    }
}
