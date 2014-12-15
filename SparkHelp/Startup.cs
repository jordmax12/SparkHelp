using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SparkHelp.Startup))]
namespace SparkHelp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
