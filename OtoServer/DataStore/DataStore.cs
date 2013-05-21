using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtoServer.DataStore
{
    public class App
    {
        public string guid;
        public AppVersion current;
        public List<AppVersion> versions;
    }

    public class AppVersion
    {
        public string version; // must be w.x.y.z
        public List<string> url_locations;
        public List<AppPackage> package;
    }

    public class AppPackage
    {
        public string hash;
        public string name;
        public bool required;
        public UInt32 size;
        public List<AppAction> actions;
    }

    public class AppAction
    {
        public string on_event;
        public string run;
        public string arguments;
        public string version;
        public string on_success;
    }

    public class DataStore
    {
        private static DataStore instance = null;
        public static void Initialize()
        {
            instance = new DataStore();
            instance.TestPopulate();
        }

        public static DataStore Instance() { return instance; }

        private DataStore()
        {
        }

        private void TestPopulate()
        {
            App omaha = new App
            {
                guid = "{430FD4D0-B729-4F61-AA34-91526481799D}",
                versions = new List<AppVersion>(
                    new AppVersion[] {
                    new AppVersion {
                        version = "1.3.23.0",
                        url_locations = new List<string> ( new string[] { "http://localhost/omaha" } ),
                        package = new List<AppPackage>(
                            new AppPackage[] {
                                new AppPackage {
                                    hash = "9813058901hjklh0wre" , name = "OmahaClient.exe", required = true, size = 42 }
                            } )
                    }
                }
                    )
            };

            App chrome = new App
            {
                guid = "{D0AB2EBC-931B-4013-9FEB-C9C4C2225C8C}",
                versions = new List<AppVersion>(
                    new AppVersion[] {
                    new AppVersion {
                        version = "13.0.782.112",
                        url_locations = new List<string> ( new string[] { "http://localhost/chrome" } ),
                        package = new List<AppPackage>(
                            new AppPackage[] {
                                new AppPackage {
                                    hash = "012349jkgxc90asdhfjk99" , name = "chrome_installer.exe", required = true, size = 42 }
                            } )
                    }
                }
                    )
            };

            KnownApps = new List<App>(new App[] { omaha, chrome });

        }

        public List<App> KnownApps;
    }
}