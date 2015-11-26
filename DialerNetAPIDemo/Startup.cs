using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DialerNetAPIDemo.Startup))]
namespace DialerNetAPIDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
