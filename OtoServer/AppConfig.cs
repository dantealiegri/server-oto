using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ServiceStack.Common.Utils;
using ServiceStack.Configuration;

namespace OtoServer
{
    public class AppConfig
    {
        public string RootDirectory { get; set; }
        public string RootUrl { get; set; }

        public AppConfig()
        {
        }

        public AppConfig(IResourceManager resources)
        {
            this.TextFileExtensions = resources.GetList("TextFileExtensions");
            this.RootUrl = resources.GetString("RootUrl");
            this.RootDirectory = resources.GetString("RootDirectory").MapHostAbsolutePath().MapHostAbsolutePath()
                .Replace('\\', Path.DirectorySeparatorChar);
        }

        public IList<string> TextFileExtensions { get; set; }
    }
}