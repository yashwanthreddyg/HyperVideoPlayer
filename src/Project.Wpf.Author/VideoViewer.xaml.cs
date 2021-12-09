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
        public IMetadata _metadata;
        public uint _currentFrame = 1;
        private bool _drawing = false;
        public Point _origin;
        public Rectangle lastRectangle;
        public bool IsLinking = false;

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
            UpdateControls();
        }

        private void RenderBoxesForCurrentFrame()
        {
            if (IsLinking)
                return;

            _imageCanvas.Children.Clear();
            foreach (Box box in _metadata.GetBoxesForFrame(_currentFrame))
            {
                System.Windows.Shapes.Rectangle rect;
                rect = new System.Windows.Shapes.Rectangle();
                rect.Stroke = new SolidColorBrush(Colors.Red);
                rect.StrokeThickness = 5;

                rect.Width = box.Width;
                rect.Height = box.Height;
                Canvas.SetLeft(rect, box.X);
                Canvas.SetTop(rect, box.Y);

                _imageCanvas.Children.Add(rect);
                Panel.SetZIndex(rect, 1);
            }
        }

        private void HighlightRectangle(Point origin)
        {
            foreach (Rectangle rect in _imageCanvas.Children)
            {
                int x = (int)Canvas.GetLeft(rect);
                int y = (int)Canvas.GetTop(rect);

                if (x == origin.X && y == origin.Y)
                {
                    rect.Stroke = new SolidColorBrush(Colors.DarkCyan);
                }
            }
        }

        public void SetFrame(uint frame, Point highlightedRectangleOrigin)
        {
            this._currentFrame = frame;
            this._slider.ValueChanged -= Slider_ValueChanged;
            this._slider.Value = _currentFrame;
            this._slider.ValueChanged += Slider_ValueChanged;

            UpdateFrame();

            if (highlightedRectangleOrigin != null)
                HighlightRectangle(highlightedRectangleOrigin);
        }

        public void UpdateFrame()
        {
            this._imageWindow.Source = this._metadata.GetBitmapImageForFrame(this._currentFrame) as BitmapSource;
            RenderBoxesForCurrentFrame();
        }

        private void UpdateControls()
        {
            this._slider.IsEnabled = true;
            this.BackwardButton.IsEnabled = true;
            this.ForwardButton.IsEnabled = true;

            if (this._currentFrame <= 1)
                this.BackwardButton.IsEnabled = false;
            if (this._currentFrame == _metadata.GetFrameCount())
                this.ForwardButton.IsEnabled= false;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._currentFrame = (uint)Math.Round(e.NewValue);
            UpdateFrame();
            UpdateControls();
        }

        private void _imageCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _drawing = true;
            _origin = e.GetPosition(this);
            _imageCanvas.Children.Add(new Rectangle());
        }

        private void _imageCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _drawing = false;

            // Wait one second
            // Pop up dialog

            // Use result of MediaLink dialog (nullable)
            // Depending on result, update links ListView control or not
            ((this.Parent as Canvas).Parent as MainWindow).OnRectangleDraw();
            return;
        }

        private void _imageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_drawing && e.LeftButton == MouseButtonState.Pressed)
            {
                _imageCanvas.Children.RemoveAt(_imageCanvas.Children.Count - 1);
                Point p = e.GetPosition(this);

                Rectangle rect;
                rect = new Rectangle();
                rect.Stroke = new SolidColorBrush(Colors.Red);
                rect.StrokeThickness = 5;

                rect.Width = Math.Abs(p.X - _origin.X);
                rect.Height = Math.Abs(p.Y - _origin.Y);
                Canvas.SetLeft(rect, _origin.X);
                Canvas.SetTop(rect, _origin.Y);

                _imageCanvas.Children.Add(rect);
                Panel.SetZIndex(rect, 5);
                lastRectangle = rect;
            }
        }        

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            this._slider.Value = Math.Min(this._slider.Value + 10, _metadata.GetFrameCount());
        }

        private void Backward_Click(object sender, RoutedEventArgs e)
        {
            this._slider.Value = Math.Max(this._slider.Value - 10, 1);
        }
    }
}
