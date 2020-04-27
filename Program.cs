using System;
using Autofac;
using Autofac.Core;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using SignalMan.Models;
using SignalMan.ViewModels;
using Splat;
using Splat.Autofac;

namespace SignalMan
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            var container = new ContainerBuilder();
            ConfigureServices(container);
            ConfigureVm(container);
            container.UseAutofacDependencyResolver();

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();

        private static void ConfigureServices(ContainerBuilder container)
        {
            container.RegisterType<SignalRHubConnector>().As<IHubConnector>();
            container.Register((_) => ApplicationLogService.Default).As<IApplicationLogService>();
        }

        private static void ConfigureVm(ContainerBuilder container)
        {
            container.RegisterType<MainWindowViewModel>();
        }
    }
}
