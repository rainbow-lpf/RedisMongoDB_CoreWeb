using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RedisMongoDB_CoreWeb.Startup))]
namespace RedisMongoDB_CoreWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
