using Nancy;
using Nancy.TinyIoc;
using Test.Components.Pcd8544.Server.Configuration;
using Test.Components.Pcd8544.Server.Display;

namespace Test.Components.Pcd8544.Server
{
    public class Bootstrapper : DefaultNancyBootstrapper {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {
            container.Register<IConfigProvider, AppConfigProvider>().AsSingleton();
            container.Register<IDisplayServer, DisplayServer>().AsSingleton();
        }
    }
}