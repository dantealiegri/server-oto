using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Redis;
using ServiceStack.ServiceHost;
using System.Security.Cryptography;
using System.IO;
using ServiceStack.Common.Web;

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
        public static void Initialize( AppConfig config )
        {
            instance = new RedisStore( config );
        }

        private RedisClient _client;
        private AppConfig _config;
        //private List<App> _cached;
        private Dictionary<object,App> _cached;
        private Dictionary<string, Dictionary<string, object>> _cached_version_keys; // by guid, version mapped to id.
        private SHA1 hasher;

        private RedisStore( AppConfig config )
        {
            hasher = new SHA1Managed();
            _config = config;
            _client = new RedisClient();
            _cached = null; _cached_version_keys = null;
            // DeleteAll and AddDemoApps are for testing.

            //_client.DeleteAll<RedisApp>();
            //_client.DeleteAll<RedisAppVersion>();
            if( KnownApps.Count == 0 )
            {
                AddDemoApps();
                _cached = null; _cached_version_keys = null;
            }

        }

        public override List<App> KnownApps
        {
            get
            {
                if (_cached == null)
                    FillCache();
                return _cached.Values.ToList();
            }
        }

        private void FillCache()
        {
                IList<RedisApp> r_apps;
                Dictionary<long,AppVersion> r_version = new Dictionary<long,AppVersion>();
                Dictionary<object, App> apps = new Dictionary<object, App>();

                using (var app_redis = _client.As<RedisApp>())
                {
                    r_apps = app_redis.GetAll();
                }

                _cached_version_keys = new Dictionary<string, Dictionary<string, object>>();
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
                        current = app.current_id == 0 ? null : r_version[app.current_id],
                        versions = r_version.Where( kvp => app.version_ids !=null && app.version_ids.Contains(kvp.Key)).Select( kvp => kvp.Value ).ToList()
                    };
                    _cached_version_keys[app.guid] = new Dictionary<string, object>();
                    foreach( KeyValuePair<long,AppVersion> pair in r_version.Where( kvp => app.version_ids != null && app.version_ids.Contains( kvp.Key )))
                    {
                        _cached_version_keys[app.guid][pair.Value.version] = pair.Key;
                    }


                    apps[ app.Id ] = translated;
                }
                _cached = apps;
            }

        public override bool AddApp(string appname, string appguid)
        {
            if (KnownApps.Select(ka => ka.guid).Contains(appguid))
                return false;

            using (var redis_app = _client.As<RedisApp>())
            {
                RedisApp new_app = new RedisApp { Id = redis_app.GetNextSequence(), name = appname, guid = appguid };
                redis_app.Store(new_app);
                _cached = null; _cached_version_keys = null;
            }

            return true;
        }

        public override bool AddAppVersion(string appguid, string appversion)
        {
            if ( ! KnownApps.Select(ka => ka.guid).Contains(appguid))
                return false;

            if (KnownApps.Single(ka => ka.guid == appguid).versions.Select(av => av.version).Contains(appversion))
                return false;

            using (var redis_ver = _client.As<RedisAppVersion>())
            using (var redis_app = _client.As<RedisApp>())
            {
                RedisAppVersion new_version = new RedisAppVersion { Id = redis_app.GetNextSequence(), version = appversion };
                redis_ver.Store(new_version);
                RedisApp current_app = redis_app.GetById(_cached.Single(kvp => kvp.Value.guid == appguid).Key);
                if (current_app.version_ids == null)
                    current_app.version_ids = new List<long>();
                current_app.version_ids.Add(new_version.Id);
                redis_app.Store(current_app);
                _cached = null; _cached_version_keys = null;
            }

            return true;
        }

        public override bool AddAppVersionFile(string appguid, string appversion, IFile[] files)
        {
            if ( ! KnownApps.Select(ka => ka.guid).Contains(appguid))
                return false;

            if (! KnownApps.Single(ka => ka.guid == appguid).versions.Select(av => av.version).Contains(appversion))
                return false;

            var targetDir = new FileInfo( Path.Combine( _config.RootDirectory, appguid + "/"+appversion));

            if (!Directory.Exists(targetDir.FullName))
                Directory.CreateDirectory(targetDir.FullName);

            using( var redis_version = _client.As<RedisAppVersion>() )
            {
                RedisAppVersion this_version = redis_version.GetById(_cached_version_keys[appguid][appversion]);
                foreach (IFile file in files)
                {
                    string sha1hash = Convert.ToBase64String(hasher.ComputeHash(file.InputStream));
                    file.InputStream.Seek(0, SeekOrigin.Begin);
                    if (this_version.package == null)
                        this_version.package = new List<AppPackage>();
                    this_version.package.Add(new AppPackage { size = (uint)file.ContentLength, name = file.FileName, required = true, hash = sha1hash });
                    file.SaveTo(Path.Combine(targetDir.FullName, file.FileName));
                }
                redis_version.Store(this_version);
                _cached = null; _cached_version_keys = null;
            }

            return true;
        }

        public override object GetAppVersionFile(string appguid, string appversion, string filename)
        {
            var targetDir = new FileInfo( Path.Combine( _config.RootDirectory, appguid + "/"+appversion));

            if (!Directory.Exists(targetDir.FullName))
            {
                throw HttpError.NotFound( String.Format("Application {0}, version {1} not found.", appguid, appversion ));
            }
            var targetFile = new FileInfo(Path.Combine(targetDir.FullName, filename));
            if ( ! targetFile.Exists)
            {
                throw HttpError.NotFound( String.Format("No such file {2} in Application {0}, version {1}.", appguid, appversion, filename ));
            }

            return new HttpResult(new MemoryStream(File.ReadAllBytes(targetFile.FullName)), "application/octect-stream");

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
                        version_ids = new List<long> { omaha_version.Id },
                        current_id = omaha_version.Id
                    };

                    chrome = new RedisApp
                    {
                        Id = redis_app.GetNextSequence(),
                        name = "Chrome",
                        guid = "{D0AB2EBC-931B-4013-9FEB-C9C4C2225C8C}",
                        version_ids = new List<long> { chrome_version.Id },
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