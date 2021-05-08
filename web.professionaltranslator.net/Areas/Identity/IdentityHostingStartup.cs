using Microsoft.AspNetCore.Hosting;
using web.professionaltranslator.net.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace web.professionaltranslator.net.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}