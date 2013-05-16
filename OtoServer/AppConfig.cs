using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Configuration;

namespace OtoServer
{
    public class AppConfig
    {
        public string RootDirectory { get; set; }

        public AppConfig()
        {
        }

        public AppConfig(IResourceManager resources)
        {
        }
    }
}