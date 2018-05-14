
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CommunicationApp.Startup))]
namespace CommunicationApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
