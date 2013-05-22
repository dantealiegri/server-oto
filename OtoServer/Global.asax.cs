using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Configuration;

namespace OtoServer
{
    public class Global : System.Web.HttpApplication
    {
        public class OtoApp : AppHostBase
        {
            private AppConfig config;
            public OtoApp() : base("Omaha Terminal Operator", typeof(OtoFiles).Assembly) 
            {
                config = new AppConfig( new ConfigurationResourceManager());
                DataStore.RedisStore.Initialize(config);
            }

            public override void Configure(Funq.Container container)
            {
                SetConfig(
                    new EndpointHostConfig
                    {
                        GlobalResponseHeaders =
                        {
                            { "Access-Control-Allow-Methods", "GET,POST" }
                        }
                    });
                container.Register(config);

                // if no directory, add.
            }


        }

        void Application_Start(object sender, EventArgs e)
        {
           // Console.WriteLine("Started");
           // Omaha.Testing.Serialize();
            (new OtoApp()).Init();
        }

    }
}
