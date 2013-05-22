using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Redis;

namespace OtoServer.DataStore
{
    public class RedisApp
    {
        public long Id { get; set; }
        public string guid { get; set; }
        public string name { get; set; }
        public long current_id { get; set; }
        public List<long> version_ids { get; set; }
    }

    public class RedisAppVersion
    {
        public long Id { get; set; }
        public string version { get; set; } // must be w.x.y.z
        public List<string> url_locations { get; set; }
        public List<AppPackage> package { get; set; }
        public List<AppAction> actions { get; set; }
    }


    public class RedisStore : DataStore
    {
        public static void Initialize()
        {
            instance = new RedisStore();
        }

        private RedisClient _client;
        private List<App> _cached;

        private RedisStore()
        {
            _client = new RedisClient();
            _cached = null;
            // DeleteAll and AddDemoApps are for testing.

            //_client.DeleteAll<RedisApp>();
            //_client.DeleteAll<RedisAppVersion>();
            if( KnownApps.Count == 0 )
            {
                AddDemoApps();
                _cached = null;
            }

        }

        public override List<App> KnownApps
        {
            get
            {
                if (_cached == null)
                    FillCache();
                return _cached;
            }
        }

        private void FillCache()
        {
                IList<RedisApp> r_apps;
                Dictionary<long,AppVersion> r_version = new Dictionary<long,AppVersion>();
                List<App> apps = new List<App>();

                using (var app_redis = _client.As<RedisApp>())
                {
                    r_apps = app_redis.GetAll();
                }

                using (var version_redis = _client.As<RedisAppVersion>())
                {
                    foreach (RedisAppVersion rv in version_redis.GetAll())
                        r_version[rv.Id] = new AppVersion { version = rv.version, actions = rv.actions, package = rv.package, url_locations = rv.url_locations };
                }

                foreach (RedisApp app in r_apps)
                {
                    App translated = new App
                    {
                        guid = app.guid,
                        name = app.name,
                        current = r_version[app.current_id],
                        versions = r_version.Where( kvp => app.version_ids !=null && app.version_ids.Contains(kvp.Key)).Select( kvp => kvp.Value ).ToList()
                    };

                    apps.Add(translated);
                }
                _cached = apps;
            }
    
    
        private void AddDemoApps()
        {
            RedisApp omaha, chrome;

            using (var redis_app = _client.As<RedisApp>())
            {
                using (var redis_version = _client.As<RedisAppVersion>())
                {
                    RedisAppVersion omaha_version = new RedisAppVersion
                    {
                        Id = redis_version.GetNextSequence(),
                        version = "1.3.23.0",
                        url_locations = new List<string>(new string[] { "http://localhost/omaha" }),
                        package = new List<AppPackage>(
                            new AppPackage[] {
                                new AppPackage {
                                    hash = "9813058901hjklh0wre" , name = "OmahaClient.exe", required = true, size = 42 }
                            })
                    };
                    RedisAppVersion chrome_version = new RedisAppVersion
                    {
                        Id = redis_version.GetNextSequence(),
                        version = "13.0.782.112",
                        url_locations = new List<string>(new string[] { "http://localhost/chrome" }),
                        package = new List<AppPackage>(
                            new AppPackage[] {
                                new AppPackage {
                                    hash = "012349jkgxc90asdhfjk99" , name = "chrome_installer.exe", required = true, size = 42 }
                            }),
                        actions = new List<AppAction>(new AppAction[] {
                                new AppAction { on_event = "install", arguments ="--do-not-launch-chrome", run="chrome_installer.exe" },
                                new AppAction { on_event = "postinstall", version ="13.0.782.112", on_success="exitsilentlyonlaunchcmd" }
                            })

                    };

                    redis_version.Store(omaha_version);
                    redis_version.Store(chrome_version);

                    List<RedisAppVersion> vs = redis_version.GetAll().ToList();


                    omaha = new RedisApp
                    {
                        Id = redis_app.GetNextSequence(),
                        name = "Omaha",
                        guid = "{430FD4D0-B729-4F61-AA34-91526481799D}",
                        version_ids = new List<long> { 1 },
                        current_id = omaha_version.Id
                    };

                    chrome = new RedisApp
                    {
                        Id = redis_app.GetNextSequence(),
                        name = "Chrome",
                        guid = "{D0AB2EBC-931B-4013-9FEB-C9C4C2225C8C}",
                        version_ids = new List<long> { 2 },
                        current_id = chrome_version.Id

                    };
                }
                redis_app.Store(omaha);
                redis_app.Store(chrome);

                List<RedisApp> avs = redis_app.GetAll().ToList();

            }




        }
    }
}