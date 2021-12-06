using Project.Core;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project.Wpf.Author
{
    public partial class VideoViewer : UserControl
    {
        public MediaManager MediaManager { get; set; }
        private IMetadata _metadata;
        private uint _currentFrame = 1;
        private bool _drawing = false;
        private Point _origin;

        public VideoViewer()
        {
            InitializeComponent();
        }

        public void LoadVideo(string videoPath)
        {
            _metadata = MediaManager.GetMetadataFor(videoPath);

            this._slider.Maximum = _metadata.GetFrameCount();
            this._slider.Minimum = 1;
            this._slider.IsEnabled = true;

            UpdateFrame();
            EnableControls();
        }

        private void RenderBoxesForCurrentFrame()
        {
            return;
            foreach (Box box in _metadata.GetBoxesForFrame(_currentFrame))
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

        private void UpdateFrame()
        {
            this._imageWindow.Source = this._metadata.GetBitmapImageForFrame(this._currentFrame) as BitmapSource;
            RenderBoxesForCurrentFrame();
        }

        private void EnableControls()
        {
            this._slider.IsEnabled = true;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._currentFrame = (uint)Math.Round(e.NewValue);
            UpdateFrame();
        }

        private void _imageCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _drawing = true;
            _origin = e.GetPosition(this);
        }

        private void _imageCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _drawing = false;
            return;
        }

        private void _imageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_drawing)
            {
                _imageCanvas.Children.Clear();

                Point p = e.GetPosition(this);

                Rectangle rect;
                rect = new Rectangle();
                rect.Stroke = new SolidColorBrush(Colors.Red);

                rect.Width = Math.Abs(p.X - _origin.X);
                rect.Height = Math.Abs(p.Y - _origin.Y);
                Canvas.SetLeft(rect, _origin.X);
                Canvas.SetTop(rect, _origin.Y);

                _imageCanvas.Children.Add(rect);
                Panel.SetZIndex(rect, 5);
            }
        }
    }
}
