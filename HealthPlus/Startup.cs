using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HealthPlus.Startup))]
namespace HealthPlus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
