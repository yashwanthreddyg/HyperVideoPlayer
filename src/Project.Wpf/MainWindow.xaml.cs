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
        public MainWindow()
        {
            InitializeComponent();
            IMetadata metadata = MediaManager.GetMetadataFor("D:/gitwork/CSCI_576_Project/data/London/LondonOne");
            //metadata.AddMediaLink(new Core.Models.MediaLink
            //{
            //    FromX = 67
            //});
            metadata.RemoveMediaLink(Guid.Parse("97e443ed-a55e-473f-a27c-afbc5effb05c"));
            metadata.Save();
        }
    }
}
