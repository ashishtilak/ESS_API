using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ESS.Startup))]

namespace ESS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}