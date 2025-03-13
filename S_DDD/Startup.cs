using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(S_DDD.Startup))]
namespace S_DDD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
