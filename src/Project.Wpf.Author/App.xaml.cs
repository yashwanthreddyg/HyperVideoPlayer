using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Serilog;

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
                           services.AddLogging(loggingBuilder =>
                           {
                               loggingBuilder.AddSerilog(dispose: true);
                           });
                           ;
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
