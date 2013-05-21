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

        public AppConfig()
        {
        }

        public AppConfig(IResourceManager resources)
        {
            this.TextFileExtensions = resources.GetList("TextFileExtensions");
            this.RootDirectory = resources.GetString("RootDirectory").MapHostAbsolutePath().MapHostAbsolutePath()
                .Replace('\\', Path.DirectorySeparatorChar);
        }

        public IList<string> TextFileExtensions { get; set; }
    }
}