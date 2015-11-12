using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(iSelectManager.Startup))]
namespace iSelectManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
