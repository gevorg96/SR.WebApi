using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SR.AcceptanceTests.Client;

namespace SR.AcceptanceTests.Emulators
{
    public class SrEmulator: ServerEmulator
    {
        protected override void Configure(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHttpClient<SrClient>(client => context.Configuration.Bind("Http:Client:SR", client))
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseDefaultCredentials = true });
        }

        public SrClient UsingClient => ServiceProvider.GetRequiredService<SrClient>();
    }
}