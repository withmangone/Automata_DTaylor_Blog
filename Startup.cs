using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Automata_DTaylor_Blog.Startup))]
namespace Automata_DTaylor_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
