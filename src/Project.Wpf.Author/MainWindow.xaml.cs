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
    }

    public class LinkInfo
    {
        public string Name { get; set; }
        public int Frame { get; set; }
    }
}
