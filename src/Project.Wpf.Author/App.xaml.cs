using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using Serilog;
using Project.Core;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace Project.Wpf.Author
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
              .Enrich.FromLogContext()
              .WriteTo.File("Author.log")
              .CreateLogger();

            _host = new HostBuilder()
                       .ConfigureServices((context, services) =>
                       {
                           services.AddSingleton<MainWindow>();
                           services.AddSingleton<Func<Bitmap, object>>((Bitmap bmp) =>
                           {
                               using (MemoryStream memory = new MemoryStream())
                               {
                                   bmp.Save(memory, ImageFormat.Bmp);
                                   memory.Position = 0;
                                   BitmapImage bitmapImage = new BitmapImage();
                                   bitmapImage.BeginInit();
                                   bitmapImage.StreamSource = memory;
                                   bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                   bitmapImage.EndInit();
                                   return bitmapImage;
                               }
                           });
                           services.AddSingleton<MediaManager>();
                           services.AddLogging(loggingBuilder =>
                           {
                               loggingBuilder.AddSerilog(dispose: true);
                           });

                       })
                       .Build();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetService<MainWindow>();
            mainWindow.Show();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }
    }
}
