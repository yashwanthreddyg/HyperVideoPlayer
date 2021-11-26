using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Project.Core;
using System.Windows.Media;
using System.Windows.Controls;

namespace Project.Wpf.Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogger _logger;
        private readonly MediaManager _mediaManager;
        private bool _isVideoPlaying = false;
        private uint _currentFrame = 0;
        private IMetadata? _metadata;

        public MainWindow(ILogger<MainWindow> logger, MediaManager mediaManager)
        {
            InitializeComponent();

            _logger = logger;
            _mediaManager = mediaManager;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    _metadata = _mediaManager.GetMetadataFor(dialog.SelectedPath);
                    
                    this._slider.Maximum = _metadata.GetFrameCount();
                    this._slider.Minimum = 1;
                    this._slider.IsEnabled = true;
                    this._currentFrame = 1;

                    UpdateFrame();
                    EnableControls();
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Play()
        {
            // TODO - play the video at 30fps
        }

        private void PlayButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this._isVideoPlaying)
            {
                this._isVideoPlaying = false;
                this._playButtonImage.Source = new BitmapImage(new Uri("pack://application:,,,/icons8-play-button-circled-100.png"));
            }
            else
            {
                this._isVideoPlaying = true;
                this._playButtonImage.Source = new BitmapImage(new Uri("pack://application:,,,/icons8-pause-button-100.png"));

                Play();
            }
        }

        private void RenderBoxesForCurrentFrame()
        {
            // TODO : for each box, render on canvas.
            System.Windows.Shapes.Rectangle rect;
            rect = new System.Windows.Shapes.Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Black);
            //rect.Fill = new SolidColorBrush(Colors.Black);
            rect.Width = 100; // Replace with box.width
            rect.Height = 100; // Replace with box.height
            Canvas.SetLeft(rect, 0); // Replace with box.X
            Canvas.SetTop(rect, 0); // replace with box.Y
            
            _imageCanvas.Children.Add(rect);
            Panel.SetZIndex(rect, 2);
            //List<Core.Models.Box> boxes = _metadata.GetBoxesForFrame(this._currentFrame);

            //foreach (Core.Models.Box box in boxes)
            //{
                
            //}
        }

        private void EnableControls()
        {
            this._playButton.IsEnabled = true;
            this._stopButton.IsEnabled = true;
            this._slider.IsEnabled = true;
        }

        private void DisableControls()
        {
            this._playButton.IsEnabled = false;
            this._stopButton.IsEnabled = false;
            this._slider.IsEnabled = false;
        }

        private void UpdateFrame()
        {
            this._imageWindow.Source = this._metadata?.GetBitmapImageForFrame(this._currentFrame) as BitmapSource;
            RenderBoxesForCurrentFrame();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._currentFrame = (uint)Math.Round(e.NewValue);
            UpdateFrame();
        }

        private void StopButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._slider.Value = 0;
            this._imageWindow.Source = null;
            this._isVideoPlaying = false;

            DisableControls();
        }
    }
}
