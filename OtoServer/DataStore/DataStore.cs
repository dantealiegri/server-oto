using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;

namespace OtoServer.DataStore
{
    public class App
    {
        public string guid { get; set; }
        public string name { get; set; }
        public AppVersion current { get; set; }
        public List<AppVersion> versions { get; set; }
    }

    public class AppVersion
    {
        public string version { get; set; } // must be w.x.y.z
        public List<string> url_locations { get; set; }
        public List<AppPackage> package { get; set; }
        public List<AppAction> actions { get; set; }
    }

    public class AppPackage
    {
        public string hash { get; set; }
        public string name { get; set; }
        public bool required { get; set; }
        public UInt32 size { get; set; }
    }

    public class AppAction
    {
        public string on_event { get; set; }
        public string run { get; set; }
        public string arguments { get; set; }
        public string version { get; set; }
        public string on_success { get; set; }
    }


    public abstract class DataStore : IDataStore
    {
        protected static DataStore instance = null;
        public static DataStore Instance() { return instance; }
        public abstract List<App> KnownApps { get; }
        public abstract bool AddApp( string name, string guid );
        public abstract bool AddAppVersion(string appguid, string appversion);
        public abstract bool AddAppVersionFile(string appguid, string appversion, IFile[] files);
        public abstract object GetAppVersionFile(string appguid, string appversion, string filename);
    }


    public interface IDataStore
    {
        List<App> KnownApps { get;}
        bool AddApp(string name, string guid);
        bool AddAppVersion(string appguid, string appversion);
        bool AddAppVersionFile(string appguid, string appversion, IFile[] files);
        object GetAppVersionFile(string appguid, string appversion, string filename);
    }

    public class TestDataStore : DataStore
    {
        private TestDataStore()
        {
        }
        public static void Initialize()
        {
            instance = new TestDataStore();
            ((TestDataStore)instance).TestPopulate();
        }

        public override bool AddApp(string name, string guid)
        {
            return false;
        }

        public override bool AddAppVersion(string appguid, string appversion)
        {
            return false;
        }

        public override bool AddAppVersionFile(string appguid, string appversion, IFile[] files)
        {
            return false;
        }

        public override object GetAppVersionFile(string appguid, string appversion, string filename)
        {
            return null;
        }


        public override List<App> KnownApps
        {
            get
            {
                return _apps;
            }
        }

        private List<App> _apps;
        private void TestPopulate()
        {
            App omaha = new App
            {
                name = "Omaha",
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
            omaha.current = omaha.versions[0];

            App chrome = new App
            {
                name = "Chrome",
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
                            } ),
                            actions = new List<AppAction>( new AppAction[] {
                                new AppAction { on_event = "install", arguments ="--do-not-launch-chrome", run="chrome_installer.exe" },
                                new AppAction { on_event = "postinstall", version ="13.0.782.112", on_success="exitsilentlyonlaunchcmd" }
                            })
                       
                    }
                }
                    )
            };
            chrome.current = chrome.versions[0];

            _apps = new List<App>(new App[] { omaha, chrome });

        }
    }
}