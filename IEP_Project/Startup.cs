using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IEP_Project.Startup))]
namespace IEP_Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
