﻿using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(WageringGG.Server.Areas.Identity.IdentityHostingStartup))]
namespace WageringGG.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}