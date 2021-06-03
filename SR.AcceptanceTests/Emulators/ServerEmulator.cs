using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SR.AcceptanceTests.Emulators
{
    public abstract class ServerEmulator : Emulator
    {
        protected sealed override IHost CreateHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => builder.AddJsonFile($"appsettings.json", true))
                .ConfigureServices(Configure)
                .Build();
        }

        protected abstract void Configure(HostBuilderContext context, IServiceCollection services);
    }
}