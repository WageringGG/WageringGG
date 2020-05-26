using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WageringGG.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Configuration.AddJsonFile("appsettings.json");
            if (builder.HostEnvironment.IsDevelopment())
                builder.Configuration.AddJsonFile("appsettings.Development.json");
            var config = builder.Configuration.Build();

            builder.RootComponents.Add<App>("app");
            builder.Services.AddHttpClient("WageringGG.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WageringGG.ServerAPI"));
            builder.Services.AddSingleton(config);
            builder.Services.AddApiAuthorization();

            await builder.Build().RunAsync();
        }
    }
}
