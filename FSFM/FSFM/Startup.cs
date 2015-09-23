using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FSFM.Startup))]
namespace FSFM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
