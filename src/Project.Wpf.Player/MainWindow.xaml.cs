using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Project.Core;
using Project.Core.Models;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;

namespace Project.Wpf.Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly double MS_PER_FRAME = 1000.0 / 30;
        private readonly ILogger _logger;
        private readonly MediaManager _mediaManager;
        private bool _isVideoPlaying = false;
        private uint _currentFrame = 0;
        private IMetadata? _metadata;
        private System.Windows.Forms.Timer timerForVideoSync;

        public MainWindow(ILogger<MainWindow> logger, MediaManager mediaManager)
        {
            InitializeComponent();

            _logger = logger;
            _mediaManager = mediaManager;
            

            timerForVideoSync = new System.Windows.Forms.Timer();
            timerForVideoSync.Interval = (int) 60;

            timerForVideoSync.Tick += syncVideo;
            timerForVideoSync.Start();

            EnableControls();
        }


        void syncVideo(object sender, EventArgs e)
        {
            if(this._isVideoPlaying)
            {
                uint frameNum = (uint) Math.Floor(_mediaElement.Position.TotalMilliseconds / MS_PER_FRAME) + 1;

                if (frameNum > _metadata.GetFrameCount())
                {
                    StopMedia();
                    return;
                }

                _currentFrame = frameNum;
                this._slider.Value = frameNum;
                this._imageWindow.Source = this._metadata?.GetBitmapImageForFrame(frameNum) as BitmapSource;
                RenderBoxesForCurrentFrame();
            }
        }

        private void LoadVideo(string videoPath, uint currentFrame = 0)
        {
            _metadata = _mediaManager.GetMetadataFor(videoPath);

            this._slider.Maximum = _metadata.GetFrameCount();
            this._slider.Minimum = 1;
            this._slider.IsEnabled = true;

            _mediaElement.Source = _metadata.GetAudioPath();

            if (currentFrame > 0)
            {
                this._currentFrame = currentFrame;
                var ts = TimeSpan.FromMilliseconds(currentFrame* this.MS_PER_FRAME);

                _mediaElement.Position = ts;
            }
            else
            {
                this._currentFrame = 1;
            }
            
            UpdateFrame();
            EnableControls();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    LoadVideo(dialog.SelectedPath);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Play()
        {
            _mediaElement.Play();
            this._isVideoPlaying = true;
        }

        private void Pause()
        {
            _mediaElement.Pause();
            this._isVideoPlaying = false;
        }

        private void PlayButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this._isVideoPlaying)
            {
                this._isVideoPlaying = false;
                this._playButtonImage.Source = new BitmapImage(new Uri("pack://application:,,,/icons8-play-button-circled-100.png"));
                Pause();
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
            foreach (Box box in GetMockBoxes())
            {
                System.Windows.Shapes.Rectangle rect;
                rect = new System.Windows.Shapes.Rectangle();
                rect.Stroke = new SolidColorBrush(Colors.Black);
                
                rect.Width = box.Width;
                rect.Height = box.Height;
                Canvas.SetLeft(rect, box.X);
                Canvas.SetTop(rect, box.Y);

                _imageCanvas.Children.Add(rect);
                Panel.SetZIndex(rect, 1);
            }
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
            _mediaElement.Position = TimeSpan.FromMilliseconds(this._currentFrame * MS_PER_FRAME);
            UpdateFrame();
        }

        private void StopMedia()
        {
            this._slider.Value = 0;
            this._imageWindow.Source = null;
            this._isVideoPlaying = false;
            this._mediaElement.Stop();
            this._playButtonImage.Source = new BitmapImage(new Uri("pack://application:,,,/icons8-play-button-circled-100.png"));
            DisableControls();
        }

        private void StopButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StopMedia();
        }

        private void _mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {

        }

        private void _mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {

        }        

        private bool IsBoundedByBox(Point p, Box box)
        {
            if (box.X <= p.X && box.Y <= p.Y)
            {
                if (box.X + box.Width >= p.X && box.Y + box.Height >= p.Y)
                {
                    return true;
                }
            }
            return false;
        }

        private List<Box> GetMockBoxes()
        {
            List<Box> boxes = new List<Box>();

            boxes.Add(new Box(20, 30, 50, 60, new MediaLink(0, 1000, 0, 0, 0, 0, 0, 0, 0, 0, @"C:\repos\MSD_Datasets\NewYorkCity\NewYorkCity\NYOne")));
            boxes.Add(new Box(120, 130, 50, 60, new MediaLink(0, 7500, 0, 0, 0, 0, 0, 0, 0, 0, @"C:\repos\MSD_Datasets\London\LondonOne")));
            boxes.Add(new Box(170, 220, 20, 30, new MediaLink(0, 6000, 0, 0, 0, 0, 0, 0, 0, 0, @"C:\repos\MSD_Datasets\AIFilmTwo")));

            return boxes;
        }

        private void Image_Clicked(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);

            // List<Box> boxes = _metadata.GetBoxesForFrame(this._currentFrame);
            var boxes = GetMockBoxes();

            foreach (Box box in boxes)
            {
                if (IsBoundedByBox(p, box))
                {
                    LoadVideo(box.MediaLink.ToVideo, box.MediaLink.ToFrame);
                    return;
                }
            }
        }
    }
}
