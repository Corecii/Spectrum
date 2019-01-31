using Autofac;
using Spectrum.Resonator.Infrastructure;
using Spectrum.Resonator.Infrastructure.Modules;
using System.Windows;

namespace Spectrum.Resonator
{
    public partial class App : Application
    {
        private static IContainer Container { get; set; }
        private ContainerBuilder ContainerBuilder { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ContainerBuilder = new ContainerBuilder();
            ContainerBuilder.RegisterModule<ViewModelModule>();
            ContainerBuilder.RegisterModule<ServiceModule>();

            Container = ContainerBuilder.Build();

            Resolver.Instance.Initialize(Container);
        }
    }
}
