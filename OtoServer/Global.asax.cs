using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;
using System.Text;

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
                LogManager.LogFactory = new Log4NetFactory(true);
            }

            public override void Configure(Funq.Container container)
            {
#if false
                this.RequestFilters.Add((hreq, hreq2, dto) =>
                    {
                        byte[] rdata = new byte[hreq.ContentLength + 10];
                        ILog l = LogManager.GetLogger(GetType());
                        l.Debug("Content Length is " + hreq.ContentLength);
                        hreq.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                        hreq.InputStream.Read(rdata, 0, (int)hreq.ContentLength);
                        //hreq.InputStream.Read( rdata,0,hreq.ContentLength);
                        l.Debug("Full? " + Encoding.UTF8.GetString( rdata ));
                        l.Debug("Headers");
                        foreach (string k in new List<String>(hreq.Headers.AllKeys))
                        {
                            l.Debug(k + " : " + hreq.Headers[k]);

                        }


                    });
#endif
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
