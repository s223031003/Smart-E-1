using Smart_E.Areas.Identity;
using Smart_E.Areas.Identity.Pages;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace Smart_E.Areas.Identity
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