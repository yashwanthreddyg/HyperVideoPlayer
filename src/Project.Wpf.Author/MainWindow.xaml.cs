using Microsoft.Extensions.Logging;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ILogger<MainWindow> logger, MediaManager mediaManager)
        {
            InitializeComponent();

            IMetadata metadata = mediaManager.GetMetadataFor("D:/gitwork/CSCI_576_Project/data/London/LondonOne");
            logger.LogDebug("in constructor");
            metadata.RemoveMediaLink(Guid.Parse("429b42fe-4a1e-448a-aa26-13ff6494ea08"));
            metadata.Save();
            BitmapImage bi = (BitmapImage)metadata.GetBitmapImageForFrame(1);
            metadata.Save();
        }
    }
}
