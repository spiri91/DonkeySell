using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DonkeySellApi.Startup))]
namespace DonkeySellApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
