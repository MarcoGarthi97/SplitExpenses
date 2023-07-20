using Owin;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
using SplitExpenses.Models;

[assembly: OwinStartup(typeof(SplitExpenses.Startup))]
namespace SplitExpenses
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var idProvider = new CustomUserIdProvider();

            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);

            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}