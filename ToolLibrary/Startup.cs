using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ToolLibrary.Startup))]
namespace ToolLibrary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
