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
    /// <summary>
    /// Interaction logic for MediaLinkCreatorPanel.xaml
    /// </summary>
    public partial class MediaLinkCreatorPanel : UserControl
    {
        public class MediaLinkCreatorResult
        {
            public string linkName;
            public uint destX;
            public uint destY;
            public uint destWidth;
            public uint destHeight;
            public uint destFrame;
            public string linkedFile;
            public uint linkedFrame;
        }

        MediaManager MediaManager;
        public MediaLinkCreatorResult result;

        public MediaLinkCreatorPanel(String currentVideoPath, uint minFrame, MediaManager mm)
        {
            InitializeComponent();
            this._projectVideo.IsLinking = true;
            this._linkedVideo.IsLinking = true;
            this.MediaManager = mm;
            this._projectVideo.MediaManager = mm;
            this._linkedVideo.MediaManager = mm;
            this._projectVideo.LoadVideo(currentVideoPath);
            this._projectVideo._slider.Minimum = minFrame;
            this.Width = 800;
            this.Height = 474;
        }

        private void Save_Clicked(object sender, RoutedEventArgs e)
        {
            result = new MediaLinkCreatorResult();
            result.linkName = tb_linkName.Text;
            result.destX = (uint)_projectVideo._origin.X;
            result.destY = (uint)_projectVideo._origin.Y;
            result.destWidth = (uint)_projectVideo.lastRectangle.Width;
            result.destHeight = (uint)_projectVideo.lastRectangle.Height;
            result.destFrame = (uint)_projectVideo._slider.Value;
            result.linkedFile = _linkedVideo._metadata.GetPath();
            result.linkedFrame = (uint)_linkedVideo._slider.Value;
        }

        private void Open_Clicked(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this._linkedVideo.LoadVideo(dialog.SelectedPath);
                }
            }
        }

        private void LinkName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.tb_linkName.Text.Length > 0)
                this._saveButton.IsEnabled = true;
            else
                this._saveButton.IsEnabled = false;
        }
    }
}
