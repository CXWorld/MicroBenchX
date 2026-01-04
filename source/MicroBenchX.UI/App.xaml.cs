using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MicroBenchX.UI.Services;
using MicroBenchX.UI.ViewModels;
using MicroBenchX.UI.Views;

namespace MicroBenchX.UI;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(services);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
            })
            .Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Register Services
        services.AddSingleton<HardwareMonitorService>();

        // Register ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<PpcViewModel>();
        services.AddTransient<IpcViewModel>();
        services.AddTransient<LatencyViewModel>();
        services.AddTransient<InfoViewModel>();

        // Register Views
        services.AddSingleton<MainWindow>();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        _host.Start();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private async void Application_Exit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
    }

    public static T GetService<T>() where T : class
    {
        var app = (App)Current;
        return app._host.Services.GetRequiredService<T>();
    }
}
