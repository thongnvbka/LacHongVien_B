using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ViELearn.BackEnd.Startup))]
namespace ViELearn.BackEnd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
